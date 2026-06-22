using System.Net.Http.Json;
using System.Text.Json;
using ERPKeys.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ERPKeys.Infrastructure.Services;

public sealed class GoogleAddressLookupService : IAddressLookupService
{
    private const string AutocompleteFieldMask =
        "suggestions.placePrediction.placeId,suggestions.placePrediction.text.text";
    private const string DetailsFieldMask =
        "id,formattedAddress,addressComponents";

    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<GoogleAddressLookupService> _logger;

    public GoogleAddressLookupService(
        HttpClient http,
        IConfiguration configuration,
        ILogger<GoogleAddressLookupService> logger)
    {
        _http = http;
        _apiKey = configuration["GoogleMap:key"]?.Trim() ?? string.Empty;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AddressSuggestionDto>> GetSuggestionsAsync(
        string input,
        string sessionToken,
        CancellationToken ct = default)
    {
        EnsureConfigured();

        var normalizedInput = input.Trim();
        if (normalizedInput.Length < 4)
            return [];

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://places.googleapis.com/v1/places:autocomplete");
        request.Headers.Add("X-Goog-Api-Key", _apiKey);
        request.Headers.Add("X-Goog-FieldMask", AutocompleteFieldMask);
        request.Content = JsonContent.Create(new
        {
            input = normalizedInput,
            sessionToken
        });

        using var response = await _http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(ct));
        if (!document.RootElement.TryGetProperty("suggestions", out var suggestions))
            return [];

        return suggestions.EnumerateArray()
            .Where(item => item.TryGetProperty("placePrediction", out _))
            .Select(item => item.GetProperty("placePrediction"))
            .Select(prediction => new AddressSuggestionDto(
                prediction.GetProperty("placeId").GetString() ?? string.Empty,
                prediction.GetProperty("text").GetProperty("text").GetString() ?? string.Empty))
            .Where(suggestion =>
                !string.IsNullOrWhiteSpace(suggestion.PlaceId) &&
                !string.IsNullOrWhiteSpace(suggestion.Description))
            .Take(5)
            .ToList();
    }

    public async Task<AddressDetailsDto?> GetDetailsAsync(
        string placeId,
        string sessionToken,
        CancellationToken ct = default)
    {
        EnsureConfigured();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://places.googleapis.com/v1/places/{Uri.EscapeDataString(placeId)}" +
            $"?sessionToken={Uri.EscapeDataString(sessionToken)}");
        request.Headers.Add("X-Goog-Api-Key", _apiKey);
        request.Headers.Add("X-Goog-FieldMask", DetailsFieldMask);

        using var response = await _http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        await EnsureSuccessAsync(response, ct);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(ct));
        var root = document.RootElement;
        var components = ReadComponents(root);
        var streetNumber = GetComponent(components, "street_number");
        var route = GetComponent(components, "route");
        var premise = GetComponent(components, "premise");
        var line1 = JoinNonEmpty(" ", streetNumber, route);
        if (string.IsNullOrWhiteSpace(line1))
            line1 = premise;

        return new AddressDetailsDto(
            root.TryGetProperty("id", out var id) ? id.GetString() ?? placeId : placeId,
            root.TryGetProperty("formattedAddress", out var formatted)
                ? formatted.GetString() ?? string.Empty
                : string.Empty,
            line1,
            GetComponent(components, "subpremise"),
            FirstComponent(components, "locality", "postal_town", "sublocality_level_1"),
            GetComponent(components, "administrative_area_level_1", useShortText: true),
            GetComponent(components, "postal_code"),
            GetComponent(components, "country", useShortText: true));
    }

    private static IReadOnlyList<JsonElement> ReadComponents(JsonElement root) =>
        root.TryGetProperty("addressComponents", out var components)
            ? components.EnumerateArray().ToList()
            : [];

    private static string? FirstComponent(
        IReadOnlyList<JsonElement> components,
        params string[] types)
    {
        foreach (var type in types)
        {
            var value = GetComponent(components, type);
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }
        return null;
    }

    private static string? GetComponent(
        IReadOnlyList<JsonElement> components,
        string type,
        bool useShortText = false)
    {
        foreach (var component in components)
        {
            if (!component.TryGetProperty("types", out var types) ||
                !types.EnumerateArray().Any(value => value.GetString() == type))
                continue;

            var property = useShortText ? "shortText" : "longText";
            if (component.TryGetProperty(property, out var text))
                return text.GetString();
        }
        return null;
    }

    private static string? JoinNonEmpty(string separator, params string?[] values)
    {
        var result = string.Join(separator, values.Where(value => !string.IsNullOrWhiteSpace(value)));
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("Google address lookup is not configured.");
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
            return;

        var providerMessage = await response.Content.ReadAsStringAsync(ct);
        _logger.LogWarning(
            "Google address lookup failed with status {StatusCode}. Response: {ProviderMessage}",
            (int)response.StatusCode,
            providerMessage);
        throw new InvalidOperationException("Address suggestions are temporarily unavailable.");
    }
}
