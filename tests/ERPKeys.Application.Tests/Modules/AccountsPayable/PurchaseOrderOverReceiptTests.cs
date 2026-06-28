using ERPKeys.Domain.Modules.AccountsPayable;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.AccountsPayable;

public class PurchaseOrderOverReceiptTests
{
    [Fact]
    public void Receive_AllowsQuantityAtConfiguredOverReceiptLimit()
    {
        var line = CreateLine(100m);

        line.Receive(105m, 5m);

        Assert.Equal(105m, line.ReceivedQty);
    }

    [Fact]
    public void Receive_RejectsQuantityAboveConfiguredOverReceiptLimit()
    {
        var line = CreateLine(100m);

        var error = Assert.Throws<InvalidOperationException>(() =>
            line.Receive(105.01m, 5m));

        Assert.Contains("Cannot receive more than 105", error.Message);
    }

    [Fact]
    public void Receive_DisallowsOverReceiptWhenToleranceIsZero()
    {
        var line = CreateLine(100m);

        Assert.Throws<InvalidOperationException>(() => line.Receive(100.01m));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    public void Parameters_RejectInvalidPercentage(decimal percentage)
    {
        var parameters = new AccountsPayableParameters(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() =>
            parameters.UpdateOverReceiptPolicy(true, percentage));
    }

    private static PurchaseOrderLine CreateLine(decimal orderedQuantity) =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "SKU-1",
            "Test item",
            "Each",
            orderedQuantity,
            10m);
}
