using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ERPKeys.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ERPKeys.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid? OrganizationId
    {
        get
        {
            var value = User?.FindFirstValue("orgId");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string Username =>
        User?.FindFirstValue("username")
        ?? User?.Identity?.Name
        ?? User?.FindFirstValue(ClaimTypes.Email)
        ?? "System";

    public bool IsAdmin =>
        User?.IsInRole("Admin") == true;

    public IReadOnlySet<string> Permissions =>
        User?.FindAll("permission")
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
        ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public string? IpAddress =>
        _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
