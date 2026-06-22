using ERPKeys.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ERPKeys.Api.Controllers;

[ApiController]
[Route("api/address-lookup")]
[Authorize]
[EnableRateLimiting("address-lookup")]
public class AddressLookupController : ControllerBase
{
    private readonly IAddressLookupService _addressLookup;

    public AddressLookupController(IAddressLookupService addressLookup) =>
        _addressLookup = addressLookup;

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions(
        [FromQuery] string input,
        [FromQuery] string sessionToken,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Trim().Length < 4)
            return Ok(Array.Empty<AddressSuggestionDto>());
        if (!IsValidSessionToken(sessionToken))
            return BadRequest(new { error = "A valid address lookup session token is required." });

        try
        {
            return Ok(await _addressLookup.GetSuggestionsAsync(input, sessionToken, ct));
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
        }
    }

    [HttpGet("details/{placeId}")]
    public async Task<IActionResult> GetDetails(
        string placeId,
        [FromQuery] string sessionToken,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(placeId))
            return BadRequest(new { error = "A place identifier is required." });
        if (!IsValidSessionToken(sessionToken))
            return BadRequest(new { error = "A valid address lookup session token is required." });

        try
        {
            var details = await _addressLookup.GetDetailsAsync(placeId, sessionToken, ct);
            return details is null ? NotFound() : Ok(details);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
        }
    }

    private static bool IsValidSessionToken(string? sessionToken) =>
        Guid.TryParse(sessionToken, out _);
}
