using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public class GeneralJournalVoucherTemplate : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid LedgerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Reference { get; private set; } = string.Empty;
    public string JournalType { get; private set; } = "General";
    public string LinesJson { get; private set; } = "[]";

    public Ledger? Ledger { get; private set; }

    private GeneralJournalVoucherTemplate() { }

    public GeneralJournalVoucherTemplate(
        Guid organizationId,
        Guid userId,
        Guid ledgerId,
        string name,
        string? description,
        string? reference,
        string journalType,
        string linesJson)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        if (userId == Guid.Empty) throw new ArgumentException("User is required.");
        if (ledgerId == Guid.Empty) throw new ArgumentException("Ledger is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Template name is required.");
        if (string.IsNullOrWhiteSpace(journalType)) throw new ArgumentException("Journal type is required.");
        if (string.IsNullOrWhiteSpace(linesJson)) throw new ArgumentException("Template lines are required.");

        OrganizationId = organizationId;
        UserId = userId;
        LedgerId = ledgerId;
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Reference = reference?.Trim() ?? string.Empty;
        JournalType = journalType.Trim();
        LinesJson = linesJson;
    }
}
