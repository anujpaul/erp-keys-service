using ERPKeys.Domain.Modules.AccountsPayable;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.AccountsPayable;

public class AccountsPayablePostingLifecycleTests
{
    [Fact]
    public void ApprovedInvoice_RetainsPostedJournalReference()
    {
        var invoice = CreateInvoice();
        var journalEntryId = Guid.NewGuid();

        invoice.Approve();
        invoice.SetJournalEntry(journalEntryId);

        Assert.Equal(APInvoiceStatus.Approved, invoice.Status);
        Assert.Equal(journalEntryId, invoice.JournalEntryId);
    }

    [Fact]
    public void PostedPayment_RetainsJournalReference()
    {
        var journalEntryId = Guid.NewGuid();
        var payment = new APPayment(
            Guid.NewGuid(),
            "PAY-000001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            125m,
            "BankTransfer",
            "BANK-001");

        payment.Post(journalEntryId);

        Assert.Equal(APPaymentStatus.Posted, payment.Status);
        Assert.Equal(journalEntryId, payment.JournalEntryId);
    }

    [Fact]
    public void PostedPayment_CannotBePostedTwice()
    {
        var payment = new APPayment(
            Guid.NewGuid(),
            "PAY-000001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            125m,
            "BankTransfer");
        payment.Post(Guid.NewGuid());

        var error = Assert.Throws<InvalidOperationException>(() =>
            payment.Post(Guid.NewGuid()));

        Assert.Contains("draft payment", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static APInvoice CreateInvoice() =>
        new(
            Guid.NewGuid(),
            "APINV-000001",
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30),
            "Vendor invoice",
            "VENDOR-001",
            100m,
            25m);
}
