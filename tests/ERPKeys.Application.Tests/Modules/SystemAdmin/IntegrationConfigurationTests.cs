using ERPKeys.Domain.Modules.SystemAdmin;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.SystemAdmin;

public class IntegrationConfigurationTests
{
    [Fact]
    public void InitialConfiguration_IsActiveWithoutExposingSecretState()
    {
        var configuration = Create();

        Assert.Equal(IntegrationReviewStatus.Active, configuration.ReviewStatus);
        Assert.False(configuration.IsConfigured);
        Assert.Null(configuration.PendingEncryptedSecrets);
    }

    [Fact]
    public void Change_RemainsPendingUntilApproved()
    {
        var configuration = Create();
        configuration.ConfigureInitial(
            """{"BaseUrl":"https://old.example.com"}""",
            "encrypted-old-secret",
            "admin");

        configuration.SubmitChange(
            """{"BaseUrl":"https://new.example.com"}""",
            "encrypted-new-secret",
            "requester");

        Assert.Equal(IntegrationReviewStatus.PendingReview, configuration.ReviewStatus);
        Assert.Equal("""{"BaseUrl":"https://old.example.com"}""", configuration.SettingsJson);
        Assert.Equal("encrypted-old-secret", configuration.EncryptedSecrets);

        configuration.Approve("admin", "Validated with the provider.");

        Assert.Equal(IntegrationReviewStatus.Active, configuration.ReviewStatus);
        Assert.Equal("""{"BaseUrl":"https://new.example.com"}""", configuration.SettingsJson);
        Assert.Equal("encrypted-new-secret", configuration.EncryptedSecrets);
        Assert.Null(configuration.PendingSettingsJson);
    }

    [Fact]
    public void RejectedChange_DoesNotReplaceActiveConfiguration()
    {
        var configuration = Create();
        configuration.ConfigureInitial(
            """{"BaseUrl":"https://old.example.com"}""",
            "encrypted-old-secret",
            "admin");
        configuration.SubmitChange("{}", "encrypted-new-secret", "requester");

        configuration.Reject("admin", "The submitted endpoint could not be verified.");

        Assert.Equal(IntegrationReviewStatus.Rejected, configuration.ReviewStatus);
        Assert.Equal("encrypted-old-secret", configuration.EncryptedSecrets);
        Assert.Null(configuration.PendingEncryptedSecrets);
    }

    private static IntegrationConfiguration Create() =>
        new(
            Guid.NewGuid(),
            "PRIMARY_LLM",
            "Primary LLM",
            "LLM",
            "OpenAICompatible",
            null,
            """[{"key":"ApiKey","label":"API key","dataType":"password","isSecret":true,"isRequired":true}]""",
            false,
            "admin");
}
