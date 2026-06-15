using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Modules.GeneralLedger.DTOs;
using ERPKeys.Domain.Modules.GeneralLedger;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ERPKeys.Application.Modules.GeneralLedger.Services;

public interface IGeneralLedgerService
{
    Task<IEnumerable<ChartOfAccountsDto>> GetChartsOfAccountsAsync(CancellationToken ct = default);
    Task<ChartOfAccountsDto> CreateChartOfAccountsAsync(CreateChartOfAccountsRequest req, CancellationToken ct = default);
    Task<IEnumerable<LedgerDto>> GetLedgersAsync(CancellationToken ct = default);
    Task<LedgerDto> CreateLedgerAsync(CreateLedgerRequest req, CancellationToken ct = default);
    Task SetDefaultLedgerAsync(Guid id, CancellationToken ct = default);
    Task<GeneralLedgerParametersDto?> GetParametersAsync(CancellationToken ct = default);
    Task<GeneralLedgerParametersDto> UpdateParametersAsync(UpdateGeneralLedgerParametersRequest req, CancellationToken ct = default);

    // Fiscal Calendar
    Task<IEnumerable<FiscalCalendarDto>> GetFiscalCalendarsAsync(CancellationToken ct = default);
    Task<FiscalCalendarDto> CreateFiscalCalendarAsync(CreateFiscalCalendarRequest req, CancellationToken ct = default);
    Task SetDefaultFiscalCalendarAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<FiscalYearDto>> GetFiscalYearsAsync(Guid? fiscalCalendarId = null, CancellationToken ct = default);
    Task<FiscalYearDto?> GetFiscalYearAsync(Guid id, CancellationToken ct = default);
    Task<FiscalYearDto> CreateFiscalYearAsync(CreateFiscalYearRequest req, CancellationToken ct = default);
    Task CloseFiscalYearAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<FiscalPeriodDto>> GetPeriodsAsync(Guid fiscalYearId, CancellationToken ct = default);
    Task<FiscalPeriodDto> CreatePeriodAsync(Guid fiscalYearId, CreatePeriodRequest req, CancellationToken ct = default);
    Task<IEnumerable<FiscalPeriodDto>> GeneratePeriodsAsync(Guid fiscalYearId, GeneratePeriodsRequest req, CancellationToken ct = default);
    Task<FiscalPeriodDto> UpdatePeriodAsync(Guid fiscalYearId, Guid periodId, UpdatePeriodRequest req, CancellationToken ct = default);
    Task DeletePeriodAsync(Guid fiscalYearId, Guid periodId, CancellationToken ct = default);
    Task ClosePeriodAsync(Guid periodId, CancellationToken ct = default);
    Task<FiscalPeriodDto?> GetCurrentPeriodAsync(CancellationToken ct = default);

    // Chart of Accounts
    Task<IEnumerable<AccountTypeDto>> GetAccountTypesAsync(CancellationToken ct = default);
    Task<IEnumerable<AccountDto>> GetAccountsAsync(Guid? chartOfAccountsId = null, CancellationToken ct = default);
    Task<AccountDto> CreateAccountAsync(CreateAccountRequest req, CancellationToken ct = default);
    Task DeactivateAccountAsync(Guid id, CancellationToken ct = default);

    // Financial Dimensions
    Task<IEnumerable<FinancialDimensionDto>> GetFinancialDimensionsAsync(CancellationToken ct = default);
    Task<FinancialDimensionDto> CreateFinancialDimensionAsync(CreateFinancialDimensionRequest req, CancellationToken ct = default);
    Task<FinancialDimensionValueDto> CreateFinancialDimensionValueAsync(Guid dimensionId, CreateFinancialDimensionValueRequest req, CancellationToken ct = default);
    Task DeactivateFinancialDimensionAsync(Guid id, CancellationToken ct = default);
    Task DeactivateFinancialDimensionValueAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<FinancialDimensionSetDto>> GetFinancialDimensionSetsAsync(CancellationToken ct = default);
    Task<FinancialDimensionSetDto> CreateFinancialDimensionSetAsync(CreateFinancialDimensionSetRequest req, CancellationToken ct = default);
    Task SetDefaultFinancialDimensionSetAsync(Guid id, CancellationToken ct = default);
    Task DeactivateFinancialDimensionSetAsync(Guid id, CancellationToken ct = default);

    // Journal Entries
    Task<IEnumerable<JournalEntryDto>> GetJournalEntriesAsync(Guid? fiscalPeriodId = null, CancellationToken ct = default);
    Task<JournalEntryDto?> GetJournalEntryAsync(Guid id, CancellationToken ct = default);
    Task<JournalEntryDto> CreateJournalEntryAsync(CreateJournalEntryRequest req, CancellationToken ct = default);
    Task<JournalEntryDto> CreateAndPostJournalEntryAsync(CreateJournalEntryRequest req, CancellationToken ct = default);
    Task PostJournalEntryAsync(Guid id, CancellationToken ct = default);
    Task VoidJournalEntryAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<GeneralJournalVoucherTemplateDto>> GetVoucherTemplatesAsync(CancellationToken ct = default);
    Task<GeneralJournalVoucherTemplateDto> SaveVoucherTemplateAsync(
        SaveGeneralJournalVoucherTemplateRequest req, CancellationToken ct = default);
    Task DeleteVoucherTemplateAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<AccrualSchemeDto>> GetAccrualSchemesAsync(CancellationToken ct = default);
    Task<AccrualSchemeDto> CreateAccrualSchemeAsync(
        CreateAccrualSchemeRequest req, CancellationToken ct = default);
    Task DeactivateAccrualSchemeAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<AccrualPostingRunDto>> GetAccrualPostingRunsAsync(
        Guid? schemeId = null, CancellationToken ct = default);
    Task<AccrualPostingRunDto> PostAccrualSchemeAsync(
        Guid schemeId, PostAccrualSchemeRequest req, CancellationToken ct = default);
    Task<IEnumerable<TrialBalanceLineDto>> GetTrialBalanceAsync(Guid fiscalPeriodId, CancellationToken ct = default);

    // Currencies
    Task<IEnumerable<CurrencyDto>> GetCurrenciesAsync(bool activeOnly = false, CancellationToken ct = default);
    Task<CurrencyDto?> GetCurrencyAsync(Guid id, CancellationToken ct = default);
    Task<CurrencyDto?> GetBaseCurrencyAsync(CancellationToken ct = default);
    Task<CurrencyDto> CreateCurrencyAsync(CreateCurrencyRequest req, CancellationToken ct = default);
    Task<CurrencyDto> UpdateCurrencyAsync(Guid id, UpdateCurrencyRequest req, CancellationToken ct = default);
    Task<CurrencyDto> UpdateExchangeRateAsync(Guid id, UpdateExchangeRateRequest req, CancellationToken ct = default);
    Task<CurrencyDto> SetBaseCurrencyAsync(Guid id, CancellationToken ct = default);
    Task ActivateCurrencyAsync(Guid id, CancellationToken ct = default);
    Task DeactivateCurrencyAsync(Guid id, CancellationToken ct = default);
}

public class GeneralLedgerService : IGeneralLedgerService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentOrganizationService _org;
    private readonly ICurrentUserService _user;

    public GeneralLedgerService(
        IAppDbContext db,
        ICurrentOrganizationService org,
        ICurrentUserService user)
    {
        _db = db;
        _org = org;
        _user = user;
    }

    // ── Fiscal Calendar ───────────────────────────────────────────────────────

    public async Task<IEnumerable<FiscalCalendarDto>> GetFiscalCalendarsAsync(CancellationToken ct = default)
    {
        var calendars = await _db.FiscalCalendars
            .Include(c => c.FiscalYears)
            .OrderByDescending(c => c.IsDefault)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);
        return calendars.Select(ToFiscalCalendarDto);
    }

    public async Task<IEnumerable<ChartOfAccountsDto>> GetChartsOfAccountsAsync(CancellationToken ct = default)
    {
        var charts = await _db.ChartsOfAccounts.Include(c => c.Accounts)
            .OrderByDescending(c => c.IsDefault).ThenBy(c => c.Name).ToListAsync(ct);
        return charts.Select(ToChartOfAccountsDto);
    }

    public async Task<ChartOfAccountsDto> CreateChartOfAccountsAsync(
        CreateChartOfAccountsRequest req, CancellationToken ct = default)
    {
        var code = req.Code.Trim().ToUpperInvariant();
        if (await _db.ChartsOfAccounts.AnyAsync(c => c.Code == code, ct))
            throw new InvalidOperationException($"Chart of accounts '{code}' already exists.");

        var makeDefault = req.IsDefault || !await _db.ChartsOfAccounts.AnyAsync(ct);
        if (makeDefault)
            await _db.ChartsOfAccounts.Where(c => c.IsDefault)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsDefault, false), ct);

        var chart = new ChartOfAccounts(
            _org.OrganizationId, code, req.Name, req.Description, makeDefault);
        _db.ChartsOfAccounts.Add(chart);
        await _db.SaveChangesAsync(ct);
        return ToChartOfAccountsDto(chart);
    }

    public async Task<IEnumerable<LedgerDto>> GetLedgersAsync(CancellationToken ct = default)
    {
        var ledgers = await _db.Ledgers
            .Include(l => l.FunctionalCurrency)
            .Include(l => l.ReportingCurrency)
            .Include(l => l.FiscalCalendar)
            .Include(l => l.ChartOfAccounts)
            .OrderByDescending(l => l.IsDefault).ThenBy(l => l.Name)
            .ToListAsync(ct);
        return ledgers.Select(ToLedgerDto);
    }

    public async Task<LedgerDto> CreateLedgerAsync(
        CreateLedgerRequest req, CancellationToken ct = default)
    {
        var code = req.Code.Trim().ToUpperInvariant();
        if (await _db.Ledgers.AnyAsync(l => l.Code == code, ct))
            throw new InvalidOperationException($"Ledger '{code}' already exists.");

        var currency = await _db.Currencies.FirstOrDefaultAsync(
            c => c.Id == req.FunctionalCurrencyId && c.IsActive, ct)
            ?? throw new InvalidOperationException("Functional currency not found or inactive.");
        Currency? reportingCurrency = null;
        if (req.ReportingCurrencyId.HasValue)
        {
            reportingCurrency = await _db.Currencies.FirstOrDefaultAsync(
                c => c.Id == req.ReportingCurrencyId.Value && c.IsActive, ct)
                ?? throw new InvalidOperationException("Reporting currency not found or inactive.");
            if (reportingCurrency.Id == currency.Id)
                throw new InvalidOperationException(
                    "Reporting currency must differ from functional currency.");
        }
        var calendar = await _db.FiscalCalendars.FirstOrDefaultAsync(
            c => c.Id == req.FiscalCalendarId, ct)
            ?? throw new InvalidOperationException("Fiscal calendar not found.");
        var chart = await _db.ChartsOfAccounts.FirstOrDefaultAsync(
            c => c.Id == req.ChartOfAccountsId && c.IsActive, ct)
            ?? throw new InvalidOperationException("Chart of accounts not found or inactive.");
        var makeDefault = req.IsDefault || !await _db.Ledgers.AnyAsync(ct);
        if (makeDefault)
            await _db.Ledgers.Where(l => l.IsDefault)
                .ExecuteUpdateAsync(s => s.SetProperty(l => l.IsDefault, false), ct);

        var ledger = new Ledger(_org.OrganizationId, code, req.Name,
            currency.Id, calendar.Id, chart.Id, req.Description, makeDefault,
            reportingCurrency?.Id);
        _db.Ledgers.Add(ledger);
        await _db.SaveChangesAsync(ct);
        return new LedgerDto(ledger.Id, ledger.Code, ledger.Name, ledger.Description,
            currency.Id, currency.Code, reportingCurrency?.Id, reportingCurrency?.Code,
            calendar.Id, calendar.Name,
            chart.Id, chart.Name, ledger.IsDefault, ledger.IsActive);
    }

    public async Task SetDefaultLedgerAsync(Guid id, CancellationToken ct = default)
    {
        var ledger = await _db.Ledgers.FirstOrDefaultAsync(l => l.Id == id, ct)
            ?? throw new InvalidOperationException("Ledger not found.");
        if (ledger.IsDefault) return;
        await _db.Ledgers.Where(l => l.IsDefault)
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.IsDefault, false), ct);
        ledger.SetDefault(true);
        var parameters = await _db.GeneralLedgerParameters.FirstOrDefaultAsync(ct);
        if (parameters is not null)
        {
            parameters.Update(
                ledger.Id,
                parameters.DefaultFinancialDimensionSetId,
                parameters.RetainedEarningsAccountId,
                parameters.RoundingDifferenceAccountId,
                parameters.RealizedGainAccountId,
                parameters.RealizedLossAccountId,
                parameters.UnrealizedGainAccountId,
                parameters.UnrealizedLossAccountId,
                parameters.AllowPostingToClosedPeriods,
                parameters.RequireDimensionsOnJournalLines,
                parameters.MaximumPennyDifference,
                parameters.DefaultJournalType);
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task<GeneralLedgerParametersDto?> GetParametersAsync(
        CancellationToken ct = default)
    {
        var parameters = await ParametersQuery().FirstOrDefaultAsync(ct);
        return parameters is null ? null : ToParametersDto(parameters);
    }

    public async Task<GeneralLedgerParametersDto> UpdateParametersAsync(
        UpdateGeneralLedgerParametersRequest req,
        CancellationToken ct = default)
    {
        var ledger = await _db.Ledgers.FirstOrDefaultAsync(
            l => l.Id == req.DefaultLedgerId && l.IsActive, ct)
            ?? throw new InvalidOperationException("Ledger not found or inactive.");

        if (!await _db.FiscalCalendars.AnyAsync(c => c.Id == req.FiscalCalendarId, ct))
            throw new InvalidOperationException("Fiscal calendar not found.");
        if (!await _db.Currencies.AnyAsync(
            c => c.Id == req.FunctionalCurrencyId && c.IsActive, ct))
            throw new InvalidOperationException("Functional currency not found or inactive.");
        if (!await _db.ChartsOfAccounts.AnyAsync(
            c => c.Id == req.ChartOfAccountsId && c.IsActive, ct))
            throw new InvalidOperationException("Chart of accounts not found or inactive.");

        if (req.DefaultFinancialDimensionSetId.HasValue &&
            !await _db.FinancialDimensionSets.AnyAsync(
                s => s.Id == req.DefaultFinancialDimensionSetId.Value && s.IsActive, ct))
        {
            throw new InvalidOperationException(
                "Default financial dimension set not found or inactive.");
        }

        var systemAccountIds = new[]
        {
            req.RetainedEarningsAccountId,
            req.RoundingDifferenceAccountId,
            req.RealizedGainAccountId,
            req.RealizedLossAccountId,
            req.UnrealizedGainAccountId,
            req.UnrealizedLossAccountId
        }.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();

        var validAccountCount = await _db.Accounts.CountAsync(
            a => systemAccountIds.Contains(a.Id)
                && a.ChartOfAccountsId == req.ChartOfAccountsId
                && a.Status == AccountStatus.Active
                && !a.IsHeaderAccount
                && a.AllowManualEntry, ct);
        if (validAccountCount != systemAccountIds.Count)
        {
            throw new InvalidOperationException(
                "System accounts must be active posting accounts in the ledger's chart of accounts.");
        }

        ledger.Configure(
            req.LedgerName,
            req.FunctionalCurrencyId,
            req.FiscalCalendarId,
            req.ChartOfAccountsId);

        var parameters = await _db.GeneralLedgerParameters.FirstOrDefaultAsync(ct);
        if (parameters is null)
        {
            parameters = new GeneralLedgerParameters(_org.OrganizationId, ledger.Id);
            _db.GeneralLedgerParameters.Add(parameters);
        }

        parameters.Update(
            ledger.Id,
            req.DefaultFinancialDimensionSetId,
            req.RetainedEarningsAccountId,
            req.RoundingDifferenceAccountId,
            req.RealizedGainAccountId,
            req.RealizedLossAccountId,
            req.UnrealizedGainAccountId,
            req.UnrealizedLossAccountId,
            req.AllowPostingToClosedPeriods,
            req.RequireDimensionsOnJournalLines,
            req.MaximumPennyDifference,
            req.DefaultJournalType);

        await _db.Ledgers.Where(l => l.IsDefault && l.Id != ledger.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.IsDefault, false), ct);
        ledger.SetDefault(true);
        await _db.SaveChangesAsync(ct);

        return ToParametersDto(
            await ParametersQuery().SingleAsync(p => p.Id == parameters.Id, ct));
    }

    public async Task<FiscalCalendarDto> CreateFiscalCalendarAsync(
        CreateFiscalCalendarRequest req,
        CancellationToken ct = default)
    {
        var normalizedName = req.Name.Trim().ToLower();
        if (await _db.FiscalCalendars.AnyAsync(c => c.Name.ToLower() == normalizedName, ct))
            throw new InvalidOperationException("A fiscal calendar with this name already exists.");

        var makeDefault = req.IsDefault || !await _db.FiscalCalendars.AnyAsync(ct);
        if (makeDefault)
            await _db.FiscalCalendars
                .Where(c => c.IsDefault)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(c => c.IsDefault, false),
                    ct);

        var calendar = new FiscalCalendar(
            _org.OrganizationId, req.Name, req.Description, req.CalendarType, makeDefault);
        _db.FiscalCalendars.Add(calendar);
        await _db.SaveChangesAsync(ct);
        return ToFiscalCalendarDto(calendar);
    }

    public async Task SetDefaultFiscalCalendarAsync(Guid id, CancellationToken ct = default)
    {
        var calendar = await _db.FiscalCalendars.FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new InvalidOperationException("Fiscal calendar not found.");
        if (calendar.IsDefault) return;

        await _db.FiscalCalendars
            .Where(c => c.IsDefault)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(c => c.IsDefault, false),
                ct);
        calendar.SetDefault(true);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<FiscalYearDto>> GetFiscalYearsAsync(
        Guid? fiscalCalendarId = null,
        CancellationToken ct = default)
    {
        var query = _db.FiscalYears
            .Include(y => y.FiscalCalendar)
            .Where(y => !y.IsDeleted);
        if (fiscalCalendarId.HasValue)
            query = query.Where(y => y.FiscalCalendarId == fiscalCalendarId.Value);

        var years = await query
            .OrderByDescending(y => y.StartDate)
            .ToListAsync(ct);
        return years.Select(ToFiscalYearDto);
    }

    public async Task<FiscalYearDto?> GetFiscalYearAsync(Guid id, CancellationToken ct = default)
    {
        var y = await _db.FiscalYears
            .Include(x => x.FiscalCalendar)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        return y is null ? null : ToFiscalYearDto(y);
    }

    public async Task<FiscalYearDto> CreateFiscalYearAsync(CreateFiscalYearRequest req, CancellationToken ct = default)
    {
        var calendar = await _db.FiscalCalendars.FirstOrDefaultAsync(
            c => c.Id == req.FiscalCalendarId, ct)
            ?? throw new InvalidOperationException("Fiscal calendar not found.");

        if (await _db.FiscalYears.AnyAsync(y =>
            y.FiscalCalendarId == calendar.Id &&
            req.StartDate <= y.EndDate &&
            req.EndDate >= y.StartDate, ct))
            throw new InvalidOperationException("Fiscal years in the same calendar cannot overlap.");

        var fy = new FiscalYear(
            _org.OrganizationId,
            calendar.Id,
            req.Name,
            req.Description,
            req.StartDate,
            req.EndDate,
            calendar.CalendarType);
        if (req.AutoGeneratePeriods && calendar.CalendarType != FiscalCalendarTypes.Custom)
            fy.GeneratePeriodsForCalendar();
        _db.FiscalYears.Add(fy);
        await _db.SaveChangesAsync(ct);
        return ToFiscalYearDto(fy, calendar.Name);
    }

    public async Task<FiscalPeriodDto> CreatePeriodAsync(Guid fiscalYearId, CreatePeriodRequest req, CancellationToken ct = default)
    {
        var fy = await _db.FiscalYears
            .Include(y => y.FiscalCalendar)
            .Include(y => y.Periods)
            .FirstOrDefaultAsync(y => y.Id == fiscalYearId && !y.IsDeleted, ct)
            ?? throw new InvalidOperationException("Fiscal year not found.");
        var period = fy.AddPeriod(req.Name, req.StartDate, req.EndDate);
        await _db.SaveChangesAsync(ct);
        return ToPeriodDto(period);
    }

    public async Task<IEnumerable<FiscalPeriodDto>> GeneratePeriodsAsync(Guid fiscalYearId, GeneratePeriodsRequest req, CancellationToken ct = default)
    {
        var fy = await _db.FiscalYears
            .Include(y => y.Periods)
            .FirstOrDefaultAsync(y => y.Id == fiscalYearId && !y.IsDeleted, ct)
            ?? throw new InvalidOperationException("Fiscal year not found.");

        // 1. Wipe existing periods from DB
        var existing = await _db.FiscalPeriods.Where(p => p.FiscalYearId == fiscalYearId).ToListAsync(ct);
        _db.FiscalPeriods.RemoveRange(existing);
        await _db.SaveChangesAsync(ct);

        // 2. Build period list using domain helpers (these operate on an in-memory FY)
        if (fy.CalendarType == FiscalCalendarTypes.Custom)
            throw new InvalidOperationException("Custom calendars require periods to be entered manually.");

        var template = new FiscalYear(
            _org.OrganizationId,
            fy.FiscalCalendarId,
            fy.Name,
            fy.Description,
            fy.StartDate,
            fy.EndDate,
            fy.CalendarType);
        template.GeneratePeriodsForCalendar();

        // 3. Persist generated periods with the real FiscalYearId
        var saved = new List<FiscalPeriod>();
        foreach (var p in template.Periods.OrderBy(x => x.PeriodNumber))
        {
            var period = new FiscalPeriod(fiscalYearId, p.PeriodNumber, p.Name, p.StartDate, p.EndDate);
            _db.FiscalPeriods.Add(period);
            saved.Add(period);
        }
        await _db.SaveChangesAsync(ct);

        return saved.Select(ToPeriodDto);
    }

    public async Task<FiscalPeriodDto> UpdatePeriodAsync(Guid fiscalYearId, Guid periodId, UpdatePeriodRequest req, CancellationToken ct = default)
    {
        var period = await _db.FiscalPeriods
            .FirstOrDefaultAsync(p => p.Id == periodId && p.FiscalYearId == fiscalYearId && !p.IsDeleted, ct)
            ?? throw new InvalidOperationException("Period not found.");
        if (period.Status != FiscalPeriodStatus.Open)
            throw new InvalidOperationException("Only Open periods can be edited.");
        period.Update(req.Name, req.StartDate, req.EndDate);
        await _db.SaveChangesAsync(ct);
        return ToPeriodDto(period);
    }

    public async Task DeletePeriodAsync(Guid fiscalYearId, Guid periodId, CancellationToken ct = default)
    {
        var hasEntries = await _db.JournalEntries.AnyAsync(j => j.FiscalPeriodId == periodId && !j.IsDeleted, ct);
        if (hasEntries) throw new InvalidOperationException("Cannot delete a period that has journal entries posted to it.");

        var period = await _db.FiscalPeriods
            .FirstOrDefaultAsync(p => p.Id == periodId && p.FiscalYearId == fiscalYearId && !p.IsDeleted, ct)
            ?? throw new InvalidOperationException("Period not found.");
        if (period.Status != FiscalPeriodStatus.Open)
            throw new InvalidOperationException("Only Open periods can be deleted.");

        _db.FiscalPeriods.Remove(period);
        await _db.SaveChangesAsync(ct);
    }

    public async Task CloseFiscalYearAsync(Guid id, CancellationToken ct = default)
    {
        var fy = await _db.FiscalYears.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Fiscal year not found.");
        fy.Close();
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<FiscalPeriodDto>> GetPeriodsAsync(Guid fiscalYearId, CancellationToken ct = default)
    {
        var periods = await _db.FiscalPeriods
            .Where(p => p.FiscalYearId == fiscalYearId && !p.IsDeleted)
            .OrderBy(p => p.PeriodNumber)
            .ToListAsync(ct);
        return periods.Select(ToPeriodDto);
    }

    public async Task ClosePeriodAsync(Guid periodId, CancellationToken ct = default)
    {
        var period = await _db.FiscalPeriods.FirstOrDefaultAsync(p => p.Id == periodId && !p.IsDeleted, ct)
            ?? throw new InvalidOperationException("Period not found.");
        period.Close();
        await _db.SaveChangesAsync(ct);
    }

    public async Task<FiscalPeriodDto?> GetCurrentPeriodAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var period = await _db.FiscalPeriods
            .Include(p => p.FiscalYear)
                .ThenInclude(y => y!.FiscalCalendar)
            .Where(p => !p.IsDeleted && p.Status == FiscalPeriodStatus.Open
                        && p.FiscalYear!.FiscalCalendar!.IsDefault
                        && p.StartDate <= today && p.EndDate >= today)
            .OrderBy(p => p.StartDate)
            .FirstOrDefaultAsync(ct);
        return period is null ? null : ToPeriodDto(period);
    }

    // ── Chart of Accounts ─────────────────────────────────────────────────────

    public async Task<IEnumerable<AccountTypeDto>> GetAccountTypesAsync(CancellationToken ct = default)
    {
        var types = await _db.AccountTypes.Where(t => !t.IsDeleted).OrderBy(t => t.DisplayOrder).ToListAsync(ct);
        return types.Select(t => new AccountTypeDto(t.Id, t.Code, t.Name, t.Nature.ToString(), t.DisplayOrder));
    }

    public async Task<IEnumerable<AccountDto>> GetAccountsAsync(
        Guid? chartOfAccountsId = null, CancellationToken ct = default)
    {
        var query = _db.Accounts
            .Include(a => a.AccountType)
            .Include(a => a.ParentAccount)
            .Where(a => !a.IsDeleted);
        if (chartOfAccountsId.HasValue)
            query = query.Where(a => a.ChartOfAccountsId == chartOfAccountsId.Value);
        var accounts = await query
            .OrderBy(a => a.AccountNumber)
            .ToListAsync(ct);
        return accounts.Select(ToAccountDto);
    }

    public async Task<AccountDto> CreateAccountAsync(CreateAccountRequest req, CancellationToken ct = default)
    {
        var chart = await _db.ChartsOfAccounts.FirstOrDefaultAsync(
            c => c.Id == req.ChartOfAccountsId && c.IsActive, ct)
            ?? throw new InvalidOperationException("Chart of accounts not found or inactive.");
        if (!await _db.AccountTypes.AnyAsync(t => t.Id == req.AccountTypeId, ct))
            throw new InvalidOperationException("Account type not found.");
        Account? parentAccount = null;
        if (req.ParentAccountId.HasValue)
        {
            parentAccount = await _db.Accounts.FirstOrDefaultAsync(
                a => a.Id == req.ParentAccountId &&
                    a.ChartOfAccountsId == chart.Id &&
                    a.IsHeaderAccount &&
                    a.Status == AccountStatus.Active, ct)
                ?? throw new InvalidOperationException(
                    "Parent account must be an active header account in the same chart of accounts.");
        }
        if (await _db.Accounts.AnyAsync(
            a => a.ChartOfAccountsId == chart.Id &&
                a.AccountNumber == req.AccountNumber.Trim(), ct))
            throw new InvalidOperationException(
                $"Account number '{req.AccountNumber.Trim()}' already exists in '{chart.Name}'.");
        var account = new Account(_org.OrganizationId, req.AccountNumber, req.Name, req.AccountTypeId,
            req.IsHeaderAccount, req.ParentAccountId, req.Description, req.Currency,
            level: parentAccount is null ? 1 : parentAccount.Level + 1,
            chartOfAccountsId: chart.Id);
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync(ct);

        var created = await _db.Accounts
            .Include(a => a.AccountType)
            .Include(a => a.ParentAccount)
            .FirstAsync(a => a.Id == account.Id, ct);
        return ToAccountDto(created);
    }

    public async Task DeactivateAccountAsync(Guid id, CancellationToken ct = default)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, ct)
            ?? throw new InvalidOperationException("Account not found.");
        account.Deactivate();
        await _db.SaveChangesAsync(ct);
    }

    // Financial Dimensions

    public async Task<IEnumerable<FinancialDimensionDto>> GetFinancialDimensionsAsync(CancellationToken ct = default)
    {
        var dimensions = await _db.FinancialDimensions
            .Include(d => d.Values)
            .OrderBy(d => d.Name)
            .ToListAsync(ct);
        return dimensions.Select(ToFinancialDimensionDto);
    }

    public async Task<FinancialDimensionDto> CreateFinancialDimensionAsync(
        CreateFinancialDimensionRequest req,
        CancellationToken ct = default)
    {
        var code = FinancialDimension.NormalizeCode(req.Code);
        if (await _db.FinancialDimensions.AnyAsync(d => d.Code == code, ct))
            throw new InvalidOperationException($"Financial dimension '{code}' already exists.");

        var dimension = new FinancialDimension(_org.OrganizationId, code, req.Name, req.Description);
        _db.FinancialDimensions.Add(dimension);
        await _db.SaveChangesAsync(ct);
        return ToFinancialDimensionDto(dimension);
    }

    public async Task<FinancialDimensionValueDto> CreateFinancialDimensionValueAsync(
        Guid dimensionId,
        CreateFinancialDimensionValueRequest req,
        CancellationToken ct = default)
    {
        var dimension = await _db.FinancialDimensions.FirstOrDefaultAsync(d => d.Id == dimensionId, ct)
            ?? throw new InvalidOperationException("Financial dimension not found.");
        if (!dimension.IsActive)
            throw new InvalidOperationException("Values cannot be added to an inactive dimension.");

        var code = FinancialDimension.NormalizeCode(req.Code);
        if (await _db.FinancialDimensionValues.AnyAsync(
            v => v.FinancialDimensionId == dimensionId && v.Code == code, ct))
            throw new InvalidOperationException($"Value '{code}' already exists in {dimension.Name}.");

        var value = new FinancialDimensionValue(dimensionId, code, req.Name, req.Description);
        _db.FinancialDimensionValues.Add(value);
        await _db.SaveChangesAsync(ct);
        return ToFinancialDimensionValueDto(value);
    }

    public async Task DeactivateFinancialDimensionAsync(Guid id, CancellationToken ct = default)
    {
        var dimension = await _db.FinancialDimensions.FirstOrDefaultAsync(d => d.Id == id, ct)
            ?? throw new InvalidOperationException("Financial dimension not found.");
        dimension.Deactivate();
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeactivateFinancialDimensionValueAsync(Guid id, CancellationToken ct = default)
    {
        var value = await _db.FinancialDimensionValues.FirstOrDefaultAsync(v => v.Id == id, ct)
            ?? throw new InvalidOperationException("Financial dimension value not found.");
        value.Deactivate();
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<FinancialDimensionSetDto>> GetFinancialDimensionSetsAsync(CancellationToken ct = default)
    {
        var sets = await _db.FinancialDimensionSets
            .Include(s => s.Members)
                .ThenInclude(m => m.FinancialDimension)
            .OrderByDescending(s => s.IsDefault)
            .ThenBy(s => s.Name)
            .ToListAsync(ct);
        return sets.Select(ToFinancialDimensionSetDto);
    }

    public async Task<FinancialDimensionSetDto> CreateFinancialDimensionSetAsync(
        CreateFinancialDimensionSetRequest req,
        CancellationToken ct = default)
    {
        var name = req.Name.Trim();
        if (await _db.FinancialDimensionSets.AnyAsync(s => s.Name.ToLower() == name.ToLower(), ct))
            throw new InvalidOperationException($"Financial dimension set '{name}' already exists.");

        var memberIds = req.Members.Select(m => m.FinancialDimensionId).Distinct().ToList();
        var dimensions = await _db.FinancialDimensions
            .Where(d => memberIds.Contains(d.Id) && d.IsActive)
            .ToListAsync(ct);
        if (dimensions.Count != memberIds.Count)
            throw new InvalidOperationException("One or more selected financial dimensions are missing or inactive.");

        var makeDefault = req.IsDefault || !await _db.FinancialDimensionSets.AnyAsync(ct);
        if (makeDefault)
            await _db.FinancialDimensionSets.Where(s => s.IsDefault)
                .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.IsDefault, false), ct);

        var set = new FinancialDimensionSet(
            _org.OrganizationId,
            name,
            req.Description,
            makeDefault,
            req.Members.Select(m => (m.FinancialDimensionId, m.IsRequired)));
        _db.FinancialDimensionSets.Add(set);
        await _db.SaveChangesAsync(ct);

        return (await GetFinancialDimensionSetsAsync(ct)).Single(s => s.Id == set.Id);
    }

    public async Task SetDefaultFinancialDimensionSetAsync(Guid id, CancellationToken ct = default)
    {
        var set = await _db.FinancialDimensionSets.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new InvalidOperationException("Financial dimension set not found.");
        if (!set.IsActive)
            throw new InvalidOperationException("An inactive dimension set cannot be the default.");
        if (set.IsDefault) return;

        await _db.FinancialDimensionSets.Where(s => s.IsDefault)
            .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.IsDefault, false), ct);
        set.SetDefault(true);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeactivateFinancialDimensionSetAsync(Guid id, CancellationToken ct = default)
    {
        var set = await _db.FinancialDimensionSets.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new InvalidOperationException("Financial dimension set not found.");
        set.Deactivate();
        await _db.SaveChangesAsync(ct);
    }

    // ── Journal Entries ───────────────────────────────────────────────────────

    public async Task<IEnumerable<JournalEntryDto>> GetJournalEntriesAsync(Guid? fiscalPeriodId = null, CancellationToken ct = default)
    {
        var query = _db.JournalEntries
            .Include(e => e.Ledger)
            .Include(e => e.FiscalPeriod)
            .Include(e => e.Lines).ThenInclude(l => l.Account)
            .Include(e => e.Lines).ThenInclude(l => l.FinancialDimensionSet)
            .Include(e => e.Lines).ThenInclude(l => l.DimensionValues)
                .ThenInclude(dv => dv.FinancialDimensionValue)
                .ThenInclude(v => v!.FinancialDimension)
            .Where(e => !e.IsDeleted);

        if (fiscalPeriodId.HasValue)
            query = query.Where(e => e.FiscalPeriodId == fiscalPeriodId.Value);

        var entries = await query.OrderByDescending(e => e.EntryDate).ToListAsync(ct);
        return entries.Select(ToJournalEntryDto);
    }

    public async Task<JournalEntryDto?> GetJournalEntryAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await _db.JournalEntries
            .Include(e => e.Ledger)
            .Include(e => e.FiscalPeriod)
            .Include(e => e.Lines).ThenInclude(l => l.Account)
            .Include(e => e.Lines).ThenInclude(l => l.FinancialDimensionSet)
            .Include(e => e.Lines).ThenInclude(l => l.DimensionValues)
                .ThenInclude(dv => dv.FinancialDimensionValue)
                .ThenInclude(v => v!.FinancialDimension)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);
        return entry is null ? null : ToJournalEntryDto(entry);
    }

    public async Task<JournalEntryDto> CreateJournalEntryAsync(CreateJournalEntryRequest req, CancellationToken ct = default)
        => await CreateJournalEntryAsync(req, postImmediately: false, ct);

    public async Task<JournalEntryDto> CreateAndPostJournalEntryAsync(
        CreateJournalEntryRequest req,
        CancellationToken ct = default)
        => await CreateJournalEntryAsync(req, postImmediately: true, ct);

    private async Task<JournalEntryDto> CreateJournalEntryAsync(
        CreateJournalEntryRequest req,
        bool postImmediately,
        CancellationToken ct)
    {
        var ledger = await _db.Ledgers
            .Include(l => l.FunctionalCurrency)
            .FirstOrDefaultAsync(l => l.Id == req.LedgerId && l.IsActive, ct)
            ?? throw new InvalidOperationException("Ledger not found or inactive.");
        var period = await _db.FiscalPeriods
            .Include(p => p.FiscalYear)
            .FirstOrDefaultAsync(p => p.Id == req.FiscalPeriodId
                && p.FiscalYear!.FiscalCalendarId == ledger.FiscalCalendarId, ct);
        if (period is null)
            throw new InvalidOperationException("The fiscal period does not belong to the ledger's fiscal calendar.");

        var parameters = await _db.GeneralLedgerParameters.FirstOrDefaultAsync(ct);
        if (period.Status != FiscalPeriodStatus.Open &&
            parameters?.AllowPostingToClosedPeriods != true)
            throw new InvalidOperationException(
                "Journal entries can only be created in an open fiscal period.");

        var accountIds = req.Lines?.Select(l => l.AccountId).Distinct().ToList() ?? [];
        var matchingAccountCount = await _db.Accounts.CountAsync(
            a => accountIds.Contains(a.Id)
                && a.ChartOfAccountsId == ledger.ChartOfAccountsId
                && a.Status == AccountStatus.Active, ct);
        if (matchingAccountCount != accountIds.Count)
            throw new InvalidOperationException(
                "All journal accounts must be active and belong to the ledger's chart of accounts.");

        var count = await _db.JournalEntries.CountAsync(ct) + 1;
        var entry = new JournalEntry(_org.OrganizationId, $"JE-{count:D6}", req.EntryDate, req.FiscalPeriodId,
            req.Description, req.Reference,
            string.IsNullOrWhiteSpace(req.JournalType)
                ? parameters?.DefaultJournalType ?? "General"
                : req.JournalType,
            ledger.FunctionalCurrency!.Code, ledger.Id);

        if (req.Lines != null)
        {
            foreach (var line in req.Lines)
            {
                if (parameters?.RequireDimensionsOnJournalLines == true &&
                    !line.FinancialDimensionSetId.HasValue)
                    throw new InvalidOperationException(
                        "A financial dimension set is required on every journal line.");

                await ValidateDimensionSelectionAsync(
                    line.FinancialDimensionSetId,
                    line.FinancialDimensionValueIds,
                    ct);
                entry.AddLine(
                    line.AccountId,
                    line.Description,
                    line.Debit,
                    line.Credit,
                    line.FinancialDimensionSetId,
                    line.FinancialDimensionValueIds);
            }
        }

        var difference = entry.TotalDebit - entry.TotalCredit;
        if (difference != 0 &&
            Math.Abs(difference) <= (parameters?.MaximumPennyDifference ?? 0))
        {
            if (parameters?.RoundingDifferenceAccountId is null)
                throw new InvalidOperationException(
                    "A rounding difference account is required to balance this journal.");

            entry.AddLine(
                parameters.RoundingDifferenceAccountId.Value,
                "Automatic rounding difference",
                difference < 0 ? Math.Abs(difference) : 0,
                difference > 0 ? difference : 0);
        }

        if (postImmediately)
            entry.Post();

        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync(ct);
        return (await GetJournalEntryAsync(entry.Id, ct))!;
    }

    public async Task<IEnumerable<GeneralJournalVoucherTemplateDto>> GetVoucherTemplatesAsync(
        CancellationToken ct = default)
    {
        var userId = RequireUserId();
        var templates = await _db.GeneralJournalVoucherTemplates
            .Include(t => t.Ledger)
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Name)
            .ToListAsync(ct);
        return templates.Select(ToVoucherTemplateDto);
    }

    public async Task<GeneralJournalVoucherTemplateDto> SaveVoucherTemplateAsync(
        SaveGeneralJournalVoucherTemplateRequest req,
        CancellationToken ct = default)
    {
        var userId = RequireUserId();
        var name = req.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Template name is required.");
        if (req.Lines is null || req.Lines.Count < 2)
            throw new InvalidOperationException("A voucher template must contain at least two lines.");

        var ledger = await _db.Ledgers.FirstOrDefaultAsync(
            l => l.Id == req.LedgerId && l.IsActive, ct)
            ?? throw new InvalidOperationException("Ledger not found or inactive.");

        var accountIds = req.Lines.Select(l => l.AccountId).Distinct().ToList();
        var accountCount = await _db.Accounts.CountAsync(
            a => accountIds.Contains(a.Id)
                && a.ChartOfAccountsId == ledger.ChartOfAccountsId
                && a.Status == AccountStatus.Active
                && !a.IsHeaderAccount
                && a.AllowManualEntry, ct);
        if (accountCount != accountIds.Count)
            throw new InvalidOperationException(
                "All template accounts must be active posting accounts in the selected ledger.");

        var parameters = await _db.GeneralLedgerParameters.FirstOrDefaultAsync(ct);
        foreach (var line in req.Lines)
        {
            if (line.Debit < 0 || line.Credit < 0 || (line.Debit > 0 && line.Credit > 0))
                throw new InvalidOperationException(
                    "Template line amounts must be non-negative and cannot contain both debit and credit.");
            if (parameters?.RequireDimensionsOnJournalLines == true &&
                !line.FinancialDimensionSetId.HasValue)
                throw new InvalidOperationException(
                    "A financial dimension set is required on every template line.");
            await ValidateDimensionSelectionAsync(
                line.FinancialDimensionSetId,
                line.FinancialDimensionValueIds,
                ct);
        }

        if (await _db.GeneralJournalVoucherTemplates.AnyAsync(
            t => t.UserId == userId && t.Name.ToLower() == name.ToLower(), ct))
            throw new InvalidOperationException($"A voucher template named '{name}' already exists.");

        var template = new GeneralJournalVoucherTemplate(
            _org.OrganizationId,
            userId,
            ledger.Id,
            name,
            req.Description,
            req.Reference,
            req.JournalType,
            JsonSerializer.Serialize(req.Lines));
        _db.GeneralJournalVoucherTemplates.Add(template);
        await _db.SaveChangesAsync(ct);

        return new GeneralJournalVoucherTemplateDto(
            template.Id, template.Name, template.LedgerId, ledger.Code,
            template.Description, template.Reference, template.JournalType,
            req.Lines, template.CreatedAt);
    }

    public async Task DeleteVoucherTemplateAsync(Guid id, CancellationToken ct = default)
    {
        var userId = RequireUserId();
        var template = await _db.GeneralJournalVoucherTemplates.FirstOrDefaultAsync(
            t => t.Id == id && t.UserId == userId, ct)
            ?? throw new InvalidOperationException("Voucher template not found.");
        template.SoftDelete();
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<AccrualSchemeDto>> GetAccrualSchemesAsync(
        CancellationToken ct = default)
    {
        var schemes = await AccrualSchemesQuery()
            .OrderByDescending(s => s.IsActive)
            .ThenBy(s => s.Code)
            .ToListAsync(ct);
        return schemes.Select(ToAccrualSchemeDto);
    }

    public async Task<AccrualSchemeDto> CreateAccrualSchemeAsync(
        CreateAccrualSchemeRequest req,
        CancellationToken ct = default)
    {
        var code = req.Code.Trim().ToUpperInvariant();
        if (await _db.AccrualSchemes.AnyAsync(s => s.Code == code, ct))
            throw new InvalidOperationException($"Accrual scheme '{code}' already exists.");

        if (!Enum.TryParse<AccrualAllocationMethod>(
            req.AllocationMethod, ignoreCase: true, out var allocationMethod))
            throw new ArgumentException("Allocation method must be Even or Custom.");

        var ledger = await _db.Ledgers.FirstOrDefaultAsync(
            l => l.Id == req.LedgerId && l.IsActive, ct)
            ?? throw new InvalidOperationException("Ledger not found or inactive.");

        var accountIds = new[] { req.DebitAccountId, req.CreditAccountId }.Distinct().ToList();
        var accountCount = await _db.Accounts.CountAsync(
            a => accountIds.Contains(a.Id)
                && a.ChartOfAccountsId == ledger.ChartOfAccountsId
                && a.Status == AccountStatus.Active
                && !a.IsHeaderAccount
                && a.AllowManualEntry, ct);
        if (accountCount != 2)
            throw new InvalidOperationException(
                "Debit and credit accounts must be different active posting accounts in the selected ledger.");

        var dimensionValueIds = req.FinancialDimensionValueIds?.Distinct().ToList() ?? [];
        await ValidateDimensionSelectionAsync(
            req.FinancialDimensionSetId, dimensionValueIds, ct);
        var parameters = await _db.GeneralLedgerParameters.FirstOrDefaultAsync(ct);
        if (parameters?.RequireDimensionsOnJournalLines == true &&
            !req.FinancialDimensionSetId.HasValue)
            throw new InvalidOperationException(
                "A financial dimension set is required for this accrual scheme.");

        var scheme = new AccrualScheme(
            _org.OrganizationId,
            code,
            req.Name,
            req.Description,
            ledger.Id,
            req.DebitAccountId,
            req.CreditAccountId,
            req.JournalType,
            allocationMethod,
            req.DefaultPeriodCount,
            req.FinancialDimensionSetId,
            JsonSerializer.Serialize(dimensionValueIds));
        scheme.SetAllocations(req.AllocationPercentages ?? []);
        _db.AccrualSchemes.Add(scheme);
        await _db.SaveChangesAsync(ct);

        return ToAccrualSchemeDto(
            await AccrualSchemesQuery().SingleAsync(s => s.Id == scheme.Id, ct));
    }

    public async Task DeactivateAccrualSchemeAsync(Guid id, CancellationToken ct = default)
    {
        var scheme = await _db.AccrualSchemes.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new InvalidOperationException("Accrual scheme not found.");
        scheme.Deactivate();
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<AccrualPostingRunDto>> GetAccrualPostingRunsAsync(
        Guid? schemeId = null,
        CancellationToken ct = default)
    {
        var query = AccrualPostingRunsQuery();
        if (schemeId.HasValue)
            query = query.Where(r => r.AccrualSchemeId == schemeId.Value);
        var runs = await query.OrderByDescending(r => r.PostedAt).ToListAsync(ct);
        return runs.Select(ToAccrualPostingRunDto);
    }

    public async Task<AccrualPostingRunDto> PostAccrualSchemeAsync(
        Guid schemeId,
        PostAccrualSchemeRequest req,
        CancellationToken ct = default)
    {
        var scheme = await AccrualSchemesQuery()
            .SingleOrDefaultAsync(s => s.Id == schemeId && s.IsActive, ct)
            ?? throw new InvalidOperationException("Accrual scheme not found or inactive.");
        var ledger = scheme.Ledger
            ?? throw new InvalidOperationException("The accrual scheme ledger could not be loaded.");

        var reference = req.Reference.Trim();
        if (await _db.AccrualPostingRuns.AnyAsync(
            r => r.AccrualSchemeId == scheme.Id && r.Reference == reference, ct))
            throw new InvalidOperationException(
                $"Accrual scheme '{scheme.Code}' has already been posted with reference '{reference}'.");

        var periods = await _db.FiscalPeriods
            .Include(p => p.FiscalYear)
            .Where(p => p.FiscalYear!.FiscalCalendarId == ledger.FiscalCalendarId)
            .OrderBy(p => p.StartDate)
            .ToListAsync(ct);
        var startIndex = periods.FindIndex(p => p.Id == req.StartFiscalPeriodId);
        if (startIndex < 0)
            throw new InvalidOperationException(
                "The starting fiscal period does not belong to the scheme ledger.");
        var targetPeriods = periods.Skip(startIndex).Take(scheme.DefaultPeriodCount).ToList();
        if (targetPeriods.Count != scheme.DefaultPeriodCount)
            throw new InvalidOperationException(
                "The fiscal calendar does not contain enough periods for this accrual schedule.");
        for (var index = 1; index < targetPeriods.Count; index++)
        {
            if (targetPeriods[index - 1].EndDate.AddDays(1).Date != targetPeriods[index].StartDate.Date)
                throw new InvalidOperationException("Accrual target fiscal periods must be contiguous.");
        }

        var parameters = await _db.GeneralLedgerParameters.FirstOrDefaultAsync(ct);
        if (parameters?.AllowPostingToClosedPeriods != true &&
            targetPeriods.Any(p => p.Status != FiscalPeriodStatus.Open))
            throw new InvalidOperationException(
                "All accrual target periods must be open before posting.");

        var percentages = scheme.AllocationMethod == AccrualAllocationMethod.Even
            ? CalculateEvenPercentages(scheme.DefaultPeriodCount)
            : scheme.Allocations.OrderBy(a => a.PeriodOffset).Select(a => a.Percentage).ToList();
        var amounts = CalculateAccrualAmounts(req.TotalAmount, percentages);
        var dimensionValueIds =
            JsonSerializer.Deserialize<List<Guid>>(scheme.FinancialDimensionValueIdsJson) ?? [];

        var run = new AccrualPostingRun(
            _org.OrganizationId,
            scheme.Id,
            _user.UserId,
            reference,
            req.Description ?? scheme.Description,
            req.StartFiscalPeriodId,
            req.TotalAmount);

        var entryNumber = await _db.JournalEntries.CountAsync(ct) + 1;
        for (var index = 0; index < targetPeriods.Count; index++)
        {
            var period = targetPeriods[index];
            var amount = amounts[index];
            var entry = new JournalEntry(
                _org.OrganizationId,
                $"JE-{entryNumber + index:D6}",
                period.EndDate,
                period.Id,
                string.IsNullOrWhiteSpace(req.Description) ? scheme.Name : req.Description!,
                reference,
                scheme.JournalType,
                ledger.FunctionalCurrency!.Code,
                scheme.LedgerId);
            entry.AddLine(
                scheme.DebitAccountId,
                $"{scheme.Name} - {period.Name}",
                amount,
                0,
                scheme.FinancialDimensionSetId,
                dimensionValueIds);
            entry.AddLine(
                scheme.CreditAccountId,
                $"{scheme.Name} - {period.Name}",
                0,
                amount,
                scheme.FinancialDimensionSetId,
                dimensionValueIds);
            entry.Post();
            _db.JournalEntries.Add(entry);
            run.AddLine(period.Id, entry.Id, index, percentages[index], amount);
        }

        _db.AccrualPostingRuns.Add(run);
        await _db.SaveChangesAsync(ct);

        return ToAccrualPostingRunDto(
            await AccrualPostingRunsQuery().SingleAsync(r => r.Id == run.Id, ct));
    }

    public async Task PostJournalEntryAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await _db.JournalEntries
            .Include(e => e.Lines)
            .Include(e => e.FiscalPeriod)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct)
            ?? throw new InvalidOperationException("Journal entry not found.");
        var parameters = await _db.GeneralLedgerParameters.FirstOrDefaultAsync(ct);
        if (entry.FiscalPeriod?.Status != FiscalPeriodStatus.Open &&
            parameters?.AllowPostingToClosedPeriods != true)
            throw new InvalidOperationException(
                "Journal entries can only be posted to an open fiscal period.");
        entry.Post();
        await _db.SaveChangesAsync(ct);
    }

    public async Task VoidJournalEntryAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await _db.JournalEntries
            .Include(e => e.Lines)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct)
            ?? throw new InvalidOperationException("Journal entry not found.");
        entry.Void();
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<TrialBalanceLineDto>> GetTrialBalanceAsync(Guid fiscalPeriodId, CancellationToken ct = default)
    {
        var lines = await _db.JournalLines
            .Include(l => l.Account).ThenInclude(a => a!.AccountType)
            .Include(l => l.JournalEntry)
            .Where(l => l.JournalEntry!.FiscalPeriodId == fiscalPeriodId
                        && l.JournalEntry.Status == JournalEntryStatus.Posted
                        && !l.JournalEntry.IsDeleted)
            .ToListAsync(ct);

        return lines
            .GroupBy(l => new { l.Account!.AccountNumber, l.Account.Name, TypeName = l.Account.AccountType!.Name })
            .Select(g => new TrialBalanceLineDto(
                g.Key.AccountNumber, g.Key.Name, g.Key.TypeName,
                g.Sum(l => l.Debit), g.Sum(l => l.Credit),
                g.Sum(l => l.Debit) - g.Sum(l => l.Credit)))
            .OrderBy(t => t.AccountNumber);
    }

    // ── Currencies ────────────────────────────────────────────────────────────

    public async Task<IEnumerable<CurrencyDto>> GetCurrenciesAsync(bool activeOnly = false, CancellationToken ct = default)
    {
        var query = _db.Currencies.Where(c => !c.IsDeleted);
        if (activeOnly) query = query.Where(c => c.IsActive);
        var list = await query.OrderByDescending(c => c.IsBase).ThenBy(c => c.Code).ToListAsync(ct);
        return list.Select(ToCurrencyDto);
    }

    public async Task<CurrencyDto?> GetCurrencyAsync(Guid id, CancellationToken ct = default)
    {
        var c = await _db.Currencies.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        return c is null ? null : ToCurrencyDto(c);
    }

    public async Task<CurrencyDto?> GetBaseCurrencyAsync(CancellationToken ct = default)
    {
        var c = await _db.Currencies.FirstOrDefaultAsync(x => x.IsBase && !x.IsDeleted, ct);
        return c is null ? null : ToCurrencyDto(c);
    }

    public async Task<CurrencyDto> CreateCurrencyAsync(CreateCurrencyRequest req, CancellationToken ct = default)
    {
        var code = req.Code.ToUpperInvariant();
        if (await _db.Currencies.AnyAsync(c => c.Code == code && !c.IsDeleted, ct))
            throw new InvalidOperationException($"Currency '{code}' already exists.");

        if (req.IsBase && await _db.Currencies.AnyAsync(c => c.IsBase && !c.IsDeleted, ct))
            throw new InvalidOperationException("A base currency already exists. Use SetBaseCurrency to change it.");

        var currency = new Currency(_org.OrganizationId, code, req.Name, req.Symbol,
            req.DecimalPlaces, req.IsBase ? 1m : req.ExchangeRate, req.IsBase,
            req.NumericCode, req.Country);

        _db.Currencies.Add(currency);
        await _db.SaveChangesAsync(ct);
        return ToCurrencyDto(currency);
    }

    public async Task<CurrencyDto> UpdateCurrencyAsync(Guid id, UpdateCurrencyRequest req, CancellationToken ct = default)
    {
        var currency = await _db.Currencies.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct)
            ?? throw new InvalidOperationException("Currency not found.");
        currency.UpdateDetails(req.Name, req.Symbol, req.DecimalPlaces, req.Country, req.NumericCode);
        await _db.SaveChangesAsync(ct);
        return ToCurrencyDto(currency);
    }

    public async Task<CurrencyDto> UpdateExchangeRateAsync(Guid id, UpdateExchangeRateRequest req, CancellationToken ct = default)
    {
        var currency = await _db.Currencies.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct)
            ?? throw new InvalidOperationException("Currency not found.");
        currency.UpdateExchangeRate(req.ExchangeRate);
        await _db.SaveChangesAsync(ct);
        return ToCurrencyDto(currency);
    }

    public async Task<CurrencyDto> SetBaseCurrencyAsync(Guid id, CancellationToken ct = default)
    {
        var newBase = await _db.Currencies.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct)
            ?? throw new InvalidOperationException("Currency not found.");

        // Clear old base
        var oldBase = await _db.Currencies.FirstOrDefaultAsync(c => c.IsBase && !c.IsDeleted && c.Id != id, ct);
        if (oldBase is not null)
        {
            // old base keeps its data but loses IsBase — direct reflection since domain exposes internal
            oldBase.RemoveBaseStatus(1m);
        }
        // Use the internal method (same assembly via InternalsVisibleTo or just call SetAsBase)
        newBase.SetAsBase();
        await _db.SaveChangesAsync(ct);
        return ToCurrencyDto(newBase);
    }

    public async Task ActivateCurrencyAsync(Guid id, CancellationToken ct = default)
    {
        var currency = await _db.Currencies.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct)
            ?? throw new InvalidOperationException("Currency not found.");
        currency.Activate();
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeactivateCurrencyAsync(Guid id, CancellationToken ct = default)
    {
        var currency = await _db.Currencies.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct)
            ?? throw new InvalidOperationException("Currency not found.");
        currency.Deactivate();
        await _db.SaveChangesAsync(ct);
    }

    // ── Mappers ───────────────────────────────────────────────────────────────

    private static FiscalCalendarDto ToFiscalCalendarDto(FiscalCalendar calendar) => new(
        calendar.Id,
        calendar.Name,
        calendar.Description,
        calendar.CalendarType,
        calendar.IsDefault,
        calendar.FiscalYears.Count(y => !y.IsDeleted),
        calendar.CreatedAt);

    private static FiscalYearDto ToFiscalYearDto(FiscalYear y)
        => ToFiscalYearDto(y, y.FiscalCalendar?.Name ?? string.Empty);

    private static FiscalYearDto ToFiscalYearDto(FiscalYear y, string calendarName) => new(
        y.Id, y.Name, y.Description, y.FiscalCalendarId, calendarName,
        y.StartDate, y.EndDate, y.CalendarType,
        y.Status.ToString(), y.PeriodCount, y.CreatedAt);

    private static FiscalPeriodDto ToPeriodDto(FiscalPeriod p) => new(
        p.Id, p.FiscalYearId, p.PeriodNumber, p.Name,
        p.StartDate, p.EndDate, p.Status.ToString());

    private static AccountDto ToAccountDto(Account a) => new(
        a.Id, a.ChartOfAccountsId, a.AccountNumber, a.Name, a.Description,
        a.AccountTypeId, a.AccountType?.Name ?? string.Empty,
        a.ParentAccountId, a.ParentAccount?.Name,
        a.IsHeaderAccount, a.AllowManualEntry,
        a.Status.ToString(), a.Currency, a.Level);

    private static FinancialDimensionDto ToFinancialDimensionDto(FinancialDimension dimension) => new(
        dimension.Id, dimension.Code, dimension.Name, dimension.Description, dimension.IsActive,
        dimension.Values.OrderBy(v => v.Code).Select(ToFinancialDimensionValueDto).ToList());

    private static FinancialDimensionValueDto ToFinancialDimensionValueDto(FinancialDimensionValue value) => new(
        value.Id, value.FinancialDimensionId, value.Code, value.Name, value.Description, value.IsActive);

    private static FinancialDimensionSetDto ToFinancialDimensionSetDto(FinancialDimensionSet set) => new(
        set.Id, set.Name, set.Description, set.IsDefault, set.IsActive,
        set.Members.OrderBy(m => m.DisplayOrder).Select(m => new FinancialDimensionSetMemberDto(
            m.FinancialDimensionId,
            m.FinancialDimension?.Code ?? string.Empty,
            m.FinancialDimension?.Name ?? string.Empty,
            m.DisplayOrder,
            m.IsRequired)).ToList());

    private static CurrencyDto ToCurrencyDto(Currency c) => new(
        c.Id, c.Code, c.Name, c.Symbol, c.DecimalPlaces, c.ExchangeRate,
        c.IsBase, c.IsActive, c.NumericCode, c.Country, c.RateUpdatedAt, c.CreatedAt);

    private static ChartOfAccountsDto ToChartOfAccountsDto(ChartOfAccounts chart) => new(
        chart.Id, chart.Code, chart.Name, chart.Description,
        chart.IsDefault, chart.IsActive, chart.Accounts.Count(a => !a.IsDeleted));

    private static LedgerDto ToLedgerDto(Ledger ledger) => new(
        ledger.Id, ledger.Code, ledger.Name, ledger.Description,
        ledger.FunctionalCurrencyId, ledger.FunctionalCurrency?.Code ?? string.Empty,
        ledger.ReportingCurrencyId, ledger.ReportingCurrency?.Code,
        ledger.FiscalCalendarId, ledger.FiscalCalendar?.Name ?? string.Empty,
        ledger.ChartOfAccountsId, ledger.ChartOfAccounts?.Name ?? string.Empty,
        ledger.IsDefault, ledger.IsActive);

    private IQueryable<GeneralLedgerParameters> ParametersQuery() =>
        _db.GeneralLedgerParameters
            .Include(p => p.DefaultLedger)
                .ThenInclude(l => l!.FiscalCalendar)
            .Include(p => p.DefaultLedger)
                .ThenInclude(l => l!.ChartOfAccounts)
            .Include(p => p.DefaultLedger)
                .ThenInclude(l => l!.FunctionalCurrency)
            .Include(p => p.DefaultFinancialDimensionSet)
            .Include(p => p.RetainedEarningsAccount)
            .Include(p => p.RoundingDifferenceAccount)
            .Include(p => p.RealizedGainAccount)
            .Include(p => p.RealizedLossAccount)
            .Include(p => p.UnrealizedGainAccount)
            .Include(p => p.UnrealizedLossAccount);

    private static GeneralLedgerParametersDto ToParametersDto(GeneralLedgerParameters p) => new(
        p.Id,
        p.DefaultLedgerId,
        p.DefaultLedger?.Code ?? string.Empty,
        p.DefaultLedger?.Name ?? string.Empty,
        p.DefaultLedger?.FiscalCalendarId ?? Guid.Empty,
        p.DefaultLedger?.FiscalCalendar?.Name ?? string.Empty,
        p.DefaultLedger?.ChartOfAccountsId ?? Guid.Empty,
        p.DefaultLedger?.ChartOfAccounts?.Name ?? string.Empty,
        p.DefaultLedger?.FunctionalCurrencyId ?? Guid.Empty,
        p.DefaultLedger?.FunctionalCurrency?.Code ?? string.Empty,
        p.DefaultFinancialDimensionSetId,
        p.DefaultFinancialDimensionSet?.Name,
        p.RetainedEarningsAccountId,
        p.RetainedEarningsAccount?.AccountNumber,
        p.RoundingDifferenceAccountId,
        p.RoundingDifferenceAccount?.AccountNumber,
        p.RealizedGainAccountId,
        p.RealizedGainAccount?.AccountNumber,
        p.RealizedLossAccountId,
        p.RealizedLossAccount?.AccountNumber,
        p.UnrealizedGainAccountId,
        p.UnrealizedGainAccount?.AccountNumber,
        p.UnrealizedLossAccountId,
        p.UnrealizedLossAccount?.AccountNumber,
        p.AllowPostingToClosedPeriods,
        p.RequireDimensionsOnJournalLines,
        p.MaximumPennyDifference,
        p.DefaultJournalType);

    private static GeneralJournalVoucherTemplateDto ToVoucherTemplateDto(
        GeneralJournalVoucherTemplate template) => new(
        template.Id,
        template.Name,
        template.LedgerId,
        template.Ledger?.Code ?? string.Empty,
        template.Description,
        template.Reference,
        template.JournalType,
        JsonSerializer.Deserialize<List<CreateJournalLineRequest>>(template.LinesJson) ?? [],
        template.CreatedAt);

    private IQueryable<AccrualScheme> AccrualSchemesQuery() =>
        _db.AccrualSchemes
            .Include(s => s.Ledger).ThenInclude(l => l!.FunctionalCurrency)
            .Include(s => s.DebitAccount)
            .Include(s => s.CreditAccount)
            .Include(s => s.FinancialDimensionSet)
            .Include(s => s.Allocations);

    private IQueryable<AccrualPostingRun> AccrualPostingRunsQuery() =>
        _db.AccrualPostingRuns
            .Include(r => r.AccrualScheme)
            .Include(r => r.Lines).ThenInclude(l => l.FiscalPeriod)
            .Include(r => r.Lines).ThenInclude(l => l.JournalEntry);

    private static AccrualSchemeDto ToAccrualSchemeDto(AccrualScheme scheme) => new(
        scheme.Id,
        scheme.Code,
        scheme.Name,
        scheme.Description,
        scheme.LedgerId,
        scheme.Ledger?.Code ?? string.Empty,
        scheme.DebitAccountId,
        scheme.DebitAccount?.AccountNumber ?? string.Empty,
        scheme.DebitAccount?.Name ?? string.Empty,
        scheme.CreditAccountId,
        scheme.CreditAccount?.AccountNumber ?? string.Empty,
        scheme.CreditAccount?.Name ?? string.Empty,
        scheme.JournalType,
        scheme.AllocationMethod.ToString(),
        scheme.DefaultPeriodCount,
        scheme.FinancialDimensionSetId,
        scheme.FinancialDimensionSet?.Name,
        JsonSerializer.Deserialize<List<Guid>>(scheme.FinancialDimensionValueIdsJson) ?? [],
        scheme.Allocations
            .OrderBy(a => a.PeriodOffset)
            .Select(a => new AccrualSchemeAllocationDto(a.PeriodOffset, a.Percentage))
            .ToList(),
        scheme.IsActive,
        scheme.CreatedAt);

    private static AccrualPostingRunDto ToAccrualPostingRunDto(AccrualPostingRun run) => new(
        run.Id,
        run.AccrualSchemeId,
        run.AccrualScheme?.Code ?? string.Empty,
        run.Reference,
        run.Description,
        run.TotalAmount,
        run.PostedAt,
        run.Lines
            .OrderBy(l => l.PeriodOffset)
            .Select(l => new AccrualPostingLineDto(
                l.FiscalPeriodId,
                l.FiscalPeriod?.Name ?? string.Empty,
                l.JournalEntryId,
                l.JournalEntry?.EntryNumber ?? string.Empty,
                l.PeriodOffset,
                l.Percentage,
                l.Amount))
            .ToList());

    private static List<decimal> CalculateEvenPercentages(int count)
    {
        var percentages = new List<decimal>(count);
        var allocated = 0m;
        for (var index = 0; index < count; index++)
        {
            var percentage = index == count - 1
                ? 100m - allocated
                : Math.Round(100m / count, 4, MidpointRounding.AwayFromZero);
            percentages.Add(percentage);
            allocated += percentage;
        }
        return percentages;
    }

    private static List<decimal> CalculateAccrualAmounts(
        decimal totalAmount,
        IReadOnlyList<decimal> percentages)
    {
        if (totalAmount <= 0)
            throw new ArgumentException("Accrual amount must be greater than zero.");
        var amounts = new List<decimal>(percentages.Count);
        var allocated = 0m;
        for (var index = 0; index < percentages.Count; index++)
        {
            var amount = index == percentages.Count - 1
                ? totalAmount - allocated
                : Math.Round(
                    totalAmount * percentages[index] / 100m,
                    4,
                    MidpointRounding.AwayFromZero);
            if (amount <= 0)
                throw new InvalidOperationException(
                    "The accrual amount is too small for the configured allocation schedule.");
            amounts.Add(amount);
            allocated += amount;
        }
        return amounts;
    }

    private static JournalEntryDto ToJournalEntryDto(JournalEntry e) => new(
        e.Id, e.LedgerId, e.Ledger?.Code ?? string.Empty,
        e.EntryNumber, e.EntryDate, e.FiscalPeriodId,
        e.FiscalPeriod?.Name ?? string.Empty,
        e.Description, e.Reference, e.JournalType,
        e.Status.ToString(), e.Currency,
        e.TotalDebit, e.TotalCredit, e.CreatedAt,
        e.Lines.OrderBy(l => l.LineOrder).Select(l => new JournalLineDto(
            l.Id, l.AccountId,
            l.Account?.AccountNumber ?? string.Empty,
            l.Account?.Name ?? string.Empty,
            l.Description, l.Debit, l.Credit, l.LineOrder,
            l.FinancialDimensionSetId,
            l.FinancialDimensionSet?.Name,
            l.DimensionValues
                .Where(dv => dv.FinancialDimensionValue?.FinancialDimension is not null)
                .OrderBy(dv => dv.FinancialDimensionValue!.FinancialDimension!.Name)
                .Select(dv => new JournalLineDimensionDto(
                    dv.FinancialDimensionValue!.FinancialDimensionId,
                    dv.FinancialDimensionValue.FinancialDimension!.Code,
                    dv.FinancialDimensionValue.FinancialDimension.Name,
                    dv.FinancialDimensionValueId,
                    dv.FinancialDimensionValue.Code,
                    dv.FinancialDimensionValue.Name))
                .ToList())).ToList());

    private async Task ValidateDimensionSelectionAsync(
        Guid? setId,
        IReadOnlyList<Guid>? selectedValueIds,
        CancellationToken ct)
    {
        var valueIds = selectedValueIds?.Distinct().ToList() ?? [];
        if (!setId.HasValue)
        {
            if (valueIds.Count > 0)
                throw new InvalidOperationException(
                    "A financial dimension set is required when dimension values are selected.");
            return;
        }

        var set = await _db.FinancialDimensionSets
            .Include(s => s.Members)
            .FirstOrDefaultAsync(s => s.Id == setId.Value && s.IsActive, ct)
            ?? throw new InvalidOperationException("Financial dimension set not found or inactive.");

        var values = await _db.FinancialDimensionValues
            .Include(v => v.FinancialDimension)
            .Where(v => valueIds.Contains(v.Id) && v.IsActive && v.FinancialDimension!.IsActive)
            .ToListAsync(ct);
        if (values.Count != valueIds.Count)
            throw new InvalidOperationException(
                "One or more financial dimension values are missing or inactive.");

        var memberIds = set.Members.Select(m => m.FinancialDimensionId).ToHashSet();
        if (values.Any(v => !memberIds.Contains(v.FinancialDimensionId)))
            throw new InvalidOperationException(
                "A selected value does not belong to the chosen financial dimension set.");
        if (values.GroupBy(v => v.FinancialDimensionId).Any(g => g.Count() > 1))
            throw new InvalidOperationException(
                "Only one value may be selected for each financial dimension.");

        var selectedDimensionIds = values.Select(v => v.FinancialDimensionId).ToHashSet();
        if (set.Members.Any(m => m.IsRequired && !selectedDimensionIds.Contains(m.FinancialDimensionId)))
            throw new InvalidOperationException(
                "All required financial dimensions must have a value.");
    }

    private Guid RequireUserId() =>
        _user.UserId ?? throw new InvalidOperationException(
            "An authenticated user is required to manage voucher templates.");
}
