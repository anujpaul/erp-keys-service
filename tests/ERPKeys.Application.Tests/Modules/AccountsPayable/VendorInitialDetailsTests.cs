using ERPKeys.Domain.Modules.AccountsPayable;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.AccountsPayable;

public class VendorInitialDetailsTests
{
    [Fact]
    public void InitializeAddressAndContactRecords_CreatesRecordsFromVendorHeader()
    {
        var vendor = CreateVendor(
            email: "buyer@example.com",
            phone: "555-0100",
            billingAddress: "10 Billing Road, Austin, TX 78701",
            shippingAddress: "20 Warehouse Lane, Dallas, TX 75001");

        vendor.InitializeAddressAndContactRecords();

        var billing = Assert.Single(vendor.Addresses,
            address => address.AddressType == VendorAddressType.Billing);
        Assert.True(billing.IsPrimary);
        Assert.Equal("10 Billing Road, Austin, TX 78701", billing.SingleLine);

        var shipping = Assert.Single(vendor.Addresses,
            address => address.AddressType == VendorAddressType.Shipping);
        Assert.False(shipping.IsPrimary);
        Assert.Equal("20 Warehouse Lane, Dallas, TX 75001", shipping.SingleLine);

        var contact = Assert.Single(vendor.Contacts);
        Assert.True(contact.IsPrimary);
        Assert.Equal(vendor.Name, contact.Name);
        Assert.Equal("buyer@example.com", contact.Email);
        Assert.Equal("555-0100", contact.Phone);
    }

    [Fact]
    public void InitializeAddressAndContactRecords_DoesNotCreateEmptyOrDuplicateRecords()
    {
        var vendor = CreateVendor();

        vendor.InitializeAddressAndContactRecords();
        vendor.InitializeAddressAndContactRecords();

        Assert.Empty(vendor.Addresses);
        Assert.Empty(vendor.Contacts);
    }

    [Fact]
    public void InitializeAddressAndContactRecords_IsIdempotent()
    {
        var vendor = CreateVendor(
            email: "buyer@example.com",
            billingAddress: "10 Billing Road");

        vendor.InitializeAddressAndContactRecords();
        vendor.InitializeAddressAndContactRecords();

        Assert.Equal(2, vendor.Addresses.Count);
        Assert.Single(vendor.Addresses,
            address => address.AddressType == VendorAddressType.Billing);
        Assert.Single(vendor.Addresses,
            address => address.AddressType == VendorAddressType.Shipping);
        Assert.Single(vendor.Contacts);
    }

    private static Vendor CreateVendor(
        string? email = null,
        string? phone = null,
        string? billingAddress = null,
        string? shippingAddress = null) =>
        new(
            Guid.NewGuid(),
            "VEND-00001",
            "Example Vendor",
            email,
            phone,
            billingAddress,
            "USD",
            30,
            null,
            billingAddress,
            shippingAddress);
}
