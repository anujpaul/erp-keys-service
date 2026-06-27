using ERPKeys.Domain.Modules.GeneralLedger;

namespace ERPKeys.Application.Modules.GeneralLedger.Services;

public static class FiscalPeriodStatusValidator
{
    public static void ValidateStatusChange(
        IReadOnlyCollection<FiscalPeriod> fiscalYearPeriods,
        Guid periodId,
        FiscalPeriodStatus requestedStatus)
    {
        var period = fiscalYearPeriods.FirstOrDefault(p => p.Id == periodId)
            ?? throw new InvalidOperationException("Period not found.");

        if (period.Status == FiscalPeriodStatus.PermanentlyClosed
            && requestedStatus != FiscalPeriodStatus.PermanentlyClosed)
        {
            throw new InvalidOperationException(
                "A permanently closed fiscal period cannot be reopened or changed.");
        }

        if (requestedStatus is FiscalPeriodStatus.Closed or FiscalPeriodStatus.PermanentlyClosed)
        {
            var earlierOpenPeriod = fiscalYearPeriods
                .Where(p => p.PeriodNumber < period.PeriodNumber)
                .OrderBy(p => p.PeriodNumber)
                .FirstOrDefault(p => p.Status == FiscalPeriodStatus.Open);

            if (earlierOpenPeriod is not null)
            {
                throw new InvalidOperationException(
                    $"Period {period.PeriodNumber} cannot be closed before period {earlierOpenPeriod.PeriodNumber}.");
            }
        }
    }
}
