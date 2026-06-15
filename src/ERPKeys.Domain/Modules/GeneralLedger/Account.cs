using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public enum AccountStatus { Active, Inactive, Suspended }

public class Account : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid ChartOfAccountsId { get; private set; }
    public string AccountNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid AccountTypeId { get; private set; }
    public Guid? ParentAccountId { get; private set; }
    public bool IsHeaderAccount { get; private set; }
    public bool AllowManualEntry { get; private set; }
    public AccountStatus Status { get; private set; } = AccountStatus.Active;
    public string Currency { get; private set; } = "USD";
    public int Level { get; private set; } = 1;

    public AccountType? AccountType { get; private set; }
    public Account? ParentAccount { get; private set; }
    public ChartOfAccounts? ChartOfAccounts { get; private set; }

    private Account() { }

    public Account(Guid organizationId, string accountNumber, string name, Guid accountTypeId,
        bool isHeaderAccount, Guid? parentAccountId = null,
        string? description = null, string currency = "USD", int level = 1,
        Guid chartOfAccountsId = default)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        if (chartOfAccountsId == Guid.Empty) throw new ArgumentException("Chart of accounts is required.");
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new ArgumentException("Account number is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Account name is required.");
        if (accountTypeId == Guid.Empty) throw new ArgumentException("Account type is required.");
        if (level < 1) throw new ArgumentException("Account level must be at least one.");

        OrganizationId = organizationId;
        ChartOfAccountsId = chartOfAccountsId;
        AccountNumber = accountNumber.Trim();
        Name = name.Trim();
        AccountTypeId = accountTypeId;
        IsHeaderAccount = isHeaderAccount;
        AllowManualEntry = !isHeaderAccount;
        ParentAccountId = parentAccountId;
        Description = description?.Trim();
        Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();
        Level = level;
    }

    public void Deactivate()
    {
        Status = AccountStatus.Inactive;
        SetUpdated();
    }

    public void Activate()
    {
        Status = AccountStatus.Active;
        SetUpdated();
    }
}
