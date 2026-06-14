using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public class ChartOfAccounts : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Account> _accounts = [];
    public IReadOnlyCollection<Account> Accounts => _accounts.AsReadOnly();

    private ChartOfAccounts() { }

    public ChartOfAccounts(Guid organizationId, string code, string name,
        string? description = null, bool isDefault = false)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Chart code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Chart name is required.");

        OrganizationId = organizationId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        IsDefault = isDefault;
    }

    public void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
        SetUpdated();
    }
}
