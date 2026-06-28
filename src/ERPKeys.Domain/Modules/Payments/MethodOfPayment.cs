using ERPKeys.Domain.Common;
using ERPKeys.Domain.Modules.CashBank;
using ERPKeys.Domain.Modules.GeneralLedger;

namespace ERPKeys.Domain.Modules.Payments;

public enum PaymentUsage
{
    Receivable,
    Payable,
    Both
}

public enum PaymentTenderType
{
    Cash,
    Card,
    BankTransfer,
    Check,
    DigitalWallet,
    GiftCard,
    OnAccount,
    Other
}

public enum PaymentSettlementMode
{
    None,
    Immediate,
    Batch,
    Manual
}

public class MethodOfPayment : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public PaymentUsage Usage { get; private set; }
    public PaymentTenderType TenderType { get; private set; }
    public string? CurrencyCode { get; private set; }
    public Guid? ProcessorConfigurationId { get; private set; }
    public Guid? SettlementBankAccountId { get; private set; }
    public Guid? ClearingAccountId { get; private set; }
    public Guid? FeeExpenseAccountId { get; private set; }
    public bool RequiresExternalAuthorization { get; private set; }
    public bool AutoCapture { get; private set; }
    public bool AllowRefunds { get; private set; } = true;
    public bool AllowManualEntry { get; private set; } = true;
    public PaymentSettlementMode SettlementMode { get; private set; }
    public int SettlementDelayDays { get; private set; }
    public TimeOnly? SettlementCutoffTime { get; private set; }
    public bool IsActive { get; private set; } = true;

    public PaymentProcessorConfiguration? ProcessorConfiguration { get; private set; }
    public BankAccount? SettlementBankAccount { get; private set; }
    public Account? ClearingAccount { get; private set; }
    public Account? FeeExpenseAccount { get; private set; }

    private MethodOfPayment() { }

    public MethodOfPayment(
        Guid organizationId,
        string code,
        string name,
        PaymentUsage usage,
        PaymentTenderType tenderType)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        Code = NormalizeRequired(code, "Method code").ToUpperInvariant();
        OrganizationId = organizationId;
        Name = NormalizeRequired(name, "Method name");
        Usage = usage;
        TenderType = tenderType;
    }

    public void Update(
        string name,
        string? description,
        PaymentUsage usage,
        PaymentTenderType tenderType,
        string? currencyCode,
        Guid? processorConfigurationId,
        Guid? settlementBankAccountId,
        Guid? clearingAccountId,
        Guid? feeExpenseAccountId,
        bool requiresExternalAuthorization,
        bool autoCapture,
        bool allowRefunds,
        bool allowManualEntry,
        PaymentSettlementMode settlementMode,
        int settlementDelayDays,
        TimeOnly? settlementCutoffTime)
    {
        if (settlementDelayDays is < 0 or > 30)
            throw new ArgumentException("Settlement delay must be between 0 and 30 days.");
        if (autoCapture && !requiresExternalAuthorization)
            throw new ArgumentException("Auto-capture requires external authorization.");
        if (requiresExternalAuthorization && !processorConfigurationId.HasValue)
            throw new ArgumentException(
                "A processor configuration is required for external authorization.");
        if (settlementMode != PaymentSettlementMode.None &&
            !settlementBankAccountId.HasValue)
            throw new ArgumentException(
                "A settlement bank account is required when settlement is enabled.");
        if (settlementMode == PaymentSettlementMode.Batch &&
            !settlementCutoffTime.HasValue)
            throw new ArgumentException("A cutoff time is required for batch settlement.");

        Name = NormalizeRequired(name, "Method name");
        Description = description?.Trim() ?? string.Empty;
        Usage = usage;
        TenderType = tenderType;
        CurrencyCode = string.IsNullOrWhiteSpace(currencyCode)
            ? null
            : currencyCode.Trim().ToUpperInvariant();
        ProcessorConfigurationId = processorConfigurationId;
        SettlementBankAccountId = settlementBankAccountId;
        ClearingAccountId = clearingAccountId;
        FeeExpenseAccountId = feeExpenseAccountId;
        RequiresExternalAuthorization = requiresExternalAuthorization;
        AutoCapture = autoCapture;
        AllowRefunds = allowRefunds;
        AllowManualEntry = allowManualEntry;
        SettlementMode = settlementMode;
        SettlementDelayDays = settlementDelayDays;
        SettlementCutoffTime = settlementCutoffTime;
        SetUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    private static string NormalizeRequired(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{field} is required.");
        return value.Trim();
    }
}
