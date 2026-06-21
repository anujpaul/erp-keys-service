using ERPKeys.Application.Modules.SystemAdmin.DTOs;

namespace ERPKeys.Application.Modules.SystemAdmin.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest req, string? ipAddress, CancellationToken ct = default);
    Task<LoginResponse> RefreshAsync(RefreshTokenRequest req, string? ipAddress, CancellationToken ct = default);
    Task LogoutAsync(Guid userId, CancellationToken ct = default);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest req, CancellationToken ct = default);
    Task<UserDto> SetPreferredOrganizationAsync(
        Guid userId,
        SetPreferredOrganizationRequest req,
        CancellationToken ct = default);
}
