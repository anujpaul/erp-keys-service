using ERPKeys.Domain.Modules.AccountsReceivable;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.AccountsReceivable;

public class SalesQuotationLifecycleTests
{
    [Fact]
    public void Workflow_approval_makes_quotation_ready_to_send()
    {
        var quotation = CreateQuotation();
        quotation.SubmitForApproval(Guid.NewGuid());

        quotation.WorkflowApproved();

        Assert.Equal(QuotationStatus.Approved, quotation.Status);
        Assert.Throws<InvalidOperationException>(quotation.Accept);

        quotation.Send();

        Assert.Equal(QuotationStatus.Sent, quotation.Status);
    }

    [Fact]
    public void Draft_quotation_can_still_be_sent_without_approval()
    {
        var quotation = CreateQuotation();

        quotation.Send();

        Assert.Equal(QuotationStatus.Sent, quotation.Status);
    }

    private static SalesQuotation CreateQuotation()
    {
        var quotation = new SalesQuotation(
            Guid.NewGuid(),
            "QUO-TEST-00001",
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30),
            "Quotation lifecycle test");
        quotation.AddLine(
            Guid.NewGuid(), "SKU-001", "Test product", null,
            "Each", 1m, 100m, 0m);
        return quotation;
    }
}
