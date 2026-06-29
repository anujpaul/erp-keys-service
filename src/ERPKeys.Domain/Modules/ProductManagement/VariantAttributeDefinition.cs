using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.ProductManagement;

public enum VariantAttributeType { Size, Color, Material }

public class VariantAttributeDefinition : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<VariantAttributeValue> _values = new();
    public IReadOnlyCollection<VariantAttributeValue> Values => _values.AsReadOnly();

    private VariantAttributeDefinition() { }

    public VariantAttributeDefinition(
        Guid organizationId, string code, string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Definition code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Definition name is required.", nameof(name));

        OrganizationId = organizationId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void AddValue(VariantAttributeType attributeType, string value, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Attribute value is required.", nameof(value));
        if (_values.Any(existing =>
                existing.AttributeType == attributeType &&
                existing.Value.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException(
                $"The {attributeType} value '{value.Trim()}' is duplicated.");

        _values.Add(new VariantAttributeValue(
            OrganizationId, Id, attributeType, value, displayOrder));
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }
}

public class VariantAttributeValue : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid VariantAttributeDefinitionId { get; private set; }
    public VariantAttributeType AttributeType { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }

    public VariantAttributeDefinition? Definition { get; private set; }

    private VariantAttributeValue() { }

    internal VariantAttributeValue(
        Guid organizationId,
        Guid definitionId,
        VariantAttributeType attributeType,
        string value,
        int displayOrder)
    {
        OrganizationId = organizationId;
        VariantAttributeDefinitionId = definitionId;
        AttributeType = attributeType;
        Value = value.Trim();
        DisplayOrder = displayOrder;
    }
}
