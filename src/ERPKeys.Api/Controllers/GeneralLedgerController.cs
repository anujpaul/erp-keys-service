using ERPKeys.Application.Modules.GeneralLedger.DTOs;
using ERPKeys.Application.Modules.GeneralLedger.Services;
using ERPKeys.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPKeys.Api.Controllers;

[Authorize]
[Authorize(Policy = PermissionKeys.GlAccess)]
[ApiController]
[Route("api/gl")]
[Produces("application/json")]
public class GeneralLedgerController : ControllerBase
{
    private readonly IGeneralLedgerService _svc;
    public GeneralLedgerController(IGeneralLedgerService svc) => _svc = svc;

    [HttpGet("charts-of-accounts")]
    public async Task<IActionResult> GetChartsOfAccounts(CancellationToken ct)
        => Ok(await _svc.GetChartsOfAccountsAsync(ct));

    [HttpPost("charts-of-accounts")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> CreateChartOfAccounts(
        [FromBody] CreateChartOfAccountsRequest req, CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateChartOfAccountsAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("ledgers")]
    public async Task<IActionResult> GetLedgers(CancellationToken ct)
        => Ok(await _svc.GetLedgersAsync(ct));

    [HttpPost("ledgers")]
    public async Task<IActionResult> CreateLedger(
        [FromBody] CreateLedgerRequest req, CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateLedgerAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("ledgers/{id:guid}/set-default")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> SetDefaultLedger(Guid id, CancellationToken ct)
    {
        try { await _svc.SetDefaultLedgerAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("parameters")]
    public async Task<IActionResult> GetParameters(CancellationToken ct)
    {
        var parameters = await _svc.GetParametersAsync(ct);
        return parameters is null ? NotFound() : Ok(parameters);
    }

    [HttpPut("parameters")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> UpdateParameters(
        [FromBody] UpdateGeneralLedgerParametersRequest req,
        CancellationToken ct)
    {
        try { return Ok(await _svc.UpdateParametersAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Fiscal Calendar ───────────────────────────────────────────────────────

    [HttpGet("fiscal-calendars")]
    public async Task<IActionResult> GetFiscalCalendars(CancellationToken ct)
        => Ok(await _svc.GetFiscalCalendarsAsync(ct));

    [HttpPost("fiscal-calendars")]
    public async Task<IActionResult> CreateFiscalCalendar(
        [FromBody] CreateFiscalCalendarRequest req,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateFiscalCalendarAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("fiscal-calendars/{id:guid}/set-default")]
    public async Task<IActionResult> SetDefaultFiscalCalendar(Guid id, CancellationToken ct)
    {
        try { await _svc.SetDefaultFiscalCalendarAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("fiscal-years")]
    public async Task<IActionResult> GetFiscalYears([FromQuery] Guid? fiscalCalendarId, CancellationToken ct)
        => Ok(await _svc.GetFiscalYearsAsync(fiscalCalendarId, ct));

    [HttpGet("fiscal-years/{id:guid}")]
    public async Task<IActionResult> GetFiscalYear(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetFiscalYearAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("fiscal-years")]
    public async Task<IActionResult> CreateFiscalYear([FromBody] CreateFiscalYearRequest req, CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateFiscalYearAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("fiscal-years/{id:guid}/close")]
    public async Task<IActionResult> CloseFiscalYear(Guid id, CancellationToken ct)
    {
        try { await _svc.CloseFiscalYearAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("fiscal-years/{fyId:guid}/periods")]
    [HttpGet("/api/fiscal-years/{fyId:guid}/periods")]
    public async Task<IActionResult> GetPeriods(Guid fyId, CancellationToken ct)
        => Ok(await _svc.GetPeriodsAsync(fyId, ct));

    [HttpPost("fiscal-years/{fyId:guid}/periods")]
    public async Task<IActionResult> CreatePeriod(Guid fyId, [FromBody] CreatePeriodRequest req, CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreatePeriodAsync(fyId, req, ct)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("fiscal-years/{fyId:guid}/periods/generate")]
    [HttpPost("/api/fiscal-years/{fyId:guid}/generate-periods")]
    public async Task<IActionResult> GeneratePeriods(Guid fyId, [FromBody] GeneratePeriodsRequest req, CancellationToken ct)
    {
        try { return Ok(await _svc.GeneratePeriodsAsync(fyId, req, ct)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("fiscal-years/{fyId:guid}/periods/{periodId:guid}")]
    public async Task<IActionResult> UpdatePeriod(Guid fyId, Guid periodId, [FromBody] UpdatePeriodRequest req, CancellationToken ct)
    {
        try { return Ok(await _svc.UpdatePeriodAsync(fyId, periodId, req, ct)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("fiscal-years/{fyId:guid}/periods/{periodId:guid}")]
    public async Task<IActionResult> DeletePeriod(Guid fyId, Guid periodId, CancellationToken ct)
    {
        try { await _svc.DeletePeriodAsync(fyId, periodId, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("periods/{id:guid}/close")]
    public async Task<IActionResult> ClosePeriod(Guid id, CancellationToken ct)
    {
        try { await _svc.ClosePeriodAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPatch("fiscal-periods/{id:guid}/status")]
    [HttpPatch("/api/fiscal-periods/{id:guid}/status")]
    public async Task<IActionResult> UpdatePeriodStatus(
        Guid id,
        [FromBody] UpdateFiscalPeriodStatusRequest req,
        CancellationToken ct)
    {
        try
        {
            var result = await _svc.UpdatePeriodStatusAsync(id, req, ct);
            return result.Warning is null ? Ok(result) : Conflict(result);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("periods/current")]
    public async Task<IActionResult> GetCurrentPeriod(CancellationToken ct)
    {
        var dto = await _svc.GetCurrentPeriodAsync(ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    // ── Chart of Accounts ─────────────────────────────────────────────────────

    [HttpGet("account-types")]
    public async Task<IActionResult> GetAccountTypes(CancellationToken ct)
        => Ok(await _svc.GetAccountTypesAsync(ct));

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts(
        [FromQuery] Guid? chartOfAccountsId, CancellationToken ct)
        => Ok(await _svc.GetAccountsAsync(chartOfAccountsId, ct));

    [HttpPost("accounts")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest req, CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateAccountAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("accounts/{id:guid}/deactivate")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> DeactivateAccount(Guid id, CancellationToken ct)
    {
        try { await _svc.DeactivateAccountAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Journal Entries ───────────────────────────────────────────────────────

    [HttpGet("financial-dimensions")]
    public async Task<IActionResult> GetFinancialDimensions(CancellationToken ct)
        => Ok(await _svc.GetFinancialDimensionsAsync(ct));

    [HttpPost("financial-dimensions")]
    public async Task<IActionResult> CreateFinancialDimension(
        [FromBody] CreateFinancialDimensionRequest req,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateFinancialDimensionAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("financial-dimensions/{id:guid}/values")]
    public async Task<IActionResult> CreateFinancialDimensionValue(
        Guid id,
        [FromBody] CreateFinancialDimensionValueRequest req,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateFinancialDimensionValueAsync(id, req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("financial-dimensions/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateFinancialDimension(Guid id, CancellationToken ct)
    {
        try { await _svc.DeactivateFinancialDimensionAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("financial-dimension-values/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateFinancialDimensionValue(Guid id, CancellationToken ct)
    {
        try { await _svc.DeactivateFinancialDimensionValueAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("financial-dimension-sets")]
    public async Task<IActionResult> GetFinancialDimensionSets(CancellationToken ct)
        => Ok(await _svc.GetFinancialDimensionSetsAsync(ct));

    [HttpPost("financial-dimension-sets")]
    public async Task<IActionResult> CreateFinancialDimensionSet(
        [FromBody] CreateFinancialDimensionSetRequest req,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateFinancialDimensionSetAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("financial-dimension-sets/{id:guid}/set-default")]
    public async Task<IActionResult> SetDefaultFinancialDimensionSet(Guid id, CancellationToken ct)
    {
        try { await _svc.SetDefaultFinancialDimensionSetAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("financial-dimension-sets/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateFinancialDimensionSet(Guid id, CancellationToken ct)
    {
        try { await _svc.DeactivateFinancialDimensionSetAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("journal-entries")]
    public async Task<IActionResult> GetJournalEntries([FromQuery] Guid? fiscalPeriodId, CancellationToken ct)
        => Ok(await _svc.GetJournalEntriesAsync(fiscalPeriodId, ct));

    [HttpGet("journal-entries/{id:guid}")]
    public async Task<IActionResult> GetJournalEntry(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetJournalEntryAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("journal-entries")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> CreateJournalEntry([FromBody] CreateJournalEntryRequest req, CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateJournalEntryAsync(req, ct)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("journal-entries/post")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    [Authorize(Policy = PermissionKeys.GlJournalPost)]
    public async Task<IActionResult> CreateAndPostJournalEntry(
        [FromBody] CreateJournalEntryRequest req,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateAndPostJournalEntryAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("journal-entries/{id:guid}")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> UpdateDraftJournalEntry(
        Guid id,
        [FromBody] CreateJournalEntryRequest req,
        CancellationToken ct)
    {
        try { return Ok(await _svc.UpdateDraftJournalEntryAsync(id, req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("journal-entries/{id:guid}/post")]
    [Authorize(Policy = PermissionKeys.GlJournalPost)]
    public async Task<IActionResult> PostJournalEntry(Guid id, CancellationToken ct)
    {
        try { await _svc.PostJournalEntryAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("journal-entries/{id:guid}/void")]
    [Authorize(Policy = PermissionKeys.GlJournalPost)]
    public async Task<IActionResult> VoidJournalEntry(Guid id, CancellationToken ct)
    {
        try { await _svc.VoidJournalEntryAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("voucher-templates")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> GetVoucherTemplates(CancellationToken ct)
        => Ok(await _svc.GetVoucherTemplatesAsync(ct));

    [HttpPost("voucher-templates")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> SaveVoucherTemplate(
        [FromBody] SaveGeneralJournalVoucherTemplateRequest req,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.SaveVoucherTemplateAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("voucher-templates/{id:guid}")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> DeleteVoucherTemplate(Guid id, CancellationToken ct)
    {
        try { await _svc.DeleteVoucherTemplateAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("accrual-schemes")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> GetAccrualSchemes(CancellationToken ct)
        => Ok(await _svc.GetAccrualSchemesAsync(ct));

    [HttpPost("accrual-schemes")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> CreateAccrualScheme(
        [FromBody] CreateAccrualSchemeRequest req,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateAccrualSchemeAsync(req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("accrual-schemes/{id:guid}/deactivate")]
    [Authorize(Policy = PermissionKeys.GlJournalManage)]
    public async Task<IActionResult> DeactivateAccrualScheme(Guid id, CancellationToken ct)
    {
        try { await _svc.DeactivateAccrualSchemeAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("accrual-posting-runs")]
    [Authorize(Policy = PermissionKeys.GlJournalView)]
    public async Task<IActionResult> GetAccrualPostingRuns(
        [FromQuery] Guid? schemeId,
        CancellationToken ct)
        => Ok(await _svc.GetAccrualPostingRunsAsync(schemeId, ct));

    [HttpPost("accrual-schemes/{id:guid}/post")]
    [Authorize(Policy = PermissionKeys.GlJournalPost)]
    public async Task<IActionResult> PostAccrualScheme(
        Guid id,
        [FromBody] PostAccrualSchemeRequest req,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.PostAccrualSchemeAsync(id, req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("reports/trial-balance")]
    public async Task<IActionResult> GetTrialBalance([FromQuery] Guid fiscalPeriodId, CancellationToken ct)
        => Ok(await _svc.GetTrialBalanceAsync(fiscalPeriodId, ct));

    // ── Currencies ────────────────────────────────────────────────────────────

    [HttpGet("currencies")]
    public async Task<IActionResult> GetCurrencies([FromQuery] bool activeOnly = false, CancellationToken ct = default)
        => Ok(await _svc.GetCurrenciesAsync(activeOnly, ct));

    [HttpGet("currencies/base")]
    public async Task<IActionResult> GetBaseCurrency(CancellationToken ct)
    {
        var dto = await _svc.GetBaseCurrencyAsync(ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("currencies/{id:guid}")]
    public async Task<IActionResult> GetCurrency(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetCurrencyAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("currencies")]
    public async Task<IActionResult> CreateCurrency([FromBody] CreateCurrencyRequest req, CancellationToken ct)
    {
        try { return StatusCode(201, await _svc.CreateCurrencyAsync(req, ct)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("currencies/{id:guid}")]
    public async Task<IActionResult> UpdateCurrency(Guid id, [FromBody] UpdateCurrencyRequest req, CancellationToken ct)
    {
        try { return Ok(await _svc.UpdateCurrencyAsync(id, req, ct)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPatch("currencies/{id:guid}/exchange-rate")]
    public async Task<IActionResult> UpdateExchangeRate(Guid id, [FromBody] UpdateExchangeRateRequest req, CancellationToken ct)
    {
        try { return Ok(await _svc.UpdateExchangeRateAsync(id, req, ct)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("currencies/{id:guid}/set-base")]
    public async Task<IActionResult> SetBaseCurrency(Guid id, CancellationToken ct)
    {
        try { return Ok(await _svc.SetBaseCurrencyAsync(id, ct)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("currencies/{id:guid}/activate")]
    public async Task<IActionResult> ActivateCurrency(Guid id, CancellationToken ct)
    {
        try { await _svc.ActivateCurrencyAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("currencies/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateCurrency(Guid id, CancellationToken ct)
    {
        try { await _svc.DeactivateCurrencyAsync(id, ct); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
