using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.AccountsReceivable;

public class AccountsReceivableParameters : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public bool AllowSalesOrderInvoiceVariance { get; private set; }
    public decimal MaximumInvoiceVariancePercent { get; private set; }

    private AccountsReceivableParameters() { }

    public AccountsReceivableParameters(Guid organizationId)
    {
        OrganizationId = organizationId;
    }

    public void UpdateInvoiceVariancePolicy(
        bool allowVariance,
        decimal maximumVariancePercent)
    {
        if (maximumVariancePercent < 0 || maximumVariancePercent > 100)
            throw new InvalidOperationException(
                "Maximum invoice variance percentage must be between 0 and 100.");

        AllowSalesOrderInvoiceVariance = allowVariance;
        MaximumInvoiceVariancePercent =
            allowVariance ? maximumVariancePercent : 0;
        SetUpdated();
    }
}

public static class InvoiceAmountVariancePolicy
{
    public static void EnsureWithinTolerance(
        decimal salesOrderAmount,
        decimal invoiceAmount,
        decimal maximumVariancePercent)
    {
        var roundedSalesOrderAmount = decimal.Round(
            salesOrderAmount, 2, MidpointRounding.AwayFromZero);
        var roundedInvoiceAmount = decimal.Round(
            invoiceAmount, 2, MidpointRounding.AwayFromZero);
        var variance = Math.Abs(
            roundedInvoiceAmount - roundedSalesOrderAmount);
        var allowedVariance = decimal.Round(
            Math.Abs(roundedSalesOrderAmount) * maximumVariancePercent / 100m,
            2,
            MidpointRounding.AwayFromZero);

        if (variance <= allowedVariance)
            return;

        throw new InvalidOperationException(
            $"Invoice total {invoiceAmount:0.00} differs from sales order total " +
            $"{salesOrderAmount:0.00} by {variance:0.00}. The configured maximum " +
            $"variance is {maximumVariancePercent:0.####}% " +
            $"({allowedVariance:0.00}).");
    }
}
