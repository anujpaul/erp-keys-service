using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public class JournalLine : BaseEntity
{
    public Guid JournalEntryId { get; private set; }
    public Guid AccountId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public int LineOrder { get; private set; }
    public Guid? FinancialDimensionSetId { get; private set; }

    public JournalEntry? JournalEntry { get; private set; }
    public Account? Account { get; private set; }
    public FinancialDimensionSet? FinancialDimensionSet { get; private set; }

    private readonly List<JournalLineDimensionValue> _dimensionValues = new();
    public IReadOnlyCollection<JournalLineDimensionValue> DimensionValues => _dimensionValues.AsReadOnly();

    private JournalLine() { }

    public JournalLine(Guid journalEntryId, Guid accountId, string description,
        decimal debit, decimal credit, int lineOrder,
        Guid? financialDimensionSetId = null,
        IEnumerable<Guid>? financialDimensionValueIds = null)
    {
        JournalEntryId = journalEntryId;
        AccountId = accountId;
        Description = description;
        Debit = debit;
        Credit = credit;
        LineOrder = lineOrder;
        FinancialDimensionSetId = financialDimensionSetId;

        foreach (var valueId in financialDimensionValueIds?.Distinct() ?? [])
            _dimensionValues.Add(new JournalLineDimensionValue(Id, valueId));
    }
}
