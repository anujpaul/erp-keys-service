using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.SystemAdmin;

public enum IntegrationReviewStatus { Active, PendingReview, Rejected }

public class IntegrationConfiguration : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string ServiceCategory { get; private set; } = string.Empty;
    public string ConnectorType { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string FieldDefinitionsJson { get; private set; } = "[]";
    public bool IsEnabled { get; private set; }
    public bool IsConfigured { get; private set; }
    public string SettingsJson { get; private set; } = "{}";
    public string? EncryptedSecrets { get; private set; }
    public string? PendingSettingsJson { get; private set; }
    public string? PendingEncryptedSecrets { get; private set; }
    public IntegrationReviewStatus ReviewStatus { get; private set; }
    public string SubmittedBy { get; private set; } = string.Empty;
    public DateTime SubmittedAt { get; private set; }
    public string? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewNotes { get; private set; }

    private IntegrationConfiguration() { }

    public IntegrationConfiguration(
        Guid organizationId,
        string code,
        string name,
        string serviceCategory,
        string connectorType,
        string? description,
        string fieldDefinitionsJson,
        bool isEnabled,
        string submittedBy)
    {
        OrganizationId = organizationId;
        Code = Required(code, "Integration code").ToUpperInvariant();
        Name = Required(name, "Integration name");
        ServiceCategory = Required(serviceCategory, "Service category");
        ConnectorType = Required(connectorType, "Connector type");
        Description = Normalize(description);
        FieldDefinitionsJson = fieldDefinitionsJson;
        IsEnabled = isEnabled;
        ReviewStatus = IntegrationReviewStatus.Active;
        SubmittedBy = submittedBy;
        SubmittedAt = DateTime.UtcNow;
        ReviewedBy = submittedBy;
        ReviewedAt = SubmittedAt;
    }

    public void ConfigureInitial(
        string settingsJson,
        string? encryptedSecrets,
        string submittedBy)
    {
        if (IsConfigured)
            throw new InvalidOperationException("The integration is already configured.");

        SettingsJson = settingsJson;
        EncryptedSecrets = encryptedSecrets;
        IsConfigured = true;
        SubmittedBy = submittedBy;
        SubmittedAt = DateTime.UtcNow;
        ReviewedBy = submittedBy;
        ReviewedAt = SubmittedAt;
        ReviewStatus = IntegrationReviewStatus.Active;
        SetUpdated();
    }

    public void SubmitChange(
        string settingsJson,
        string? encryptedSecrets,
        string submittedBy)
    {
        PendingSettingsJson = settingsJson;
        PendingEncryptedSecrets = encryptedSecrets;
        ReviewStatus = IntegrationReviewStatus.PendingReview;
        SubmittedBy = submittedBy;
        SubmittedAt = DateTime.UtcNow;
        ReviewedBy = null;
        ReviewedAt = null;
        ReviewNotes = null;
        SetUpdated();
    }

    public void SetEnabled(bool enabled)
    {
        IsEnabled = enabled;
        SetUpdated();
    }

    public void Approve(string reviewedBy, string? notes)
    {
        if (ReviewStatus != IntegrationReviewStatus.PendingReview)
            throw new InvalidOperationException("This integration does not have a pending change.");

        SettingsJson = PendingSettingsJson ?? SettingsJson;
        EncryptedSecrets = PendingEncryptedSecrets ?? EncryptedSecrets;
        ClearPending();
        ReviewStatus = IntegrationReviewStatus.Active;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = Normalize(notes);
        SetUpdated();
    }

    public void Reject(string reviewedBy, string? notes)
    {
        if (ReviewStatus != IntegrationReviewStatus.PendingReview)
            throw new InvalidOperationException("This integration does not have a pending change.");

        ClearPending();
        ReviewStatus = IntegrationReviewStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = Normalize(notes);
        SetUpdated();
    }

    private void ClearPending()
    {
        PendingSettingsJson = null;
        PendingEncryptedSecrets = null;
    }

    private static string Required(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"{field} is required.");
        return value.Trim();
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
