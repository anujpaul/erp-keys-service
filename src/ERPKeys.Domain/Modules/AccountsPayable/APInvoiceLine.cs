using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.AccountsPayable;

public class APInvoiceLine : BaseEntity
{
    public Guid APInvoiceId { get; private set; }
    public Guid PurchaseOrderLineId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TaxRate { get; private set; }

    public decimal SubTotal => Math.Round(Quantity * UnitCost, 4);
    public decimal TaxAmount => Math.Round(SubTotal * TaxRate / 100m, 4);
    public decimal Total => SubTotal + TaxAmount;

    public APInvoice? APInvoice { get; private set; }
    public PurchaseOrderLine? PurchaseOrderLine { get; private set; }

    private APInvoiceLine() { }

    public APInvoiceLine(
        Guid apInvoiceId,
        Guid purchaseOrderLineId,
        decimal quantity,
        decimal unitCost,
        decimal taxRate)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Invoice line quantity must be positive.");
        if (unitCost < 0)
            throw new InvalidOperationException("Invoice line unit cost cannot be negative.");
        if (taxRate < 0)
            throw new InvalidOperationException("Invoice line tax rate cannot be negative.");

        APInvoiceId = apInvoiceId;
        PurchaseOrderLineId = purchaseOrderLineId;
        Quantity = quantity;
        UnitCost = unitCost;
        TaxRate = taxRate;
    }
}
