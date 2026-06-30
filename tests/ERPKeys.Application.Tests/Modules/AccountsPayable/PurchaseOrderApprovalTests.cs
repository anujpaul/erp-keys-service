using ERPKeys.Domain.Modules.AccountsPayable;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.AccountsPayable;

public class PurchaseOrderApprovalTests
{
    [Fact]
    public void Purchase_order_cannot_be_sent_before_approval()
    {
        var purchaseOrder = CreatePurchaseOrder();

        var exception = Assert.Throws<InvalidOperationException>(
            purchaseOrder.Send);

        Assert.Contains("approved", exception.Message);
        Assert.Equal(PurchaseOrderStatus.Draft, purchaseOrder.Status);
    }

    [Fact]
    public void Approved_purchase_order_can_be_sent_and_then_received()
    {
        var purchaseOrder = CreatePurchaseOrder();
        var workflowId = Guid.NewGuid();

        purchaseOrder.SubmitForApproval(workflowId);

        Assert.Equal(PurchaseOrderStatus.PendingApproval, purchaseOrder.Status);
        Assert.Equal(workflowId, purchaseOrder.WorkflowInstanceId);
        Assert.False(purchaseOrder.CanReceive);
        Assert.Throws<InvalidOperationException>(purchaseOrder.Send);
        Assert.Throws<InvalidOperationException>(purchaseOrder.Cancel);

        purchaseOrder.WorkflowApproved();

        Assert.Equal(PurchaseOrderStatus.Approved, purchaseOrder.Status);
        Assert.False(purchaseOrder.CanReceive);

        purchaseOrder.Send();

        Assert.Equal(PurchaseOrderStatus.Sent, purchaseOrder.Status);
        Assert.True(purchaseOrder.CanReceive);
    }

    [Fact]
    public void Rejected_purchase_order_returns_to_draft_for_correction()
    {
        var purchaseOrder = CreatePurchaseOrder();
        purchaseOrder.SubmitForApproval(Guid.NewGuid());

        purchaseOrder.WorkflowRejected("Budget owner declined the request.");

        Assert.Equal(PurchaseOrderStatus.Draft, purchaseOrder.Status);
        Assert.Equal(
            "Budget owner declined the request.",
            purchaseOrder.RejectionReason);
    }

    private static PurchaseOrder CreatePurchaseOrder()
    {
        var purchaseOrder = new PurchaseOrder(
            Guid.NewGuid(),
            "PO-TEST-00001",
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            "Approval lifecycle test");
        purchaseOrder.AddLine(
            Guid.NewGuid(),
            "1000001",
            "Test product",
            "Each",
            10m,
            12m);
        return purchaseOrder;
    }
}
