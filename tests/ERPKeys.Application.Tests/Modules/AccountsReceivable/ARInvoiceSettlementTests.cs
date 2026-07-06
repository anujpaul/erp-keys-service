using ERPKeys.Domain.Modules.AccountsReceivable;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.AccountsReceivable;

public class ARInvoiceSettlementTests
{
    [Fact]
    public void Void_rejects_partially_settled_invoice()
    {
        var invoice = CreateInvoice();
        invoice.Issue();
        invoice.ApplyPayment(25m);

        var exception = Assert.Throws<InvalidOperationException>(invoice.Void);

        Assert.Contains("Reverse the settlements first", exception.Message);
        Assert.Equal(ARInvoiceStatus.PartiallyPaid, invoice.Status);
        Assert.Equal(25m, invoice.PaidAmount);
    }

    [Fact]
    public void Void_allows_unsettled_issued_invoice()
    {
        var invoice = CreateInvoice();
        invoice.Issue();

        invoice.Void();

        Assert.Equal(ARInvoiceStatus.Voided, invoice.Status);
    }

    [Fact]
    public void Posting_parameters_store_configured_account_ids()
    {
        var accountIds = Enumerable.Range(0, 7)
            .Select(_ => Guid.NewGuid())
            .ToArray();
        var parameters = new AccountsReceivableParameters(Guid.NewGuid());

        parameters.UpdatePostingAccounts(
            accountIds[0], accountIds[1], accountIds[2], accountIds[3],
            accountIds[4], accountIds[5], accountIds[6]);

        Assert.Equal(accountIds[0], parameters.TradeReceivableAccountId);
        Assert.Equal(accountIds[1], parameters.SalesRevenueAccountId);
        Assert.Equal(accountIds[2], parameters.SalesTaxPayableAccountId);
        Assert.Equal(accountIds[3], parameters.CashAccountId);
        Assert.Equal(accountIds[4], parameters.BankAccountId);
        Assert.Equal(accountIds[5], parameters.CostOfGoodsSoldAccountId);
        Assert.Equal(accountIds[6], parameters.InventoryAccountId);
    }

    private static ARInvoice CreateInvoice()
        => new(
            Guid.NewGuid(),
            "INV-TEST",
            Guid.NewGuid(),
            new DateTime(2026, 7, 5),
            new DateTime(2026, 8, 4),
            "Test invoice",
            100m,
            0m,
            0m);
}
