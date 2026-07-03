using ERPKeys.Application.Modules.SystemAdmin.Services;
using Microsoft.AspNetCore.DataProtection;

namespace ERPKeys.Infrastructure.Services;

public sealed class DataProtectionIntegrationSecretProtector : IIntegrationSecretProtector
{
    private readonly IDataProtector _protector;

    public DataProtectionIntegrationSecretProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector(
            "ERPKeys.SystemAdmin.IntegrationSecrets.v1");
    }

    public string Protect(string plaintext) => _protector.Protect(plaintext);
    public string Unprotect(string ciphertext) => _protector.Unprotect(ciphertext);
}
