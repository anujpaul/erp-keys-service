using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.Organization;

public class NumberSequence : BaseEntity
{
    private NumberSequence() { }

    public NumberSequence(
        Guid organizationId,
        string area,
        string displayName,
        string prefix,
        int padding,
        long nextNumber = 1,
        bool includeYear = false,
        string separator = "-",
        bool allowManualOverride = false,
        bool isActive = true)
    {
        OrganizationId = organizationId;
        Area = NormalizeArea(area);
        DisplayName = displayName.Trim();
        Prefix = NormalizePrefix(prefix);
        Padding = padding;
        NextNumber = nextNumber;
        IncludeYear = includeYear;
        Separator = string.IsNullOrWhiteSpace(separator) ? "-" : separator.Trim();
        AllowManualOverride = allowManualOverride;
        IsActive = isActive;
        Validate();
    }

    public Guid OrganizationId { get; private set; }
    public string Area { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Prefix { get; private set; } = string.Empty;
    public bool IncludeYear { get; private set; }
    public string Separator { get; private set; } = "-";
    public int Padding { get; private set; } = 6;
    public long NextNumber { get; private set; } = 1;
    public bool AllowManualOverride { get; private set; }
    public bool IsActive { get; private set; } = true;

    public void Update(
        string displayName,
        string prefix,
        bool includeYear,
        string separator,
        int padding,
        long nextNumber,
        bool allowManualOverride,
        bool isActive)
    {
        DisplayName = displayName.Trim();
        Prefix = NormalizePrefix(prefix);
        IncludeYear = includeYear;
        Separator = string.IsNullOrWhiteSpace(separator) ? "-" : separator.Trim();
        Padding = padding;
        NextNumber = nextNumber;
        AllowManualOverride = allowManualOverride;
        IsActive = isActive;
        Validate();
        SetUpdated();
    }

    public string ReserveNext(DateTime documentDate)
    {
        if (!IsActive)
            throw new InvalidOperationException($"Number sequence '{DisplayName}' is inactive.");

        var value = Format(documentDate, NextNumber);
        NextNumber++;
        SetUpdated();
        return value;
    }

    public string Preview(DateTime documentDate) => Format(documentDate, NextNumber);

    private string Format(DateTime documentDate, long number)
    {
        var padded = number.ToString(new string('0', Padding));
        if (IncludeYear)
            return Join(Prefix, documentDate.Year.ToString("D4"), padded);
        return Join(Prefix, padded);
    }

    private string Join(params string[] parts) =>
        string.Join(Separator, parts.Where(part => !string.IsNullOrWhiteSpace(part)));

    private void Validate()
    {
        if (OrganizationId == Guid.Empty)
            throw new InvalidOperationException("Organization is required for a number sequence.");
        if (string.IsNullOrWhiteSpace(Area))
            throw new InvalidOperationException("Number sequence area is required.");
        if (string.IsNullOrWhiteSpace(DisplayName))
            throw new InvalidOperationException("Number sequence display name is required.");
        if (Prefix.Length > 20)
            throw new InvalidOperationException("Number sequence prefix cannot exceed 20 characters.");
        if (Separator.Length > 3)
            throw new InvalidOperationException("Number sequence separator cannot exceed 3 characters.");
        if (Padding is < 1 or > 12)
            throw new InvalidOperationException("Number sequence padding must be between 1 and 12.");
        if (NextNumber < 1)
            throw new InvalidOperationException("Next number must be at least 1.");
    }

    private static string NormalizeArea(string area) =>
        area.Trim().ToLowerInvariant();

    private static string NormalizePrefix(string prefix) =>
        prefix.Trim().ToUpperInvariant();
}
