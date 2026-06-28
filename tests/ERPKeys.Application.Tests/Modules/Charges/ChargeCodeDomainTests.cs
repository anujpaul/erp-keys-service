using ERPKeys.Domain.Modules.GeneralLedger;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.Charges;

public class ChargeCodeDomainTests
{
    [Fact]
    public void Percentage_charge_rejects_value_above_one_hundred()
    {
        var exception = Assert.Throws<ArgumentException>(() => new ChargeCode(
            Guid.NewGuid(),
            ChargeModule.AccountsReceivable,
            "LATE",
            "Late fee",
            null,
            ChargeCalculationMethod.Percentage,
            101,
            null,
            false,
            Guid.NewGuid()));

        Assert.Contains("cannot exceed 100", exception.Message);
    }

    [Fact]
    public void Percentage_charge_does_not_retain_currency()
    {
        var charge = new ChargeCode(
            Guid.NewGuid(),
            ChargeModule.AccountsPayable,
            "BANK",
            "Bank fee",
            null,
            ChargeCalculationMethod.Percentage,
            2.5m,
            "USD",
            false,
            Guid.NewGuid());

        Assert.Null(charge.CurrencyCode);
        Assert.Equal("BANK", charge.Code);
    }

    [Fact]
    public void Charge_can_be_deactivated_and_reactivated()
    {
        var charge = new ChargeCode(
            Guid.NewGuid(),
            ChargeModule.AccountsPayable,
            "STOCK",
            "Stocking fee",
            null,
            ChargeCalculationMethod.FixedAmount,
            15,
            "usd",
            true,
            Guid.NewGuid());

        charge.Deactivate();
        Assert.False(charge.IsActive);
        charge.Activate();
        Assert.True(charge.IsActive);
        Assert.Equal("USD", charge.CurrencyCode);
    }
}
