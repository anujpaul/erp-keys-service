using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public class Ledger : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid FunctionalCurrencyId { get; private set; }
    public Guid? ReportingCurrencyId { get; private set; }
    public Guid FiscalCalendarId { get; private set; }
    public Guid ChartOfAccountsId { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Currency? FunctionalCurrency { get; private set; }
    public Currency? ReportingCurrency { get; private set; }
    public FiscalCalendar? FiscalCalendar { get; private set; }
    public ChartOfAccounts? ChartOfAccounts { get; private set; }

    private Ledger() { }

    public Ledger(Guid organizationId, string code, string name,
        Guid functionalCurrencyId, Guid fiscalCalendarId, Guid chartOfAccountsId,
        string? description = null, bool isDefault = false,
        Guid? reportingCurrencyId = null)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Ledger code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Ledger name is required.");
        if (functionalCurrencyId == Guid.Empty) throw new ArgumentException("Functional currency is required.");
        if (fiscalCalendarId == Guid.Empty) throw new ArgumentException("Fiscal calendar is required.");
        if (chartOfAccountsId == Guid.Empty) throw new ArgumentException("Chart of accounts is required.");
        if (reportingCurrencyId == functionalCurrencyId)
            throw new ArgumentException("Reporting currency must differ from functional currency.");

        OrganizationId = organizationId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        FunctionalCurrencyId = functionalCurrencyId;
        ReportingCurrencyId = reportingCurrencyId;
        FiscalCalendarId = fiscalCalendarId;
        ChartOfAccountsId = chartOfAccountsId;
        Description = description?.Trim() ?? string.Empty;
        IsDefault = isDefault;
    }

    public void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
        SetUpdated();
    }

    public void Configure(
        string name,
        Guid functionalCurrencyId,
        Guid fiscalCalendarId,
        Guid chartOfAccountsId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Ledger name is required.");
        if (functionalCurrencyId == Guid.Empty) throw new ArgumentException("Functional currency is required.");
        if (fiscalCalendarId == Guid.Empty) throw new ArgumentException("Fiscal calendar is required.");
        if (chartOfAccountsId == Guid.Empty) throw new ArgumentException("Chart of accounts is required.");
        if (ReportingCurrencyId == functionalCurrencyId)
            throw new ArgumentException("Functional currency must differ from reporting currency.");

        Name = name.Trim();
        FunctionalCurrencyId = functionalCurrencyId;
        FiscalCalendarId = fiscalCalendarId;
        ChartOfAccountsId = chartOfAccountsId;
        SetUpdated();
    }
}
