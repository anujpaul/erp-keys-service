namespace ERPKeys.Application.Modules.AccountsReceivable.Services;

public static class ShipmentCostPolicy
{
    public static void EnsureValidCosts(
        IEnumerable<(string Sku, decimal AverageCost)> shipmentItems)
    {
        var zeroCostSkus = shipmentItems
            .Where(item => item.AverageCost <= 0m)
            .Select(item => item.Sku)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (zeroCostSkus.Count == 0)
            return;

        throw new InvalidOperationException(
            "Shipment cannot be posted because inventory cost is missing or zero for SKU(s): " +
            $"{string.Join(", ", zeroCostSkus)}. Receive or adjust the inventory with a valid unit cost, " +
            "then try the shipment again.");
    }
}
