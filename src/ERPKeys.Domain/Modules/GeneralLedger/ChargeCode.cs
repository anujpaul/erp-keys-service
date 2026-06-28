using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.GeneralLedger;

public enum ChargeModule
{
    AccountsPayable,
    AccountsReceivable
}

public enum ChargeCalculationMethod
{
    FixedAmount,
    Percentage
}

public class ChargeCode : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public ChargeModule Module { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ChargeCalculationMethod CalculationMethod { get; private set; }
    public decimal DefaultValue { get; private set; }
    public string? CurrencyCode { get; private set; }
    public bool IsTaxable { get; private set; }
    public Guid PostingAccountId { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Account? PostingAccount { get; private set; }

    private ChargeCode() { }

    public ChargeCode(
        Guid organizationId,
        ChargeModule module,
        string code,
        string name,
        string? description,
        ChargeCalculationMethod calculationMethod,
        decimal defaultValue,
        string? currencyCode,
        bool isTaxable,
        Guid postingAccountId)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization is required.");
        OrganizationId = organizationId;
        Module = module;
        Code = NormalizeRequired(code, "Charge code").ToUpperInvariant();
        Update(name, description, calculationMethod, defaultValue,
            currencyCode, isTaxable, postingAccountId);
    }

    public void Update(
        string name,
        string? description,
        ChargeCalculationMethod calculationMethod,
        decimal defaultValue,
        string? currencyCode,
        bool isTaxable,
        Guid postingAccountId)
    {
        if (defaultValue < 0)
            throw new ArgumentException("Default charge value cannot be negative.");
        if (calculationMethod == ChargeCalculationMethod.Percentage && defaultValue > 100)
            throw new ArgumentException("Default charge percentage cannot exceed 100.");
        if (postingAccountId == Guid.Empty)
            throw new ArgumentException("Posting account is required.");

        Name = NormalizeRequired(name, "Charge name");
        Description = description?.Trim() ?? string.Empty;
        CalculationMethod = calculationMethod;
        DefaultValue = defaultValue;
        CurrencyCode = calculationMethod == ChargeCalculationMethod.FixedAmount &&
                       !string.IsNullOrWhiteSpace(currencyCode)
            ? currencyCode.Trim().ToUpperInvariant()
            : null;
        IsTaxable = isTaxable;
        PostingAccountId = postingAccountId;
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
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{field} is required.");
        return value.Trim();
    }
}
