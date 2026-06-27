using ERPKeys.Application.Modules.GeneralLedger.Services;
using ERPKeys.Domain.Modules.GeneralLedger;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.GeneralLedger;

public class FiscalPeriodStatusValidatorTests
{
    [Fact]
    public void ValidateStatusChange_PreventsClosingLaterPeriodWhenEarlierPeriodIsOpen()
    {
        var periods = CreateMonthlyPeriods();
        periods[0].Close();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            FiscalPeriodStatusValidator.ValidateStatusChange(
                periods,
                periods[2].Id,
                FiscalPeriodStatus.Closed));

        Assert.Contains("Period 3 cannot be closed before period 2", ex.Message);
    }

    [Fact]
    public void ValidateStatusChange_AllowsClosingNextOpenPeriodInSequence()
    {
        var periods = CreateMonthlyPeriods();
        periods[0].Close();

        var exception = Record.Exception(() =>
            FiscalPeriodStatusValidator.ValidateStatusChange(
                periods,
                periods[1].Id,
                FiscalPeriodStatus.Closed));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateStatusChange_PreventsRevertingPermanentlyClosedPeriod()
    {
        var periods = CreateMonthlyPeriods();
        periods[0].PermanentlyClose();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            FiscalPeriodStatusValidator.ValidateStatusChange(
                periods,
                periods[0].Id,
                FiscalPeriodStatus.Open));

        Assert.Contains("permanently closed fiscal period cannot be reopened", ex.Message);
    }

    private static List<FiscalPeriod> CreateMonthlyPeriods()
    {
        var fiscalYear = new FiscalYear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "FY2026",
            "Fiscal Year 2026",
            new DateTime(2026, 1, 1),
            new DateTime(2026, 12, 31),
            FiscalCalendarTypes.Monthly);

        fiscalYear.GenerateMonthlyPeriods();
        return fiscalYear.Periods.OrderBy(p => p.PeriodNumber).ToList();
    }
}
