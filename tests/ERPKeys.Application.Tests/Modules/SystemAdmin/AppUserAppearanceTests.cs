using ERPKeys.Domain.Modules.SystemAdmin;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.SystemAdmin;

public class AppUserAppearanceTests
{
    [Fact]
    public void UpdateAppearance_stores_supported_themes()
    {
        var user = CreateUser();

        user.UpdateAppearance("Emerald", "Forest");

        Assert.Equal("emerald", user.HeaderThemeId);
        Assert.Equal("forest", user.SidebarThemeId);
    }

    [Theory]
    [InlineData("", "forest")]
    [InlineData("unknown", "forest")]
    [InlineData("emerald", "unknown")]
    public void UpdateAppearance_rejects_unsupported_themes(
        string headerThemeId,
        string sidebarThemeId)
    {
        var user = CreateUser();

        Assert.Throws<InvalidOperationException>(() =>
            user.UpdateAppearance(headerThemeId, sidebarThemeId));
    }

    private static AppUser CreateUser()
        => new(
            Guid.NewGuid(),
            "appearance-user",
            "appearance@example.com",
            "Appearance User",
            "not-used");
}
