using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.SystemAdmin;

public enum UserStatus { Active, Inactive, Locked }

public class AppUser : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid? PreferredOrganizationId { get; private set; }
    public string Username     { get; private set; } = string.Empty;
    public string Email        { get; private set; } = string.Empty;
    public string FullName     { get; private set; } = string.Empty;
    public string? EmployeeId  { get; private set; }
    public string? JobTitle    { get; private set; }
    public string? Department  { get; private set; }
    public string? Phone       { get; private set; }
    public string? Timezone    { get; private set; }
    public string? Locale      { get; private set; }
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? City         { get; private set; }
    public string? State        { get; private set; }
    public string? PostalCode   { get; private set; }
    public string? Country      { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public UserStatus Status   { get; private set; } = UserStatus.Active;
    public DateTime? LastLoginAt { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public string? RefreshToken          { get; private set; }
    public DateTime? RefreshTokenExpiry  { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private AppUser() { }

    public AppUser(Guid organizationId, string username, string email, string fullName, string passwordHash)
    {
        OrganizationId = organizationId;
        Username       = username.ToLowerInvariant().Trim();
        Email          = email.ToLowerInvariant().Trim();
        FullName       = fullName.Trim();
        PasswordHash   = passwordHash;
    }

    public void UpdateProfile(
        string email,
        string fullName,
        string? employeeId,
        string? jobTitle,
        string? department,
        string? phone,
        string? timezone,
        string? locale,
        string? addressLine1 = null,
        string? addressLine2 = null,
        string? city = null,
        string? state = null,
        string? postalCode = null,
        string? country = null)
    {
        Email    = email.ToLowerInvariant().Trim();
        FullName = fullName.Trim();
        EmployeeId = Normalize(employeeId);
        JobTitle = Normalize(jobTitle);
        Department = Normalize(department);
        Phone = Normalize(phone);
        Timezone = Normalize(timezone);
        Locale = Normalize(locale);
        AddressLine1 = Normalize(addressLine1);
        AddressLine2 = Normalize(addressLine2);
        City = Normalize(city);
        State = Normalize(state);
        PostalCode = Normalize(postalCode);
        Country = Normalize(country)?.ToUpperInvariant();
        SetUpdated();
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public void SetPasswordHash(string hash) { PasswordHash = hash; SetUpdated(); }

    public void SetPreferredOrganization(Guid organizationId)
    {
        PreferredOrganizationId = organizationId;
        SetUpdated();
    }

    public void RecordLogin()
    {
        LastLoginAt        = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil        = null;
        SetUpdated();
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            Status      = UserStatus.Locked;
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        }
        SetUpdated();
    }

    public void Unlock()   { Status = UserStatus.Active; FailedLoginAttempts = 0; LockedUntil = null; SetUpdated(); }
    public void Deactivate() { Status = UserStatus.Inactive; SetUpdated(); }
    public void Activate()   { Unlock(); }

    public bool IsLockedOut => Status == UserStatus.Locked ||
                               (LockedUntil.HasValue && LockedUntil > DateTime.UtcNow);

    public void SetRefreshToken(string token, DateTime expiry)
    {
        RefreshToken       = token;
        RefreshTokenExpiry = expiry;
        SetUpdated();
    }

    public void ClearRefreshToken() { RefreshToken = null; RefreshTokenExpiry = null; SetUpdated(); }

    public void AssignRole(Guid roleId)
    {
        if (_userRoles.Any(r => r.RoleId == roleId)) return;
        _userRoles.Add(new UserRole(Id, roleId));
        SetUpdated();
    }

    public void RemoveRole(Guid roleId)
    {
        var ur = _userRoles.FirstOrDefault(r => r.RoleId == roleId);
        if (ur != null) { _userRoles.Remove(ur); SetUpdated(); }
    }
}
