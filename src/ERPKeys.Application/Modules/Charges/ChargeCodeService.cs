using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Common.Services;
using ERPKeys.Domain.Modules.GeneralLedger;
using Microsoft.EntityFrameworkCore;

namespace ERPKeys.Application.Modules.Charges;

public record ChargeCodeDto(
    Guid Id,
    string Code,
    string Name,
    string Description,
    string CalculationMethod,
    decimal DefaultValue,
    string? CurrencyCode,
    bool IsTaxable,
    Guid PostingAccountId,
    string PostingAccountNumber,
    string PostingAccountName,
    bool IsActive);

public record CreateChargeCodeRequest(
    string Code,
    string Name,
    string? Description,
    string CalculationMethod,
    decimal DefaultValue,
    string? CurrencyCode,
    bool IsTaxable,
    Guid PostingAccountId);

public record UpdateChargeCodeRequest(
    string Name,
    string? Description,
    string CalculationMethod,
    decimal DefaultValue,
    string? CurrencyCode,
    bool IsTaxable,
    Guid PostingAccountId);

public record ChargePostingAccountOptionDto(
    Guid Id, string AccountNumber, string Name);

public record ChargeCurrencyOptionDto(
    string Code, string Name);

public record ChargeCodeOptionsDto(
    IReadOnlyList<ChargePostingAccountOptionDto> PostingAccounts,
    IReadOnlyList<ChargeCurrencyOptionDto> Currencies);

public interface IChargeCodeService
{
    Task<ChargeCodeOptionsDto> GetOptionsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ChargeCodeDto>> GetAsync(
        ChargeModule module, bool activeOnly, CancellationToken ct = default);
    Task<ChargeCodeDto> CreateAsync(
        ChargeModule module, CreateChargeCodeRequest request, CancellationToken ct = default);
    Task<ChargeCodeDto> UpdateAsync(
        ChargeModule module, Guid id, UpdateChargeCodeRequest request, CancellationToken ct = default);
    Task SetActiveAsync(
        ChargeModule module, Guid id, bool active, CancellationToken ct = default);
}

public class ChargeCodeService : IChargeCodeService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentOrganizationService _organization;
    private readonly IDocumentAuditService _audit;

    public ChargeCodeService(
        IAppDbContext db,
        ICurrentOrganizationService organization,
        IDocumentAuditService audit)
    {
        _db = db;
        _organization = organization;
        _audit = audit;
    }

    public async Task<IReadOnlyList<ChargeCodeDto>> GetAsync(
        ChargeModule module, bool activeOnly, CancellationToken ct = default)
    {
        var query = Query().AsNoTracking().Where(charge => charge.Module == module);
        if (activeOnly)
            query = query.Where(charge => charge.IsActive);
        var charges = await query.OrderBy(charge => charge.Code).ToListAsync(ct);
        return charges.Select(ToDto).ToList();
    }

    public async Task<ChargeCodeOptionsDto> GetOptionsAsync(
        CancellationToken ct = default)
    {
        var accounts = await _db.Accounts
            .AsNoTracking()
            .Where(account =>
                account.Status == AccountStatus.Active &&
                !account.IsHeaderAccount &&
                account.AllowManualEntry)
            .OrderBy(account => account.AccountNumber)
            .Select(account => new ChargePostingAccountOptionDto(
                account.Id, account.AccountNumber, account.Name))
            .ToListAsync(ct);
        var currencies = await _db.Currencies
            .AsNoTracking()
            .Where(currency => currency.IsActive)
            .OrderBy(currency => currency.Code)
            .Select(currency => new ChargeCurrencyOptionDto(
                currency.Code, currency.Name))
            .ToListAsync(ct);
        return new ChargeCodeOptionsDto(accounts, currencies);
    }

    public async Task<ChargeCodeDto> CreateAsync(
        ChargeModule module,
        CreateChargeCodeRequest request,
        CancellationToken ct = default)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _db.ChargeCodes.AnyAsync(
                charge => charge.Module == module && charge.Code == code, ct))
            throw new InvalidOperationException(
                $"Charge code '{code}' already exists for {ModuleLabel(module)}.");

        await ValidateReferencesAsync(
            request.PostingAccountId, request.CurrencyCode,
            ParseCalculationMethod(request.CalculationMethod), ct);

        var charge = new ChargeCode(
            _organization.OrganizationId,
            module,
            code,
            request.Name,
            request.Description,
            ParseCalculationMethod(request.CalculationMethod),
            request.DefaultValue,
            request.CurrencyCode,
            request.IsTaxable,
            request.PostingAccountId);
        _db.ChargeCodes.Add(charge);
        _audit.Add(ModuleLabel(module), "Create", charge.Id,
            nameof(ChargeCode), newValues: request);
        await _db.SaveChangesAsync(ct);
        return await LoadDtoAsync(module, charge.Id, ct);
    }

    public async Task<ChargeCodeDto> UpdateAsync(
        ChargeModule module,
        Guid id,
        UpdateChargeCodeRequest request,
        CancellationToken ct = default)
    {
        var charge = await Query()
            .FirstOrDefaultAsync(item => item.Id == id && item.Module == module, ct)
            ?? throw new InvalidOperationException("Charge code not found.");
        var oldValues = ToDto(charge);
        var calculationMethod = ParseCalculationMethod(request.CalculationMethod);
        await ValidateReferencesAsync(
            request.PostingAccountId, request.CurrencyCode, calculationMethod, ct);
        charge.Update(
            request.Name,
            request.Description,
            calculationMethod,
            request.DefaultValue,
            request.CurrencyCode,
            request.IsTaxable,
            request.PostingAccountId);
        _audit.Add(ModuleLabel(module), "Update", charge.Id,
            nameof(ChargeCode), oldValues, request);
        await _db.SaveChangesAsync(ct);
        return await LoadDtoAsync(module, charge.Id, ct);
    }

    public async Task SetActiveAsync(
        ChargeModule module,
        Guid id,
        bool active,
        CancellationToken ct = default)
    {
        var charge = await _db.ChargeCodes
            .FirstOrDefaultAsync(item => item.Id == id && item.Module == module, ct)
            ?? throw new InvalidOperationException("Charge code not found.");
        var oldValues = new { charge.IsActive };
        if (active) charge.Activate(); else charge.Deactivate();
        _audit.Add(ModuleLabel(module), active ? "Activate" : "Deactivate",
            charge.Id, nameof(ChargeCode), oldValues, new { charge.IsActive });
        await _db.SaveChangesAsync(ct);
    }

    private async Task ValidateReferencesAsync(
        Guid postingAccountId,
        string? currencyCode,
        ChargeCalculationMethod calculationMethod,
        CancellationToken ct)
    {
        var validAccount = await _db.Accounts.AnyAsync(account =>
            account.Id == postingAccountId &&
            account.Status == AccountStatus.Active &&
            !account.IsHeaderAccount &&
            account.AllowManualEntry, ct);
        if (!validAccount)
            throw new InvalidOperationException(
                "Posting account must be an active, non-header posting account.");

        if (calculationMethod == ChargeCalculationMethod.FixedAmount &&
            !string.IsNullOrWhiteSpace(currencyCode))
        {
            var normalizedCurrency = currencyCode.Trim().ToUpperInvariant();
            if (!await _db.Currencies.AnyAsync(
                    currency => currency.Code == normalizedCurrency && currency.IsActive, ct))
                throw new InvalidOperationException(
                    $"Active currency '{normalizedCurrency}' was not found.");
        }
    }

    private IQueryable<ChargeCode> Query() =>
        _db.ChargeCodes.Include(charge => charge.PostingAccount);

    private async Task<ChargeCodeDto> LoadDtoAsync(
        ChargeModule module, Guid id, CancellationToken ct) =>
        ToDto(await Query().SingleAsync(
            charge => charge.Id == id && charge.Module == module, ct));

    private static ChargeCalculationMethod ParseCalculationMethod(string value)
    {
        if (!Enum.TryParse<ChargeCalculationMethod>(value, true, out var result) ||
            !Enum.IsDefined(result))
            throw new ArgumentException(
                "Invalid calculation method. Valid values: FixedAmount, Percentage.");
        return result;
    }

    private static ChargeCodeDto ToDto(ChargeCode charge) => new(
        charge.Id,
        charge.Code,
        charge.Name,
        charge.Description,
        charge.CalculationMethod.ToString(),
        charge.DefaultValue,
        charge.CurrencyCode,
        charge.IsTaxable,
        charge.PostingAccountId,
        charge.PostingAccount?.AccountNumber ?? string.Empty,
        charge.PostingAccount?.Name ?? string.Empty,
        charge.IsActive);

    private static string ModuleLabel(ChargeModule module) =>
        module == ChargeModule.AccountsPayable ? "AP" : "AR";
}
