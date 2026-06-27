using ERPKeys.Application.Modules.AccountsPayable.Services;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.AccountsPayable;

public class InvoiceQuantityValidatorTests
{
    [Fact]
    public void Validate_ReturnsAvailableQuantity_WhenRequestIsWithinReceivedBalance()
    {
        var available = InvoiceQuantityValidator.Validate(25m, 100m, 40m, "SKU-1");

        Assert.Equal(60m, available);
    }

    [Fact]
    public void Validate_RejectsQuantityAboveUninvoicedReceivedQuantity()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            InvoiceQuantityValidator.Validate(61m, 100m, 40m, "SKU-1"));

        Assert.Contains("cannot exceed", error.Message);
        Assert.Contains("60", error.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_RejectsNonPositiveQuantity(decimal quantity)
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            InvoiceQuantityValidator.Validate(quantity, 100m, 0m, "SKU-1"));

        Assert.Contains("must be positive", error.Message);
    }
}
