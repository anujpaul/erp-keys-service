using ERPKeys.Domain.Modules.ProductManagement;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.ProductManagement;

public class ProductLifecycleTests
{
    [Fact]
    public void Product_supports_requested_lifecycle_states()
    {
        var product = CreateProduct();

        product.MarkNew();
        Assert.Equal(ProductStatus.New, product.Status);

        product.Activate();
        Assert.Equal(ProductStatus.Active, product.Status);

        product.MarkExiting();
        Assert.Equal(ProductStatus.Exiting, product.Status);

        product.Discontinue();
        Assert.Equal(ProductStatus.Discontinued, product.Status);
    }

    [Fact]
    public void Sales_tax_group_is_normalized_and_can_be_cleared()
    {
        var product = CreateProduct();

        product.SetSalesTaxGroup("  retail-tax ");
        Assert.Equal("RETAIL-TAX", product.SalesTaxGroup);

        product.SetSalesTaxGroup(" ");
        Assert.Null(product.SalesTaxGroup);
    }

    [Fact]
    public void Variant_attribute_definition_rejects_duplicate_values()
    {
        var definition = new VariantAttributeDefinition(
            Guid.NewGuid(), "apparel", "Standard Apparel");

        definition.AddValue(VariantAttributeType.Size, "M", 0);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            definition.AddValue(VariantAttributeType.Size, "m", 1));
        Assert.Contains("duplicated", exception.Message);
    }

    [Fact]
    public void Variants_receive_sequential_seven_digit_ids_from_the_product_block()
    {
        var product = CreateProduct();

        var first = product.AddVariant("M", "Light Blue", "Organic Cotton");
        var second = product.AddVariant("L", "Black", null);

        Assert.Equal(1_000_001, first.VariantNumber);
        Assert.Equal("1000001", first.Sku);
        Assert.Equal(1_000_002, second.VariantNumber);
        Assert.Equal("1000002", second.Sku);
    }

    [Fact]
    public void Product_variant_blocks_leave_room_without_overlapping()
    {
        var firstProduct = CreateProduct();
        var secondProduct = new Product(
            Guid.NewGuid(), "SKU-002", "Second product", Guid.NewGuid(),
            ProductType.Other, 10m, 5m, variantNumberBase: 1_001_000);

        var first = firstProduct.AddVariant("M", null, null);
        var second = secondProduct.AddVariant("M", null, null);

        Assert.Equal(1_000_001, first.VariantNumber);
        Assert.Equal(1_001_001, second.VariantNumber);
        Assert.True(second.VariantNumber - first.VariantNumber >= 1_000);
    }

    private static Product CreateProduct() => new(
        Guid.NewGuid(),
        "SKU-001",
        "Test product",
        Guid.NewGuid(),
        ProductType.Other,
        10m,
        5m,
        variantNumberBase: 1_000_000);
}
