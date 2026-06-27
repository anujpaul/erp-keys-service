namespace ERPKeys.Application.Modules.AccountsPayable.Services;

public static class InvoiceQuantityValidator
{
    public static decimal Validate(
        decimal requestedQuantity,
        decimal receivedQuantity,
        decimal alreadyInvoicedQuantity,
        string productCode)
    {
        if (requestedQuantity <= 0)
            throw new InvalidOperationException(
                $"Invoice quantity for {productCode} must be positive.");

        var availableQuantity = Math.Max(0, receivedQuantity - alreadyInvoicedQuantity);
        if (requestedQuantity > availableQuantity)
            throw new InvalidOperationException(
                $"Invoice quantity for {productCode} cannot exceed the available received quantity of {availableQuantity:0.####}.");

        return availableQuantity;
    }
}
