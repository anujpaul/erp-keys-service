using System.Text.Json;
using System.Text.RegularExpressions;
using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Common.Services;
using ERPKeys.Domain.Modules.SystemAdmin;
using Microsoft.EntityFrameworkCore;

namespace ERPKeys.Application.Modules.SystemAdmin.Services;

public record IntegrationFieldDefinitionDto(
    string Key,
    string Label,
    string DataType,
    bool IsSecret,
    bool IsRequired,
    string? Placeholder,
    string? HelpText);

public record IntegrationConfigurationDto(
    Guid Id,
    string Code,
    string Name,
    string ServiceCategory,
    string ConnectorType,
    string? Description,
    IReadOnlyList<IntegrationFieldDefinitionDto> Fields,
    bool IsEnabled,
    bool IsConfigured,
    IReadOnlyDictionary<string, string> Settings,
    IReadOnlyDictionary<string, string> SecretPlaceholders,
    string ReviewStatus,
    string SubmittedBy,
    DateTime SubmittedAt,
    string? ReviewedBy,
    DateTime? ReviewedAt,
    string? ReviewNotes,
    bool HasPendingChange);

public record CreateIntegrationConfigurationRequest(
    string Code,
    string Name,
    string ServiceCategory,
    string ConnectorType,
    string? Description,
    List<IntegrationFieldDefinitionDto> Fields,
    bool IsEnabled = false);

public record SaveIntegrationConfigurationRequest(
    Dictionary<string, string?> Settings,
    Dictionary<string, string?> Secrets);

public record ReviewIntegrationConfigurationRequest(string? Notes);

public record ActiveIntegrationConfiguration(
    Guid Id,
    string Code,
    string ConnectorType,
    IReadOnlyDictionary<string, string> Settings,
    IReadOnlyDictionary<string, string> Secrets);

public interface IIntegrationSecretProtector
{
    string Protect(string plaintext);
    string Unprotect(string ciphertext);
}

public interface IIntegrationConfigurationReader
{
    Task<ActiveIntegrationConfiguration?> GetActiveAsync(
        string serviceCategory,
        string? connectorType = null,
        CancellationToken ct = default);
}

public interface IIntegrationConfigurationService : IIntegrationConfigurationReader
{
    Task<IReadOnlyList<IntegrationConfigurationDto>> GetAllAsync(CancellationToken ct = default);
    Task<IntegrationConfigurationDto> CreateAsync(
        CreateIntegrationConfigurationRequest request,
        CancellationToken ct = default);
    Task<IntegrationConfigurationDto> SaveAsync(
        Guid id,
        SaveIntegrationConfigurationRequest request,
        CancellationToken ct = default);
    Task<IntegrationConfigurationDto> SetEnabledAsync(
        Guid id,
        bool enabled,
        CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IntegrationConfigurationDto> ApproveAsync(
        Guid id, string? notes, CancellationToken ct = default);
    Task<IntegrationConfigurationDto> RejectAsync(
        Guid id, string? notes, CancellationToken ct = default);
}

public class IntegrationConfigurationService : IIntegrationConfigurationService
{
    private const string Mask = "xxxxxxxx";
    private static readonly Regex CodePattern =
        new("^[A-Z0-9][A-Z0-9_-]{1,49}$", RegexOptions.Compiled);
    private static readonly Regex FieldKeyPattern =
        new("^[A-Za-z][A-Za-z0-9_.-]{0,99}$", RegexOptions.Compiled);
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IAppDbContext _db;
    private readonly ICurrentOrganizationService _org;
    private readonly ICurrentUserService _user;
    private readonly IIntegrationSecretProtector _protector;
    private readonly IDocumentAuditService _audit;

    public IntegrationConfigurationService(
        IAppDbContext db,
        ICurrentOrganizationService org,
        ICurrentUserService user,
        IIntegrationSecretProtector protector,
        IDocumentAuditService audit)
    {
        _db = db;
        _org = org;
        _user = user;
        _protector = protector;
        _audit = audit;
    }

    public async Task<IReadOnlyList<IntegrationConfigurationDto>> GetAllAsync(
        CancellationToken ct = default)
    {
        var records = await _db.IntegrationConfigurations
            .AsNoTracking()
            .OrderBy(x => x.ServiceCategory)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);
        return records.Select(ToDto).ToList();
    }

    public async Task<IntegrationConfigurationDto> CreateAsync(
        CreateIntegrationConfigurationRequest request,
        CancellationToken ct = default)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (!CodePattern.IsMatch(code))
            throw new InvalidOperationException(
                "Integration code must contain 2–50 letters, numbers, hyphens, or underscores.");
        var fields = ValidateFields(request.Fields);
        if (await _db.IntegrationConfigurations.AnyAsync(x => x.Code == code, ct))
            throw new InvalidOperationException("An integration with this code already exists.");
        if (request.IsEnabled)
            throw new InvalidOperationException(
                "Create and configure the integration before enabling it.");

        var record = new IntegrationConfiguration(
            _org.OrganizationId,
            code,
            request.Name,
            request.ServiceCategory,
            request.ConnectorType,
            request.Description,
            JsonSerializer.Serialize(fields, JsonOptions),
            false,
            _user.Username);
        _db.IntegrationConfigurations.Add(record);
        _audit.Add("SysAdmin", "Integration Created", record.Id,
            nameof(IntegrationConfiguration),
            newValues: new
            {
                record.Code,
                record.Name,
                record.ServiceCategory,
                record.ConnectorType,
                Fields = fields.Select(x => new { x.Key, x.IsSecret })
            });
        await _db.SaveChangesAsync(ct);
        return ToDto(record);
    }

    public async Task<IntegrationConfigurationDto> SaveAsync(
        Guid id,
        SaveIntegrationConfigurationRequest request,
        CancellationToken ct = default)
    {
        var record = await FindAsync(id, ct);
        var settings = Normalize(request.Settings ?? new Dictionary<string, string?>());
        var suppliedSecrets = Normalize(request.Secrets ?? new Dictionary<string, string?>());
        var existingSecrets = Decrypt(record.EncryptedSecrets);
        foreach (var secret in suppliedSecrets)
            existingSecrets[secret.Key] = secret.Value;

        ValidateValues(record, settings, existingSecrets);
        var encrypted = Encrypt(existingSecrets);
        if (!record.IsConfigured)
        {
            record.ConfigureInitial(Serialize(settings), encrypted, _user.Username);
            _audit.Add("SysAdmin", "Integration Configured", record.Id,
                nameof(IntegrationConfiguration),
                newValues: new
                {
                    record.Code,
                    SecretFields = suppliedSecrets.Keys
                });
        }
        else
        {
            record.SubmitChange(Serialize(settings), encrypted, _user.Username);
            _audit.Add("SysAdmin", "Integration Change Submitted", record.Id,
                nameof(IntegrationConfiguration),
                newValues: new
                {
                    record.Code,
                    SecretFieldsChanged = suppliedSecrets.Keys
                });
        }

        await _db.SaveChangesAsync(ct);
        return ToDto(record);
    }

    public async Task<IntegrationConfigurationDto> SetEnabledAsync(
        Guid id,
        bool enabled,
        CancellationToken ct = default)
    {
        var record = await FindAsync(id, ct);
        if (enabled)
        {
            if (!record.IsConfigured)
                throw new InvalidOperationException(
                    "Configure this integration before enabling it.");
            await using var transaction = await _db.BeginTransactionAsync(ct);
            await DisableCategoryAsync(record.ServiceCategory, record.Id, ct);
            await _db.SaveChangesAsync(ct);
            record.SetEnabled(true);
            _audit.Add("SysAdmin", "Integration Enabled", record.Id,
                nameof(IntegrationConfiguration),
                newValues: new { record.Code, Enabled = true });
            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return ToDto(record);
        }
        record.SetEnabled(false);
        _audit.Add("SysAdmin", "Integration Disabled",
            record.Id,
            nameof(IntegrationConfiguration),
            newValues: new { record.Code, Enabled = false });
        await _db.SaveChangesAsync(ct);
        return ToDto(record);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var record = await FindAsync(id, ct);
        record.SoftDelete();
        _audit.Add("SysAdmin", "Integration Removed", record.Id,
            nameof(IntegrationConfiguration),
            newValues: new { record.Code, record.Name });
        await _db.SaveChangesAsync(ct);
    }

    public Task<IntegrationConfigurationDto> ApproveAsync(
        Guid id, string? notes, CancellationToken ct = default) =>
        ReviewAsync(id, true, notes, ct);

    public Task<IntegrationConfigurationDto> RejectAsync(
        Guid id, string? notes, CancellationToken ct = default) =>
        ReviewAsync(id, false, notes, ct);

    public async Task<ActiveIntegrationConfiguration?> GetActiveAsync(
        string serviceCategory,
        string? connectorType = null,
        CancellationToken ct = default)
    {
        var query = _db.IntegrationConfigurations
            .AsNoTracking()
            .Where(x => x.IsEnabled &&
                        x.IsConfigured &&
                        x.ServiceCategory == serviceCategory);
        if (!string.IsNullOrWhiteSpace(connectorType))
            query = query.Where(x => x.ConnectorType == connectorType);

        var record = await query.OrderByDescending(x => x.UpdatedAt)
            .FirstOrDefaultAsync(ct);
        if (record is null)
            return null;

        return new ActiveIntegrationConfiguration(
            record.Id,
            record.Code,
            record.ConnectorType,
            DeserializeValues(record.SettingsJson),
            Decrypt(record.EncryptedSecrets));
    }

    private async Task<IntegrationConfigurationDto> ReviewAsync(
        Guid id, bool approve, string? notes, CancellationToken ct)
    {
        var record = await FindAsync(id, ct);
        if (approve)
            record.Approve(_user.Username, notes);
        else
            record.Reject(_user.Username, notes);

        _audit.Add("SysAdmin",
            approve ? "Integration Change Approved" : "Integration Change Rejected",
            record.Id,
            nameof(IntegrationConfiguration),
            newValues: new { record.Code, Notes = notes });
        await _db.SaveChangesAsync(ct);
        return ToDto(record);
    }

    private async Task<IntegrationConfiguration> FindAsync(Guid id, CancellationToken ct) =>
        await _db.IntegrationConfigurations.FirstOrDefaultAsync(x => x.Id == id, ct)
        ?? throw new InvalidOperationException("Integration was not found.");

    private async Task DisableCategoryAsync(
        string category,
        Guid? exceptId,
        CancellationToken ct)
    {
        var enabled = await _db.IntegrationConfigurations
            .Where(x => x.ServiceCategory == category &&
                        x.IsEnabled &&
                        (!exceptId.HasValue || x.Id != exceptId.Value))
            .ToListAsync(ct);
        foreach (var item in enabled)
            item.SetEnabled(false);
    }

    private IntegrationConfigurationDto ToDto(IntegrationConfiguration record)
    {
        var activeSecrets = Decrypt(record.EncryptedSecrets);
        var pendingSecrets = Decrypt(record.PendingEncryptedSecrets);
        var secretNames = activeSecrets.Keys.Concat(pendingSecrets.Keys).Distinct();
        return new IntegrationConfigurationDto(
            record.Id,
            record.Code,
            record.Name,
            record.ServiceCategory,
            record.ConnectorType,
            record.Description,
            DeserializeFields(record.FieldDefinitionsJson),
            record.IsEnabled,
            record.IsConfigured,
            DeserializeValues(record.PendingSettingsJson ?? record.SettingsJson),
            secretNames.ToDictionary(name => name, _ => Mask),
            record.IsConfigured ? record.ReviewStatus.ToString() : "NotConfigured",
            record.SubmittedBy,
            record.SubmittedAt,
            record.ReviewedBy,
            record.ReviewedAt,
            record.ReviewNotes,
            record.PendingSettingsJson is not null);
    }

    private static IReadOnlyList<IntegrationFieldDefinitionDto> ValidateFields(
        IReadOnlyList<IntegrationFieldDefinitionDto>? fields)
    {
        if (fields is null || fields.Count == 0)
            throw new InvalidOperationException("Add at least one configuration field.");
        if (fields.Count > 30)
            throw new InvalidOperationException("An integration can have at most 30 fields.");

        var normalized = fields.Select(field =>
        {
            var key = field.Key.Trim();
            if (!FieldKeyPattern.IsMatch(key))
                throw new InvalidOperationException(
                    $"Field key '{field.Key}' is invalid.");
            if (string.IsNullOrWhiteSpace(field.Label))
                throw new InvalidOperationException($"A label is required for field '{key}'.");
            return field with
            {
                Key = key,
                Label = field.Label.Trim(),
                DataType = string.IsNullOrWhiteSpace(field.DataType)
                    ? "text"
                    : field.DataType.Trim().ToLowerInvariant()
            };
        }).ToList();

        if (normalized.Select(x => x.Key).Distinct(StringComparer.OrdinalIgnoreCase).Count()
            != normalized.Count)
            throw new InvalidOperationException("Configuration field keys must be unique.");
        return normalized;
    }

    private static void ValidateValues(
        IntegrationConfiguration record,
        IReadOnlyDictionary<string, string> settings,
        IReadOnlyDictionary<string, string> secrets)
    {
        var fields = DeserializeFields(record.FieldDefinitionsJson);
        var knownPublic = fields.Where(x => !x.IsSecret)
            .Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var knownSecrets = fields.Where(x => x.IsSecret)
            .Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (settings.Keys.Any(key => !knownPublic.Contains(key)) ||
            secrets.Keys.Any(key => !knownSecrets.Contains(key)))
            throw new InvalidOperationException(
                "Submitted values contain fields that are not defined for this integration.");

        var missing = fields.Where(field => field.IsRequired &&
            !(field.IsSecret ? secrets : settings).ContainsKey(field.Key))
            .Select(field => field.Label)
            .ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException(
                $"Complete the required field(s): {string.Join(", ", missing)}.");

        foreach (var field in fields.Where(x => !x.IsSecret && x.DataType == "url"))
        {
            if (settings.TryGetValue(field.Key, out var value) &&
                (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                 uri.Scheme != Uri.UriSchemeHttps))
                throw new InvalidOperationException(
                    $"{field.Label} must be a valid HTTPS URL.");
        }
    }

    private string? Encrypt(Dictionary<string, string> values) =>
        values.Count == 0 ? null : _protector.Protect(Serialize(values));

    private Dictionary<string, string> Decrypt(string? ciphertext) =>
        string.IsNullOrWhiteSpace(ciphertext)
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : DeserializeValues(_protector.Unprotect(ciphertext));

    private static string Serialize(IReadOnlyDictionary<string, string> values) =>
        JsonSerializer.Serialize(values, JsonOptions);

    private static Dictionary<string, string> DeserializeValues(string json) =>
        JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions)
        ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private static IReadOnlyList<IntegrationFieldDefinitionDto> DeserializeFields(string json) =>
        JsonSerializer.Deserialize<List<IntegrationFieldDefinitionDto>>(json, JsonOptions) ?? [];

    private static Dictionary<string, string> Normalize(
        IReadOnlyDictionary<string, string?> values) =>
        values
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Key) &&
                           !string.IsNullOrWhiteSpace(pair.Value) &&
                           !string.Equals(pair.Value, Mask, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                pair => pair.Key.Trim(),
                pair => pair.Value!.Trim(),
                StringComparer.OrdinalIgnoreCase);
}
