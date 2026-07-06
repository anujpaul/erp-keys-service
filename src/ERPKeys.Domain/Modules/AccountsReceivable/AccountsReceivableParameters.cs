using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.AccountsReceivable;

public class AccountsReceivableParameters : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public bool AllowSalesOrderInvoiceVariance { get; private set; }
    public decimal MaximumInvoiceVariancePercent { get; private set; }
    public Guid? TradeReceivableAccountId { get; private set; }
    public Guid? SalesRevenueAccountId { get; private set; }
    public Guid? SalesTaxPayableAccountId { get; private set; }
    public Guid? CashAccountId { get; private set; }
    public Guid? BankAccountId { get; private set; }
    public Guid? CostOfGoodsSoldAccountId { get; private set; }
    public Guid? InventoryAccountId { get; private set; }

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

    public void UpdatePostingAccounts(
        Guid tradeReceivableAccountId,
        Guid salesRevenueAccountId,
        Guid salesTaxPayableAccountId,
        Guid cashAccountId,
        Guid bankAccountId,
        Guid costOfGoodsSoldAccountId,
        Guid inventoryAccountId)
    {
        var accountIds = new[]
        {
            tradeReceivableAccountId,
            salesRevenueAccountId,
            salesTaxPayableAccountId,
            cashAccountId,
            bankAccountId,
            costOfGoodsSoldAccountId,
            inventoryAccountId
        };
        if (accountIds.Any(id => id == Guid.Empty))
            throw new InvalidOperationException(
                "All Accounts Receivable posting accounts must be configured.");

        TradeReceivableAccountId = tradeReceivableAccountId;
        SalesRevenueAccountId = salesRevenueAccountId;
        SalesTaxPayableAccountId = salesTaxPayableAccountId;
        CashAccountId = cashAccountId;
        BankAccountId = bankAccountId;
        CostOfGoodsSoldAccountId = costOfGoodsSoldAccountId;
        InventoryAccountId = inventoryAccountId;
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
