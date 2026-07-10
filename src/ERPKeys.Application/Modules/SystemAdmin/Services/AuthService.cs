using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Common.Security;
using ERPKeys.Application.Modules.SystemAdmin.DTOs;
using ERPKeys.Application.Modules.SystemAdmin.Services.ExternalClients;
using ERPKeys.Domain.Modules.SystemAdmin;
using Microsoft.EntityFrameworkCore;

namespace ERPKeys.Application.Modules.SystemAdmin.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IIpAddressClient _ipAddressClient;

    public AuthService(IAppDbContext db, IPasswordHasher hasher, IJwtTokenService jwt, IIpAddressClient ipAddressClient)
    {
        _db     = db;
        _hasher = hasher;
        _jwt    = jwt;
        _ipAddressClient = ipAddressClient;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req, string? ipAddress, CancellationToken ct = default)
    {


        ipAddress = await _ipAddressClient.GetCityAsync(ipAddress);

        var user = await _db.AppUsers
            .FirstOrDefaultAsync(u => u.Username == req.Username.ToLowerInvariant() && !u.IsDeleted, ct)
            ?? throw new InvalidOperationException("Invalid username or password.");

        if (user.IsLockedOut)
            throw new InvalidOperationException("Account is locked. Please try again later or contact an administrator.");

        if (!_hasher.Verify(req.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _db.SaveChangesAsync(ct);
            throw new InvalidOperationException("Invalid username or password.");
        }

        var (roles, storedPermissions) = await LoadAuthorizationAsync(user.Id, ct);
        var permissions = PermissionCatalog.ExpandForRoles(storedPermissions, roles).Order().ToList();

        var accessToken  = _jwt.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _jwt.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, _jwt.RefreshTokenExpiry);
        user.RecordLogin();

        // Write audit log
        _db.AuditLogs.Add(new AuditLogEntry(user.OrganizationId, user.Id, user.Username,
            "Auth", "Login", ipAddress: ipAddress));

        await _db.SaveChangesAsync(ct);

        return new LoginResponse(
            accessToken,
            refreshToken,
            _jwt.AccessTokenExpiry,
            ToUserDto(user, roles, permissions));
    }

    public async Task<LoginResponse> RefreshAsync(RefreshTokenRequest req, string? ipAddress, CancellationToken ct = default)
    {
        var user = await _db.AppUsers
            .FirstOrDefaultAsync(u => u.RefreshToken == req.RefreshToken && !u.IsDeleted, ct)
            ?? throw new InvalidOperationException("Invalid refresh token.");

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new InvalidOperationException("Refresh token has expired. Please log in again.");

        var (roles, storedPermissions) = await LoadAuthorizationAsync(user.Id, ct);
        var permissions = PermissionCatalog.ExpandForRoles(storedPermissions, roles).Order().ToList();

        var accessToken  = _jwt.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _jwt.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, _jwt.RefreshTokenExpiry);
        await _db.SaveChangesAsync(ct);

        return new LoginResponse(
            accessToken,
            refreshToken,
            _jwt.AccessTokenExpiry,
            ToUserDto(user, roles, permissions));
    }

    public async Task LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _db.AppUsers.FindAsync([userId], ct);
        if (user is null) return;
        user.ClearRefreshToken();
        _db.AuditLogs.Add(new AuditLogEntry(user.OrganizationId, user.Id, user.Username, "Auth", "Logout"));
        await _db.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest req, CancellationToken ct = default)
    {
        var user = await _db.AppUsers.FindAsync([userId], ct)
            ?? throw new InvalidOperationException("User not found.");

        if (!_hasher.Verify(req.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect.");

        user.SetPasswordHash(_hasher.Hash(req.NewPassword));
        await _db.SaveChangesAsync(ct);
    }

    public async Task<UserDto> SetPreferredOrganizationAsync(
        Guid userId,
        SetPreferredOrganizationRequest req,
        CancellationToken ct = default)
    {
        var user = await _db.AppUsers.FindAsync([userId], ct)
            ?? throw new InvalidOperationException("User not found.");

        var (roles, storedPermissions) = await LoadAuthorizationAsync(user.Id, ct);
        var hasAllOrganizationAccess = roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);
        if (!hasAllOrganizationAccess && req.OrganizationId != user.OrganizationId)
            throw new InvalidOperationException(
                "You do not have access to the selected organization and cannot make it your default.");

        var organizationExists = await _db.Organizations
            .AnyAsync(o => o.Id == req.OrganizationId &&
                           !o.IsDeleted &&
                           o.Status == ERPKeys.Domain.Modules.Organization.OrganizationStatus.Active, ct);
        if (!organizationExists)
            throw new InvalidOperationException("The selected organization is not active or no longer exists.");

        user.SetPreferredOrganization(req.OrganizationId);
        _db.AuditLogs.Add(new AuditLogEntry(req.OrganizationId, user.Id, user.Username,
            "Auth", "SetPreferredOrganization", req.OrganizationId.ToString(), "Organization"));
        await _db.SaveChangesAsync(ct);

        var permissions = PermissionCatalog.ExpandForRoles(storedPermissions, roles).Order().ToList();

        return ToUserDto(user, roles, permissions);
    }

    public async Task<UserDto> UpdateAppearanceAsync(
        Guid userId,
        UpdateAppearanceRequest req,
        CancellationToken ct = default)
    {
        var user = await _db.AppUsers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.Id == userId && !item.IsDeleted, ct)
            ?? throw new InvalidOperationException("User not found.");
        var oldValues = new
        {
            user.HeaderThemeId,
            user.SidebarThemeId
        };

        user.UpdateAppearance(req.HeaderThemeId, req.SidebarThemeId);
        _db.AuditLogs.Add(new AuditLogEntry(
            user.OrganizationId,
            user.Id,
            user.Username,
            "Auth",
            "UpdateAppearance",
            user.Id.ToString(),
            nameof(AppUser),
            oldValues: System.Text.Json.JsonSerializer.Serialize(oldValues),
            newValues: System.Text.Json.JsonSerializer.Serialize(new
            {
                user.HeaderThemeId,
                user.SidebarThemeId
            })));
        await _db.SaveChangesAsync(ct);

        var (roles, storedPermissions) = await LoadAuthorizationAsync(user.Id, ct);
        var permissions = PermissionCatalog
            .ExpandForRoles(storedPermissions, roles)
            .Order()
            .ToList();
        return ToUserDto(user, roles, permissions);
    }

    private async Task<(List<string> Roles, List<string> Permissions)> LoadAuthorizationAsync(
        Guid userId,
        CancellationToken ct)
    {
        var assignedRoles = await _db.UserRoles
            .AsNoTracking()
            .Where(userRole => userRole.UserId == userId && userRole.Role != null)
            .Select(userRole => new
            {
                userRole.RoleId,
                Name = userRole.Role!.Name
            })
            .ToListAsync(ct);

        var roleIds = assignedRoles.Select(role => role.RoleId).ToList();
        var permissions = roleIds.Count == 0
            ? []
            : await _db.RolePermissions
                .AsNoTracking()
                .Where(permission => roleIds.Contains(permission.RoleId))
                .Select(permission => permission.Module + ":" + permission.Action)
                .Distinct()
                .ToListAsync(ct);

        return (assignedRoles.Select(role => role.Name).Distinct().ToList(), permissions);
    }

    private static UserDto ToUserDto(
        AppUser user,
        IReadOnlyList<string> roles,
        IReadOnlyList<string> permissions)
        => new(
            user.Id,
            user.OrganizationId,
            user.PreferredOrganizationId,
            user.Username,
            user.Email,
            user.FullName,
            user.EmployeeId,
            user.JobTitle,
            user.Department,
            user.Phone,
            user.Timezone,
            user.Locale,
            user.HeaderThemeId,
            user.SidebarThemeId,
            user.AddressLine1,
            user.AddressLine2,
            user.City,
            user.State,
            user.PostalCode,
            user.Country,
            user.Status.ToString(),
            user.LastLoginAt,
            roles,
            permissions,
            user.CreatedAt,
            user.UpdatedAt);
}
