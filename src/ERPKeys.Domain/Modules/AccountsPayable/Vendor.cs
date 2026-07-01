using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.AccountsPayable;

public enum VendorStatus { Active, Inactive, OnHold, Blacklisted }

public class Vendor : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string VendorNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }        // legacy — kept for compat
    public string? BillingAddress { get; private set; }
    public string? ShippingAddress { get; private set; }
    public string? Website { get; private set; }
    public string? Notes { get; private set; }
    public string Currency { get; private set; } = "USD";
    public int PaymentTermsDays { get; private set; } = 30;
    public string? TaxId { get; private set; }
    public string? BankAccountName { get; private set; }
    public string? BankAccountNumber { get; private set; }
    public string? BankRoutingNumber { get; private set; }
    public VendorStatus Status { get; private set; } = VendorStatus.Active;

    // Export tracking
    public bool      IsExported { get; private set; }
    public DateTime? ExportedAt { get; private set; }

    private Vendor() { }

    public Vendor(Guid organizationId, string vendorNumber, string name, string? email = null,
        string? phone = null, string? address = null,
        string currency = "USD", int paymentTermsDays = 30, string? taxId = null,
        string? billingAddress = null, string? shippingAddress = null,
        string? website = null, string? notes = null)
    {
        OrganizationId = organizationId;
        VendorNumber = vendorNumber;
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        BillingAddress = billingAddress ?? address;
        ShippingAddress = shippingAddress ?? address;
        Website = website;
        Notes = notes;
        Currency = currency;
        PaymentTermsDays = paymentTermsDays;
        TaxId = taxId;
    }

    public void Update(string name, string? email, string? phone,
        string? billingAddress, string? shippingAddress,
        int paymentTermsDays, string? bankAccountName, string? bankAccountNumber,
        string? bankRoutingNumber = null, string? website = null, string? notes = null)
    {
        Name = name;
        Email = email;
        Phone = phone;
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
        Address = billingAddress;  // keep legacy in sync
        PaymentTermsDays = paymentTermsDays;
        BankAccountName = bankAccountName;
        BankAccountNumber = bankAccountNumber;
        BankRoutingNumber = bankRoutingNumber;
        Website = website;
        Notes = notes;
        SetUpdated();
    }

    public void SetStatus(VendorStatus status) { Status = status; SetUpdated(); }
    public void Activate()     { Status = VendorStatus.Active;   SetUpdated(); }
    public void Deactivate()   { Status = VendorStatus.Inactive; SetUpdated(); }
    public void MarkExported() { IsExported = true;  ExportedAt = DateTime.UtcNow; SetUpdated(); }
    public void ResetExport()  { IsExported = false; ExportedAt = null;            SetUpdated(); }

    // Navigation — loaded via .Include() in queries
    public ICollection<VendorAddress> Addresses { get; private set; } = new List<VendorAddress>();
    public ICollection<VendorContact> Contacts  { get; private set; } = new List<VendorContact>();

    public void InitializeAddressAndContactRecords()
    {
        if (Addresses.Count == 0)
        {
            AddInitialAddress("Billing Address", VendorAddressType.Billing,
                BillingAddress, isPrimary: true);
            AddInitialAddress("Shipping Address", VendorAddressType.Shipping,
                ShippingAddress, isPrimary: Addresses.Count == 0);
        }

        if (Contacts.Count == 0 &&
            (!string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(Phone)))
        {
            Contacts.Add(new VendorContact(
                OrganizationId,
                Id,
                Name,
                "Primary Contact",
                Email,
                Phone,
                null,
                isPrimary: true));
        }
    }

    private void AddInitialAddress(
        string label,
        VendorAddressType addressType,
        string? address,
        bool isPrimary)
    {
        if (string.IsNullOrWhiteSpace(address))
            return;

        var normalized = address.Trim();
        var line1 = normalized.Length <= 300 ? normalized : normalized[..300];
        var line2 = normalized.Length <= 300 ? null : normalized[300..];
        Addresses.Add(new VendorAddress(
            OrganizationId,
            Id,
            label,
            addressType,
            line1,
            line2,
            string.Empty,
            null,
            null,
            string.Empty,
            isPrimary));
    }
}
