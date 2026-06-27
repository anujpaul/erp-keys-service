using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public enum FiscalPeriodStatus { Open, Closed, PermanentlyClosed }

public class FiscalPeriod : BaseEntity
{
    public Guid FiscalYearId { get; private set; }
    public int PeriodNumber { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public FiscalPeriodStatus Status { get; private set; } = FiscalPeriodStatus.Open;

    public FiscalYear? FiscalYear { get; private set; }

    private FiscalPeriod() { }

    public FiscalPeriod(Guid fiscalYearId, int periodNumber, string name, DateTime startDate, DateTime endDate)
    {
        if (periodNumber is < 1 or > 13)
            throw new ArgumentOutOfRangeException(nameof(periodNumber), "Fiscal period number must be between 1 and 13.");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Fiscal period name is required.", nameof(name));
        if (startDate > endDate)
            throw new ArgumentException("Fiscal period start date must not be after end date.");

        FiscalYearId = fiscalYearId;
        PeriodNumber = periodNumber;
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }

    public void ChangeStatus(FiscalPeriodStatus status)
    {
        if (Status == FiscalPeriodStatus.PermanentlyClosed && status != FiscalPeriodStatus.PermanentlyClosed)
            throw new InvalidOperationException("A permanently closed fiscal period cannot be reopened or changed.");

        Status = status;
        SetUpdated();
    }

    public void Close() => ChangeStatus(FiscalPeriodStatus.Closed);

    public void PermanentlyClose()
    {
        if (Status == FiscalPeriodStatus.PermanentlyClosed)
            throw new InvalidOperationException("Period is already permanently closed.");
        ChangeStatus(FiscalPeriodStatus.PermanentlyClosed);
    }

    public void Reopen()
    {
        if (Status != FiscalPeriodStatus.Closed)
            throw new InvalidOperationException("Only a closed period can be reopened.");
        ChangeStatus(FiscalPeriodStatus.Open);
    }

    public void Update(string name, DateTime startDate, DateTime endDate)
    {
        if (Status == FiscalPeriodStatus.PermanentlyClosed)
            throw new InvalidOperationException("A permanently closed fiscal period cannot be edited.");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Fiscal period name is required.", nameof(name));
        if (startDate > endDate)
            throw new ArgumentException("Fiscal period start date must not be after end date.");

        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        SetUpdated();
    }

    /// <summary>Internal — called by FiscalYear when re-numbering after a delete.</summary>
    internal void SetPeriodNumber(int num)
    {
        PeriodNumber = num;
        SetUpdated();
    }
}
