using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.Payments;

public enum PaymentProcessorEnvironment
{
    Sandbox,
    Production
}

public class PaymentProcessorConfiguration : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string ProviderCode { get; private set; } = string.Empty;
    public PaymentProcessorEnvironment Environment { get; private set; }
    public string EndpointBaseUrl { get; private set; } = string.Empty;
    public string MerchantAccountReference { get; private set; } = string.Empty;
    public string CredentialSecretReference { get; private set; } = string.Empty;
    public int TimeoutSeconds { get; private set; } = 30;
    public bool IsActive { get; private set; } = true;

    private PaymentProcessorConfiguration() { }

    public PaymentProcessorConfiguration(
        Guid organizationId,
        string code,
        string name,
        string providerCode,
        PaymentProcessorEnvironment environment,
        string endpointBaseUrl,
        string merchantAccountReference,
        string credentialSecretReference,
        int timeoutSeconds = 30)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        Code = NormalizeRequired(code, "Processor code").ToUpperInvariant();
        Update(name, providerCode, environment, endpointBaseUrl,
            merchantAccountReference, credentialSecretReference, timeoutSeconds);
    }

    public void Update(
        string name,
        string providerCode,
        PaymentProcessorEnvironment environment,
        string endpointBaseUrl,
        string merchantAccountReference,
        string credentialSecretReference,
        int timeoutSeconds)
    {
        if (timeoutSeconds is < 1 or > 300)
            throw new ArgumentException("Processor timeout must be between 1 and 300 seconds.");
        if (!Uri.TryCreate(endpointBaseUrl, UriKind.Absolute, out var endpoint) ||
            endpoint.Scheme is not ("http" or "https"))
            throw new ArgumentException("Processor endpoint must be an absolute HTTP or HTTPS URL.");
        if (environment == PaymentProcessorEnvironment.Production && endpoint.Scheme != "https")
            throw new ArgumentException("Production processor endpoints must use HTTPS.");

        Name = NormalizeRequired(name, "Processor name");
        ProviderCode = NormalizeRequired(providerCode, "Provider code").ToUpperInvariant();
        Environment = environment;
        EndpointBaseUrl = endpoint.ToString().TrimEnd('/');
        MerchantAccountReference = NormalizeRequired(
            merchantAccountReference, "Merchant account reference");
        CredentialSecretReference = NormalizeRequired(
            credentialSecretReference, "Credential secret reference");
        TimeoutSeconds = timeoutSeconds;
        SetUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    private static string NormalizeRequired(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{field} is required.");
        return value.Trim();
    }
}
