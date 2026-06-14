using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public class GeneralLedgerParameters : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid DefaultLedgerId { get; private set; }
    public Guid? DefaultFinancialDimensionSetId { get; private set; }
    public Guid? RetainedEarningsAccountId { get; private set; }
    public Guid? RoundingDifferenceAccountId { get; private set; }
    public Guid? RealizedGainAccountId { get; private set; }
    public Guid? RealizedLossAccountId { get; private set; }
    public Guid? UnrealizedGainAccountId { get; private set; }
    public Guid? UnrealizedLossAccountId { get; private set; }
    public bool AllowPostingToClosedPeriods { get; private set; }
    public bool RequireDimensionsOnJournalLines { get; private set; }
    public decimal MaximumPennyDifference { get; private set; } = 0.01m;
    public string DefaultJournalType { get; private set; } = "General";

    public Ledger? DefaultLedger { get; private set; }
    public FinancialDimensionSet? DefaultFinancialDimensionSet { get; private set; }
    public Account? RetainedEarningsAccount { get; private set; }
    public Account? RoundingDifferenceAccount { get; private set; }
    public Account? RealizedGainAccount { get; private set; }
    public Account? RealizedLossAccount { get; private set; }
    public Account? UnrealizedGainAccount { get; private set; }
    public Account? UnrealizedLossAccount { get; private set; }

    private GeneralLedgerParameters() { }

    public GeneralLedgerParameters(Guid organizationId, Guid defaultLedgerId)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        if (defaultLedgerId == Guid.Empty) throw new ArgumentException("Default ledger is required.");

        OrganizationId = organizationId;
        DefaultLedgerId = defaultLedgerId;
    }

    public void Update(
        Guid defaultLedgerId,
        Guid? defaultFinancialDimensionSetId,
        Guid? retainedEarningsAccountId,
        Guid? roundingDifferenceAccountId,
        Guid? realizedGainAccountId,
        Guid? realizedLossAccountId,
        Guid? unrealizedGainAccountId,
        Guid? unrealizedLossAccountId,
        bool allowPostingToClosedPeriods,
        bool requireDimensionsOnJournalLines,
        decimal maximumPennyDifference,
        string defaultJournalType)
    {
        if (defaultLedgerId == Guid.Empty) throw new ArgumentException("Default ledger is required.");
        if (maximumPennyDifference < 0 || maximumPennyDifference > 1)
            throw new ArgumentException("Maximum penny difference must be between 0 and 1.");
        if (string.IsNullOrWhiteSpace(defaultJournalType))
            throw new ArgumentException("Default journal type is required.");

        DefaultLedgerId = defaultLedgerId;
        DefaultFinancialDimensionSetId = defaultFinancialDimensionSetId;
        RetainedEarningsAccountId = retainedEarningsAccountId;
        RoundingDifferenceAccountId = roundingDifferenceAccountId;
        RealizedGainAccountId = realizedGainAccountId;
        RealizedLossAccountId = realizedLossAccountId;
        UnrealizedGainAccountId = unrealizedGainAccountId;
        UnrealizedLossAccountId = unrealizedLossAccountId;
        AllowPostingToClosedPeriods = allowPostingToClosedPeriods;
        RequireDimensionsOnJournalLines = requireDimensionsOnJournalLines;
        MaximumPennyDifference = maximumPennyDifference;
        DefaultJournalType = defaultJournalType.Trim();
        SetUpdated();
    }
}
