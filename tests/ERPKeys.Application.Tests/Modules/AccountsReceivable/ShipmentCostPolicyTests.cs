using ERPKeys.Application.Modules.AccountsReceivable.Services;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.AccountsReceivable;

public class ShipmentCostPolicyTests
{
    [Fact]
    public void Zero_cost_inventory_blocks_shipment()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            ShipmentCostPolicy.EnsureValidCosts(
                [("SKU-001", 0m), ("SKU-002", 15m)]));

        Assert.Contains("SKU-001", error.Message);
        Assert.Contains("valid unit cost", error.Message);
    }

    [Fact]
    public void Positive_inventory_cost_allows_shipment()
    {
        ShipmentCostPolicy.EnsureValidCosts(
            [("SKU-001", 10m), ("SKU-002", 15m)]);
    }
}
