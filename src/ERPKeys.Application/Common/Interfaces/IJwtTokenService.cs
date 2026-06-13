using ERPKeys.Domain.Modules.SystemAdmin;

namespace ERPKeys.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(AppUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string GenerateRefreshToken();
    DateTime AccessTokenExpiry { get; }
    DateTime RefreshTokenExpiry { get; }
}
