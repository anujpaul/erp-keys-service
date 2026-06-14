using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public enum AccrualAllocationMethod { Even, Custom }

public class AccrualScheme : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid LedgerId { get; private set; }
    public Guid DebitAccountId { get; private set; }
    public Guid CreditAccountId { get; private set; }
    public string JournalType { get; private set; } = "Accrual";
    public AccrualAllocationMethod AllocationMethod { get; private set; }
    public int DefaultPeriodCount { get; private set; }
    public Guid? FinancialDimensionSetId { get; private set; }
    public string FinancialDimensionValueIdsJson { get; private set; } = "[]";
    public bool IsActive { get; private set; } = true;

    public Ledger? Ledger { get; private set; }
    public Account? DebitAccount { get; private set; }
    public Account? CreditAccount { get; private set; }
    public FinancialDimensionSet? FinancialDimensionSet { get; private set; }

    private readonly List<AccrualSchemeAllocation> _allocations = [];
    public IReadOnlyCollection<AccrualSchemeAllocation> Allocations => _allocations.AsReadOnly();

    private AccrualScheme() { }

    public AccrualScheme(
        Guid organizationId,
        string code,
        string name,
        string? description,
        Guid ledgerId,
        Guid debitAccountId,
        Guid creditAccountId,
        string journalType,
        AccrualAllocationMethod allocationMethod,
        int defaultPeriodCount,
        Guid? financialDimensionSetId,
        string financialDimensionValueIdsJson)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Scheme code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Scheme name is required.");
        if (ledgerId == Guid.Empty) throw new ArgumentException("Ledger is required.");
        if (debitAccountId == Guid.Empty || creditAccountId == Guid.Empty)
            throw new ArgumentException("Debit and credit accounts are required.");
        if (debitAccountId == creditAccountId)
            throw new ArgumentException("Debit and credit accounts must differ.");
        if (defaultPeriodCount < 1 || defaultPeriodCount > 120)
            throw new ArgumentException("Default period count must be between 1 and 120.");

        OrganizationId = organizationId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        LedgerId = ledgerId;
        DebitAccountId = debitAccountId;
        CreditAccountId = creditAccountId;
        JournalType = string.IsNullOrWhiteSpace(journalType) ? "Accrual" : journalType.Trim();
        AllocationMethod = allocationMethod;
        DefaultPeriodCount = defaultPeriodCount;
        FinancialDimensionSetId = financialDimensionSetId;
        FinancialDimensionValueIdsJson = financialDimensionValueIdsJson;
    }

    public void SetAllocations(IEnumerable<decimal> percentages)
    {
        _allocations.Clear();
        var values = percentages.ToList();
        if (AllocationMethod == AccrualAllocationMethod.Even)
        {
            if (values.Count > 0)
                throw new InvalidOperationException("Even accrual schemes cannot define custom percentages.");
            return;
        }

        if (values.Count != DefaultPeriodCount)
            throw new InvalidOperationException(
                "Custom accrual schemes require one percentage for each period.");
        if (values.Any(value => value <= 0 || value > 100))
            throw new InvalidOperationException("Allocation percentages must be greater than 0 and at most 100.");
        if (Math.Abs(values.Sum() - 100m) > 0.0001m)
            throw new InvalidOperationException("Allocation percentages must total 100.");

        for (var index = 0; index < values.Count; index++)
            _allocations.Add(new AccrualSchemeAllocation(Id, index, values[index]));
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }
}

public class AccrualSchemeAllocation : BaseEntity
{
    public Guid AccrualSchemeId { get; private set; }
    public int PeriodOffset { get; private set; }
    public decimal Percentage { get; private set; }
    public AccrualScheme? AccrualScheme { get; private set; }

    private AccrualSchemeAllocation() { }

    public AccrualSchemeAllocation(Guid accrualSchemeId, int periodOffset, decimal percentage)
    {
        AccrualSchemeId = accrualSchemeId;
        PeriodOffset = periodOffset;
        Percentage = percentage;
    }
}

public class AccrualPostingRun : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid AccrualSchemeId { get; private set; }
    public Guid? PostedByUserId { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid StartFiscalPeriodId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime PostedAt { get; private set; }

    public AccrualScheme? AccrualScheme { get; private set; }
    private readonly List<AccrualPostingLine> _lines = [];
    public IReadOnlyCollection<AccrualPostingLine> Lines => _lines.AsReadOnly();

    private AccrualPostingRun() { }

    public AccrualPostingRun(
        Guid organizationId,
        Guid accrualSchemeId,
        Guid? postedByUserId,
        string reference,
        string description,
        Guid startFiscalPeriodId,
        decimal totalAmount)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Posting reference is required.");
        if (totalAmount <= 0)
            throw new ArgumentException("Accrual amount must be greater than zero.");

        OrganizationId = organizationId;
        AccrualSchemeId = accrualSchemeId;
        PostedByUserId = postedByUserId;
        Reference = reference.Trim();
        Description = description?.Trim() ?? string.Empty;
        StartFiscalPeriodId = startFiscalPeriodId;
        TotalAmount = totalAmount;
        PostedAt = DateTime.UtcNow;
    }

    public void AddLine(
        Guid fiscalPeriodId,
        Guid journalEntryId,
        int periodOffset,
        decimal percentage,
        decimal amount)
    {
        _lines.Add(new AccrualPostingLine(
            Id, fiscalPeriodId, journalEntryId, periodOffset, percentage, amount));
    }
}

public class AccrualPostingLine : BaseEntity
{
    public Guid AccrualPostingRunId { get; private set; }
    public Guid FiscalPeriodId { get; private set; }
    public Guid JournalEntryId { get; private set; }
    public int PeriodOffset { get; private set; }
    public decimal Percentage { get; private set; }
    public decimal Amount { get; private set; }

    public AccrualPostingRun? AccrualPostingRun { get; private set; }
    public FiscalPeriod? FiscalPeriod { get; private set; }
    public JournalEntry? JournalEntry { get; private set; }

    private AccrualPostingLine() { }

    public AccrualPostingLine(
        Guid accrualPostingRunId,
        Guid fiscalPeriodId,
        Guid journalEntryId,
        int periodOffset,
        decimal percentage,
        decimal amount)
    {
        AccrualPostingRunId = accrualPostingRunId;
        FiscalPeriodId = fiscalPeriodId;
        JournalEntryId = journalEntryId;
        PeriodOffset = periodOffset;
        Percentage = percentage;
        Amount = amount;
    }
}
