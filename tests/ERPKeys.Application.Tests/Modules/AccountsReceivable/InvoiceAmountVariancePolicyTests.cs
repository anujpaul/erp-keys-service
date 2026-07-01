using ERPKeys.Domain.Modules.AccountsReceivable;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.AccountsReceivable;

public class InvoiceAmountVariancePolicyTests
{
    [Fact]
    public void Parameters_normalize_disabled_variance_to_zero()
    {
        var parameters = new AccountsReceivableParameters(Guid.NewGuid());
        parameters.UpdateInvoiceVariancePolicy(true, 2.5m);

        Assert.True(parameters.AllowSalesOrderInvoiceVariance);
        Assert.Equal(2.5m, parameters.MaximumInvoiceVariancePercent);

        parameters.UpdateInvoiceVariancePolicy(false, 2.5m);

        Assert.False(parameters.AllowSalesOrderInvoiceVariance);
        Assert.Equal(0m, parameters.MaximumInvoiceVariancePercent);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    public void Parameters_reject_invalid_variance_percentages(
        decimal percentage)
    {
        var parameters = new AccountsReceivableParameters(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() =>
            parameters.UpdateInvoiceVariancePolicy(true, percentage));
    }

    [Theory]
    [InlineData(990)]
    [InlineData(1010)]
    public void Policy_allows_invoice_amounts_within_symmetric_tolerance(
        decimal invoiceAmount)
    {
        InvoiceAmountVariancePolicy.EnsureWithinTolerance(
            salesOrderAmount: 1000m,
            invoiceAmount,
            maximumVariancePercent: 1m);
    }

    [Theory]
    [InlineData(989.99)]
    [InlineData(1010.01)]
    public void Policy_rejects_invoice_amounts_outside_tolerance(
        decimal invoiceAmount)
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            InvoiceAmountVariancePolicy.EnsureWithinTolerance(
                salesOrderAmount: 1000m,
                invoiceAmount,
                maximumVariancePercent: 1m));

        Assert.Contains("configured maximum variance", exception.Message);
    }
}
