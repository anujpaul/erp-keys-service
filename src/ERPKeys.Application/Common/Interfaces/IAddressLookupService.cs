namespace ERPKeys.Application.Common.Interfaces;

public record AddressSuggestionDto(string PlaceId, string Description);

public record AddressDetailsDto(
    string PlaceId,
    string FormattedAddress,
    string? Line1,
    string? Line2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country);

public interface IAddressLookupService
{
    Task<IReadOnlyList<AddressSuggestionDto>> GetSuggestionsAsync(
        string input,
        string sessionToken,
        CancellationToken ct = default);

    Task<AddressDetailsDto?> GetDetailsAsync(
        string placeId,
        string sessionToken,
        CancellationToken ct = default);
}
