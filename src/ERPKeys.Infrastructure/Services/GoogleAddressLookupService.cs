using System.Net.Http.Json;
using System.Text.Json;
using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Modules.SystemAdmin.Services;
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
    private readonly IIntegrationConfigurationReader _integrations;
    private readonly string _fallbackApiKey;
    private readonly string _fallbackBaseUrl;
    private readonly ILogger<GoogleAddressLookupService> _logger;

    public GoogleAddressLookupService(
        HttpClient http,
        IConfiguration configuration,
        IIntegrationConfigurationReader integrations,
        ILogger<GoogleAddressLookupService> logger)
    {
        _http = http;
        _integrations = integrations;
        _fallbackApiKey = configuration["GoogleMap:key"]?.Trim() ?? string.Empty;
        _fallbackBaseUrl = configuration["GoogleMap:BaseUrl"]?.Trim()
            ?? "https://places.googleapis.com/v1";
        _logger = logger;
    }

    public async Task<IReadOnlyList<AddressSuggestionDto>> GetSuggestionsAsync(
        string input,
        string sessionToken,
        CancellationToken ct = default)
    {
        var (apiKey, baseUrl) = await GetConfigurationAsync(ct);

        var normalizedInput = input.Trim();
        if (normalizedInput.Length < 4)
            return [];

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{baseUrl}/places:autocomplete");
        request.Headers.Add("X-Goog-Api-Key", apiKey);
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
        var (apiKey, baseUrl) = await GetConfigurationAsync(ct);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl}/places/{Uri.EscapeDataString(placeId)}" +
            $"?sessionToken={Uri.EscapeDataString(sessionToken)}");
        request.Headers.Add("X-Goog-Api-Key", apiKey);
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

    private async Task<(string ApiKey, string BaseUrl)> GetConfigurationAsync(
        CancellationToken ct)
    {
        var configured = await _integrations.GetActiveAsync(
            "AddressValidation", "GooglePlaces", ct);
        var apiKey = configured?.Secrets.GetValueOrDefault("ApiKey") ?? _fallbackApiKey;
        var baseUrl = configured?.Settings.GetValueOrDefault("BaseUrl") ?? _fallbackBaseUrl;
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Google address lookup is not configured.");
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var parsed) ||
            parsed.Scheme != Uri.UriSchemeHttps)
            throw new InvalidOperationException("Google address lookup URL must use HTTPS.");
        return (apiKey, baseUrl.TrimEnd('/'));
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
