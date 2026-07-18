using ERPKeys.Domain.Modules.GeneralLedger;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.GeneralLedger;

public class JournalEntrySourceTraceabilityTests
{
    [Fact]
    public void Journal_retains_structured_source_document()
    {
        var sourceId = Guid.NewGuid();
        var journal = new JournalEntry(
            Guid.NewGuid(), "JE-TEST-00001", DateTime.UtcNow.Date,
            Guid.NewGuid(), "Customer invoice", "INV-000001",
            ledgerId: Guid.NewGuid());

        journal.SetSourceDocument(
            "AccountsReceivable", "ARInvoice", sourceId, "INV-000001");

        Assert.Equal("AccountsReceivable", journal.SourceModule);
        Assert.Equal("ARInvoice", journal.SourceDocumentType);
        Assert.Equal(sourceId, journal.SourceDocumentId);
        Assert.Equal("INV-000001", journal.SourceDocumentNumber);
    }

    [Fact]
    public void Posted_journal_source_cannot_be_changed()
    {
        var journal = new JournalEntry(
            Guid.NewGuid(), "JE-TEST-00002", DateTime.UtcNow.Date,
            Guid.NewGuid(), "Posted journal", "TEST",
            ledgerId: Guid.NewGuid());
        journal.AddLine(Guid.NewGuid(), "Debit", 1m, 0m);
        journal.AddLine(Guid.NewGuid(), "Credit", 0m, 1m);
        journal.Post();

        Assert.Throws<InvalidOperationException>(() => journal.SetSourceDocument(
            "AccountsReceivable", "ARInvoice", Guid.NewGuid(), "INV-000002"));
    }
}
