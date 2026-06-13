using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public class FinancialDimension : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private readonly List<FinancialDimensionValue> _values = new();
    public IReadOnlyCollection<FinancialDimensionValue> Values => _values.AsReadOnly();

    private FinancialDimension() { }

    public FinancialDimension(Guid organizationId, string code, string name, string? description)
    {
        OrganizationId = organizationId;
        Code = NormalizeCode(code);
        Name = Required(name, nameof(name));
        Description = description?.Trim() ?? string.Empty;
    }

    public void Deactivate() { IsActive = false; SetUpdated(); }

    public static string NormalizeCode(string value)
    {
        var code = Required(value, nameof(value)).ToUpperInvariant();
        if (code.Any(c => !char.IsLetterOrDigit(c) && c != '_' && c != '-'))
            throw new ArgumentException("Codes may contain letters, numbers, hyphens, and underscores only.");
        return code;
    }

    internal static string Required(string value, string field)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException($"{field} is required.");
        return normalized;
    }
}

public class FinancialDimensionValue : BaseEntity
{
    public Guid FinancialDimensionId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public FinancialDimension? FinancialDimension { get; private set; }

    private FinancialDimensionValue() { }

    public FinancialDimensionValue(Guid financialDimensionId, string code, string name, string? description)
    {
        FinancialDimensionId = financialDimensionId;
        Code = FinancialDimension.NormalizeCode(code);
        Name = FinancialDimension.Required(name, nameof(name));
        Description = description?.Trim() ?? string.Empty;
    }

    public void Deactivate() { IsActive = false; SetUpdated(); }
}

public class FinancialDimensionSet : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<FinancialDimensionSetMember> _members = new();
    public IReadOnlyCollection<FinancialDimensionSetMember> Members => _members.AsReadOnly();

    private FinancialDimensionSet() { }

    public FinancialDimensionSet(
        Guid organizationId,
        string name,
        string? description,
        bool isDefault,
        IEnumerable<(Guid DimensionId, bool IsRequired)> members)
    {
        OrganizationId = organizationId;
        Name = FinancialDimension.Required(name, nameof(name));
        Description = description?.Trim() ?? string.Empty;
        IsDefault = isDefault;

        var uniqueMembers = members.DistinctBy(x => x.DimensionId).ToList();
        if (uniqueMembers.Count == 0)
            throw new ArgumentException("A financial dimension set must contain at least one dimension.");

        for (var i = 0; i < uniqueMembers.Count; i++)
            _members.Add(new FinancialDimensionSetMember(Id, uniqueMembers[i].DimensionId, i + 1, uniqueMembers[i].IsRequired));
    }

    public void SetDefault(bool value) { IsDefault = value; SetUpdated(); }
    public void Deactivate() { IsActive = false; IsDefault = false; SetUpdated(); }
}

public class FinancialDimensionSetMember : BaseEntity
{
    public Guid FinancialDimensionSetId { get; private set; }
    public Guid FinancialDimensionId { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsRequired { get; private set; }

    public FinancialDimensionSet? FinancialDimensionSet { get; private set; }
    public FinancialDimension? FinancialDimension { get; private set; }

    private FinancialDimensionSetMember() { }

    internal FinancialDimensionSetMember(
        Guid financialDimensionSetId,
        Guid financialDimensionId,
        int displayOrder,
        bool isRequired)
    {
        FinancialDimensionSetId = financialDimensionSetId;
        FinancialDimensionId = financialDimensionId;
        DisplayOrder = displayOrder;
        IsRequired = isRequired;
    }
}

public class JournalLineDimensionValue : BaseEntity
{
    public Guid JournalLineId { get; private set; }
    public Guid FinancialDimensionValueId { get; private set; }

    public JournalLine? JournalLine { get; private set; }
    public FinancialDimensionValue? FinancialDimensionValue { get; private set; }

    private JournalLineDimensionValue() { }

    internal JournalLineDimensionValue(Guid journalLineId, Guid financialDimensionValueId)
    {
        JournalLineId = journalLineId;
        FinancialDimensionValueId = financialDimensionValueId;
    }
}
