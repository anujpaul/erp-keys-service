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

    private static Product CreateProduct() => new(
        Guid.NewGuid(),
        "SKU-001",
        "Test product",
        Guid.NewGuid(),
        ProductType.Other,
        10m,
        5m);
}
