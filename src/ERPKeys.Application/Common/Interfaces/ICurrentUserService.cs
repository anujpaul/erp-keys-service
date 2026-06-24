namespace ERPKeys.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? OrganizationId { get; }
    string Username { get; }
    bool IsAdmin { get; }
    IReadOnlySet<string> Permissions { get; }
    string? IpAddress { get; }
}
