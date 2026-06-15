using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public enum JournalEntryStatus { Draft, Posted, Voided }

public class JournalEntry : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid LedgerId { get; private set; }
    public string EntryNumber { get; private set; } = string.Empty;
    public DateTime EntryDate { get; private set; }
    public Guid FiscalPeriodId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Reference { get; private set; } = string.Empty;
    public string JournalType { get; private set; } = "General";
    public JournalEntryStatus Status { get; private set; } = JournalEntryStatus.Draft;
    public string Currency { get; private set; } = "USD";
    public decimal TotalDebit { get; private set; }
    public decimal TotalCredit { get; private set; }

    public FiscalPeriod? FiscalPeriod { get; private set; }
    public Ledger? Ledger { get; private set; }

    private readonly List<JournalLine> _lines = new();
    public IReadOnlyCollection<JournalLine> Lines => _lines.AsReadOnly();

    private JournalEntry() { }

    public JournalEntry(Guid organizationId, string entryNumber, DateTime entryDate, Guid fiscalPeriodId,
        string description, string reference, string journalType = "General", string currency = "USD",
        Guid ledgerId = default)
    {
        OrganizationId = organizationId;
        LedgerId = ledgerId;
        EntryNumber = entryNumber;
        EntryDate = entryDate;
        FiscalPeriodId = fiscalPeriodId;
        Description = description;
        Reference = reference;
        JournalType = journalType;
        Currency = currency;
    }

    public void AddLine(
        Guid accountId,
        string description,
        decimal debit,
        decimal credit,
        Guid? financialDimensionSetId = null,
        IEnumerable<Guid>? financialDimensionValueIds = null)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Can only add lines to a draft journal entry.");
        if (debit < 0 || credit < 0)
            throw new InvalidOperationException("Debit and credit amounts must be non-negative.");
        if (debit > 0 && credit > 0)
            throw new InvalidOperationException("A line cannot have both debit and credit amounts.");

        _lines.Add(new JournalLine(
            Id, accountId, description, debit, credit,
            _lines.Count(l => !l.IsDeleted) + 1,
            financialDimensionSetId, financialDimensionValueIds));
        Recalc();
        SetUpdated();
    }

    public void UpdateDraft(
        Guid ledgerId,
        DateTime entryDate,
        Guid fiscalPeriodId,
        string description,
        string reference,
        string journalType,
        string currency)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Only a draft journal entry can be edited.");
        if (ledgerId == Guid.Empty) throw new ArgumentException("Ledger is required.");
        if (fiscalPeriodId == Guid.Empty) throw new ArgumentException("Fiscal period is required.");

        LedgerId = ledgerId;
        EntryDate = entryDate;
        FiscalPeriodId = fiscalPeriodId;
        Description = description?.Trim() ?? string.Empty;
        Reference = reference?.Trim() ?? string.Empty;
        JournalType = string.IsNullOrWhiteSpace(journalType) ? "General" : journalType.Trim();
        Currency = currency.Trim().ToUpperInvariant();
        foreach (var line in _lines.Where(l => !l.IsDeleted))
        {
            foreach (var dimensionValue in line.DimensionValues.Where(v => !v.IsDeleted))
                dimensionValue.SoftDelete();
            line.SoftDelete();
        }
        Recalc();
        SetUpdated();
    }

    public void Post()
    {
        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Only a draft entry can be posted.");
        if (_lines.Count(l => !l.IsDeleted) < 2)
            throw new InvalidOperationException("A journal entry must have at least two lines.");
        if (TotalDebit != TotalCredit)
            throw new InvalidOperationException($"Debits ({TotalDebit}) must equal credits ({TotalCredit}).");

        Status = JournalEntryStatus.Posted;
        SetUpdated();
    }

    public void Void()
    {
        if (Status != JournalEntryStatus.Posted)
            throw new InvalidOperationException("Only a posted entry can be voided.");
        Status = JournalEntryStatus.Voided;
        SetUpdated();
    }

    private void Recalc()
    {
        TotalDebit = _lines.Where(l => !l.IsDeleted).Sum(l => l.Debit);
        TotalCredit = _lines.Where(l => !l.IsDeleted).Sum(l => l.Credit);
    }
}
