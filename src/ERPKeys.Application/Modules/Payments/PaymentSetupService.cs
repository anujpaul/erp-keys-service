using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Common.Services;
using ERPKeys.Domain.Modules.CashBank;
using ERPKeys.Domain.Modules.GeneralLedger;
using ERPKeys.Domain.Modules.Payments;
using Microsoft.EntityFrameworkCore;

namespace ERPKeys.Application.Modules.Payments;

public record PaymentProcessorConfigurationDto(
    Guid Id,
    string Code,
    string Name,
    string ProviderCode,
    string Environment,
    string EndpointBaseUrl,
    string MerchantAccountReference,
    string CredentialSecretReference,
    int TimeoutSeconds,
    bool IsActive);

public record SavePaymentProcessorConfigurationRequest(
    string Name,
    string ProviderCode,
    string Environment,
    string EndpointBaseUrl,
    string MerchantAccountReference,
    string CredentialSecretReference,
    int TimeoutSeconds = 30);

public record CreatePaymentProcessorConfigurationRequest(
    string Code,
    string Name,
    string ProviderCode,
    string Environment,
    string EndpointBaseUrl,
    string MerchantAccountReference,
    string CredentialSecretReference,
    int TimeoutSeconds = 30);

public record MethodOfPaymentDto(
    Guid Id,
    string Code,
    string Name,
    string Description,
    string Usage,
    string TenderType,
    string? CurrencyCode,
    Guid? ProcessorConfigurationId,
    string? ProcessorConfigurationName,
    Guid? SettlementBankAccountId,
    string? SettlementBankAccountName,
    Guid? ClearingAccountId,
    string? ClearingAccountNumber,
    Guid? FeeExpenseAccountId,
    string? FeeExpenseAccountNumber,
    bool RequiresExternalAuthorization,
    bool AutoCapture,
    bool AllowRefunds,
    bool AllowManualEntry,
    string SettlementMode,
    int SettlementDelayDays,
    TimeOnly? SettlementCutoffTime,
    bool IsActive);

public record SaveMethodOfPaymentRequest(
    string Name,
    string? Description,
    string Usage,
    string TenderType,
    string? CurrencyCode,
    Guid? ProcessorConfigurationId,
    Guid? SettlementBankAccountId,
    Guid? ClearingAccountId,
    Guid? FeeExpenseAccountId,
    bool RequiresExternalAuthorization,
    bool AutoCapture,
    bool AllowRefunds,
    bool AllowManualEntry,
    string SettlementMode,
    int SettlementDelayDays,
    TimeOnly? SettlementCutoffTime);

public record CreateMethodOfPaymentRequest(
    string Code,
    string Name,
    string? Description,
    string Usage,
    string TenderType,
    string? CurrencyCode,
    Guid? ProcessorConfigurationId,
    Guid? SettlementBankAccountId,
    Guid? ClearingAccountId,
    Guid? FeeExpenseAccountId,
    bool RequiresExternalAuthorization,
    bool AutoCapture,
    bool AllowRefunds = true,
    bool AllowManualEntry = true,
    string SettlementMode = "None",
    int SettlementDelayDays = 0,
    TimeOnly? SettlementCutoffTime = null);

public interface IPaymentSetupService
{
    Task<IReadOnlyList<PaymentProcessorConfigurationDto>> GetProcessorsAsync(
        bool activeOnly, CancellationToken ct = default);
    Task<PaymentProcessorConfigurationDto> CreateProcessorAsync(
        CreatePaymentProcessorConfigurationRequest request, CancellationToken ct = default);
    Task<PaymentProcessorConfigurationDto> UpdateProcessorAsync(
        Guid id, SavePaymentProcessorConfigurationRequest request, CancellationToken ct = default);
    Task SetProcessorActiveAsync(Guid id, bool active, CancellationToken ct = default);

    Task<IReadOnlyList<MethodOfPaymentDto>> GetMethodsAsync(
        bool activeOnly, string? usage, CancellationToken ct = default);
    Task<MethodOfPaymentDto> CreateMethodAsync(
        CreateMethodOfPaymentRequest request, CancellationToken ct = default);
    Task<MethodOfPaymentDto> UpdateMethodAsync(
        Guid id, SaveMethodOfPaymentRequest request, CancellationToken ct = default);
    Task SetMethodActiveAsync(Guid id, bool active, CancellationToken ct = default);
}

public class PaymentSetupService : IPaymentSetupService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentOrganizationService _org;
    private readonly IDocumentAuditService _audit;

    public PaymentSetupService(
        IAppDbContext db,
        ICurrentOrganizationService org,
        IDocumentAuditService audit)
    {
        _db = db;
        _org = org;
        _audit = audit;
    }

    public async Task<IReadOnlyList<PaymentProcessorConfigurationDto>> GetProcessorsAsync(
        bool activeOnly, CancellationToken ct = default)
    {
        var query = _db.PaymentProcessorConfigurations.AsNoTracking();
        if (activeOnly) query = query.Where(p => p.IsActive);
        var processors = await query.OrderBy(p => p.Name).ToListAsync(ct);
        return processors.Select(ToProcessorDto).ToList();
    }

    public async Task<PaymentProcessorConfigurationDto> CreateProcessorAsync(
        CreatePaymentProcessorConfigurationRequest request,
        CancellationToken ct = default)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _db.PaymentProcessorConfigurations.AnyAsync(p => p.Code == code, ct))
            throw new InvalidOperationException($"Payment processor '{code}' already exists.");

        var processor = new PaymentProcessorConfiguration(
            _org.OrganizationId,
            code,
            request.Name,
            request.ProviderCode,
            ParseEnum<PaymentProcessorEnvironment>(request.Environment, "processor environment"),
            request.EndpointBaseUrl,
            request.MerchantAccountReference,
            request.CredentialSecretReference,
            request.TimeoutSeconds);
        _db.PaymentProcessorConfigurations.Add(processor);
        _audit.Add("Payments", "Create", processor.Id,
            nameof(PaymentProcessorConfiguration), newValues: ToProcessorDto(processor));
        await _db.SaveChangesAsync(ct);
        return ToProcessorDto(processor);
    }

    public async Task<PaymentProcessorConfigurationDto> UpdateProcessorAsync(
        Guid id,
        SavePaymentProcessorConfigurationRequest request,
        CancellationToken ct = default)
    {
        var processor = await _db.PaymentProcessorConfigurations
            .FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new InvalidOperationException("Payment processor configuration not found.");
        var oldValues = ToProcessorDto(processor);
        processor.Update(
            request.Name,
            request.ProviderCode,
            ParseEnum<PaymentProcessorEnvironment>(request.Environment, "processor environment"),
            request.EndpointBaseUrl,
            request.MerchantAccountReference,
            request.CredentialSecretReference,
            request.TimeoutSeconds);
        _audit.Add("Payments", "Update", processor.Id,
            nameof(PaymentProcessorConfiguration), oldValues, ToProcessorDto(processor));
        await _db.SaveChangesAsync(ct);
        return ToProcessorDto(processor);
    }

    public async Task SetProcessorActiveAsync(
        Guid id, bool active, CancellationToken ct = default)
    {
        var processor = await _db.PaymentProcessorConfigurations
            .FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new InvalidOperationException("Payment processor configuration not found.");
        if (!active && await _db.MethodsOfPayment.AnyAsync(
            m => m.ProcessorConfigurationId == id && m.IsActive, ct))
            throw new InvalidOperationException(
                "Deactivate payment methods that use this processor first.");

        var oldValues = new { processor.IsActive };
        if (active) processor.Activate(); else processor.Deactivate();
        _audit.Add("Payments", active ? "Activate" : "Deactivate", processor.Id,
            nameof(PaymentProcessorConfiguration), oldValues,
            new { processor.IsActive });
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<MethodOfPaymentDto>> GetMethodsAsync(
        bool activeOnly, string? usage, CancellationToken ct = default)
    {
        var query = MethodQuery().AsNoTracking();
        if (activeOnly) query = query.Where(m => m.IsActive);
        if (!string.IsNullOrWhiteSpace(usage))
        {
            var parsedUsage = ParseEnum<PaymentUsage>(usage, "payment usage");
            query = query.Where(m => m.Usage == parsedUsage || m.Usage == PaymentUsage.Both);
        }
        var methods = await query.OrderBy(m => m.Code).ToListAsync(ct);
        return methods.Select(ToMethodDto).ToList();
    }

    public async Task<MethodOfPaymentDto> CreateMethodAsync(
        CreateMethodOfPaymentRequest request,
        CancellationToken ct = default)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _db.MethodsOfPayment.AnyAsync(m => m.Code == code, ct))
            throw new InvalidOperationException($"Method of payment '{code}' already exists.");

        var usage = ParseEnum<PaymentUsage>(request.Usage, "payment usage");
        var tenderType = ParseEnum<PaymentTenderType>(request.TenderType, "tender type");
        var method = new MethodOfPayment(
            _org.OrganizationId, code, request.Name, usage, tenderType);
        await ValidateAndUpdateMethodAsync(method, new SaveMethodOfPaymentRequest(
            request.Name,
            request.Description,
            request.Usage,
            request.TenderType,
            request.CurrencyCode,
            request.ProcessorConfigurationId,
            request.SettlementBankAccountId,
            request.ClearingAccountId,
            request.FeeExpenseAccountId,
            request.RequiresExternalAuthorization,
            request.AutoCapture,
            request.AllowRefunds,
            request.AllowManualEntry,
            request.SettlementMode,
            request.SettlementDelayDays,
            request.SettlementCutoffTime), ct);
        _db.MethodsOfPayment.Add(method);
        _audit.Add("Payments", "Create", method.Id,
            nameof(MethodOfPayment), newValues: ToMethodDto(method));
        await _db.SaveChangesAsync(ct);
        return await LoadMethodDtoAsync(method.Id, ct);
    }

    public async Task<MethodOfPaymentDto> UpdateMethodAsync(
        Guid id, SaveMethodOfPaymentRequest request, CancellationToken ct = default)
    {
        var method = await MethodQuery().FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new InvalidOperationException("Method of payment not found.");
        var oldValues = ToMethodDto(method);
        await ValidateAndUpdateMethodAsync(method, request, ct);
        _audit.Add("Payments", "Update", method.Id,
            nameof(MethodOfPayment), oldValues, ToMethodDto(method));
        await _db.SaveChangesAsync(ct);
        return await LoadMethodDtoAsync(method.Id, ct);
    }

    public async Task SetMethodActiveAsync(
        Guid id, bool active, CancellationToken ct = default)
    {
        var method = await _db.MethodsOfPayment.FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new InvalidOperationException("Method of payment not found.");
        var oldValues = new { method.IsActive };
        if (active) method.Activate(); else method.Deactivate();
        _audit.Add("Payments", active ? "Activate" : "Deactivate", method.Id,
            nameof(MethodOfPayment), oldValues, new { method.IsActive });
        await _db.SaveChangesAsync(ct);
    }

    private async Task ValidateAndUpdateMethodAsync(
        MethodOfPayment method,
        SaveMethodOfPaymentRequest request,
        CancellationToken ct)
    {
        var usage = ParseEnum<PaymentUsage>(request.Usage, "payment usage");
        var tenderType = ParseEnum<PaymentTenderType>(request.TenderType, "tender type");
        var settlementMode = ParseEnum<PaymentSettlementMode>(
            request.SettlementMode, "settlement mode");

        if (!string.IsNullOrWhiteSpace(request.CurrencyCode))
        {
            var currency = request.CurrencyCode.Trim().ToUpperInvariant();
            if (!await _db.Currencies.AnyAsync(c => c.Code == currency && c.IsActive, ct))
                throw new InvalidOperationException($"Active currency '{currency}' was not found.");
        }

        if (request.ProcessorConfigurationId.HasValue &&
            !await _db.PaymentProcessorConfigurations.AnyAsync(
                p => p.Id == request.ProcessorConfigurationId && p.IsActive, ct))
            throw new InvalidOperationException("Payment processor configuration not found or inactive.");

        if (request.SettlementBankAccountId.HasValue)
        {
            var bankAccount = await _db.BankAccounts.FirstOrDefaultAsync(
                b => b.Id == request.SettlementBankAccountId &&
                     b.AccountStatus == BankAccountStatus.Active, ct)
                ?? throw new InvalidOperationException(
                    "Settlement bank account not found or inactive.");
            if (!string.IsNullOrWhiteSpace(request.CurrencyCode) &&
                !string.Equals(bankAccount.Currency, request.CurrencyCode,
                    StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    "Settlement bank account currency must match the payment method currency.");
        }

        await ValidatePostingAccountAsync(request.ClearingAccountId, "Clearing", false, ct);
        await ValidatePostingAccountAsync(request.FeeExpenseAccountId, "Fee expense", true, ct);

        method.Update(
            request.Name,
            request.Description,
            usage,
            tenderType,
            request.CurrencyCode,
            request.ProcessorConfigurationId,
            request.SettlementBankAccountId,
            request.ClearingAccountId,
            request.FeeExpenseAccountId,
            request.RequiresExternalAuthorization,
            request.AutoCapture,
            request.AllowRefunds,
            request.AllowManualEntry,
            settlementMode,
            request.SettlementDelayDays,
            request.SettlementCutoffTime);
    }

    private async Task ValidatePostingAccountAsync(
        Guid? accountId, string label, bool requireExpense, CancellationToken ct)
    {
        if (!accountId.HasValue) return;
        var account = await _db.Accounts.Include(a => a.AccountType)
            .FirstOrDefaultAsync(a =>
                a.Id == accountId &&
                a.Status == AccountStatus.Active &&
                !a.IsHeaderAccount &&
                a.AllowManualEntry, ct)
            ?? throw new InvalidOperationException(
                $"{label} account must be an active posting account.");
        if (requireExpense &&
            !string.Equals(account.AccountType?.Code, "EXPENSE", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Fee expense account must use the Expense account type.");
    }

    private IQueryable<MethodOfPayment> MethodQuery() =>
        _db.MethodsOfPayment
            .Include(m => m.ProcessorConfiguration)
            .Include(m => m.SettlementBankAccount)
            .Include(m => m.ClearingAccount)
            .Include(m => m.FeeExpenseAccount);

    private async Task<MethodOfPaymentDto> LoadMethodDtoAsync(
        Guid id, CancellationToken ct) =>
        ToMethodDto(await MethodQuery().SingleAsync(m => m.Id == id, ct));

    private static PaymentProcessorConfigurationDto ToProcessorDto(
        PaymentProcessorConfiguration p) => new(
        p.Id, p.Code, p.Name, p.ProviderCode, p.Environment.ToString(),
        p.EndpointBaseUrl, p.MerchantAccountReference,
        p.CredentialSecretReference, p.TimeoutSeconds, p.IsActive);

    private static MethodOfPaymentDto ToMethodDto(MethodOfPayment m) => new(
        m.Id, m.Code, m.Name, m.Description, m.Usage.ToString(),
        m.TenderType.ToString(), m.CurrencyCode,
        m.ProcessorConfigurationId, m.ProcessorConfiguration?.Name,
        m.SettlementBankAccountId, m.SettlementBankAccount?.AccountName,
        m.ClearingAccountId, m.ClearingAccount?.AccountNumber,
        m.FeeExpenseAccountId, m.FeeExpenseAccount?.AccountNumber,
        m.RequiresExternalAuthorization, m.AutoCapture, m.AllowRefunds,
        m.AllowManualEntry, m.SettlementMode.ToString(),
        m.SettlementDelayDays, m.SettlementCutoffTime,
        m.IsActive);

    private static T ParseEnum<T>(string value, string field) where T : struct, Enum
    {
        if (!Enum.TryParse<T>(value, true, out var result) ||
            !Enum.IsDefined(result))
            throw new ArgumentException(
                $"Invalid {field}. Valid values: {string.Join(", ", Enum.GetNames<T>())}.");
        return result;
    }
}
