namespace ERPKeys.Application.Modules.ProductManagement.DTOs;

// ── Category ──────────────────────────────────────────────────────────────────

public record CategoryDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    Guid? ParentCategoryId,
    string? ParentCategoryName,
    int DisplayOrder,
    bool IsActive,
    int ProductCount,
    decimal TaxRate,       // default tax rate for all products in this category
    string? TaxCode        // e.g. CLOTHING, FOOTWEAR, FOOD_EXEMPT — for future tax engine
);

public record CreateCategoryRequest(
    string Code,
    string Name,
    string? Description     = null,
    Guid?   ParentCategoryId = null,
    int     DisplayOrder    = 0,
    decimal TaxRate         = 0,
    string? TaxCode         = null
);

// ── Brand ─────────────────────────────────────────────────────────────────────

public record BrandDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    string? Country,
    string? Website,
    string? LogoUrl,
    bool IsActive,
    int ProductCount
);

public record CreateBrandRequest(
    string Code,
    string Name,
    string? Description = null,
    string? Country = null,
    string? Website = null
);

// ── Product ───────────────────────────────────────────────────────────────────

public record ProductSummaryDto(
    Guid Id,
    string Sku,
    string Name,
    string CategoryName,
    string? BrandName,
    string ProductType,
    string GenderTarget,
    decimal BasePrice,
    string Currency,
    string Status,
    int VariantCount,
    decimal TotalStock
);

public record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    string? LongDescription,
    Guid CategoryId,
    string CategoryName,
    Guid? BrandId,
    string? BrandName,
    string ProductType,
    string GenderTarget,
    string UnitOfMeasure,
    decimal BasePrice,
    decimal BaseCost,
    decimal EffectiveTaxRate,       // resolved: override if set, else category rate
    decimal? TaxRateOverride,       // null = inherited from category
    string? SalesTaxGroup,
    decimal CategoryTaxRate,        // the category's default rate (for display)
    string? CategoryTaxCode,        // e.g. CLOTHING, FOOTWEAR
    string Currency,
    string? Tags,
    string? ImageUrl,
    string Status,
    Guid? PreferredVendorId,
    string? PreferredVendorName,
    Guid? VariantAttributeDefinitionId,
    string? VariantAttributeDefinitionName,
    DateTime CreatedAt,
    IEnumerable<ProductVariantDto> Variants
);

public record CreateProductRequest(
    string Sku,
    string Name,
    Guid CategoryId,
    string ProductType,
    decimal BasePrice,
    decimal BaseCost,
    string UnitOfMeasure = "Each",
    Guid? BrandId = null,
    string GenderTarget = "Unisex",
    string? Description = null,
    string? Tags = null,
    string Currency = "USD",
    decimal? TaxRateOverride = null,   // leave null to inherit from category
    string? SalesTaxGroup = null,
    string Status = "New"
);

public record UpdateProductRequest(
    string Name,
    string? Description,
    string? LongDescription,
    Guid CategoryId,
    Guid? BrandId,
    decimal BasePrice,
    decimal BaseCost,
    string ProductType,
    string GenderTarget,
    string? Tags,
    string? ImageUrl,
    decimal? TaxRateOverride = null   // leave null to inherit from category
);

public record SetSalesTaxGroupRequest(string? SalesTaxGroup);

public record VariantAttributeValueDto(
    Guid Id,
    string AttributeType,
    string Value,
    int DisplayOrder
);

public record VariantAttributeDefinitionDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    IEnumerable<VariantAttributeValueDto> Values
);

public record CreateVariantAttributeDefinitionRequest(
    string Code,
    string Name,
    string? Description,
    IEnumerable<string> Sizes,
    IEnumerable<string>? Colors,
    IEnumerable<string>? Materials
);

public record SetVariantAttributeDefinitionRequest(Guid? DefinitionId);

public record VariantCombinationRequest(
    string Size,
    string? Color,
    string? Material
);

public record CreateVariantBatchRequest(
    IEnumerable<VariantCombinationRequest> Variants,
    decimal InitialStock = 0m,
    decimal ReorderPoint = 5m,
    decimal MinimumStock = 0m,
    decimal MaximumStock = 100m,
    string? Location = null
);

// ── Variant ───────────────────────────────────────────────────────────────────

public record ProductVariantDto(
    Guid Id,
    Guid ProductId,
    int VariantNumber,
    string VariantName,
    string Sku,
    string? Barcode,
    string Size,
    string? Color,
    string? Material,
    decimal? PriceOverride,
    decimal? CostOverride,
    decimal EffectivePrice,
    decimal EffectiveCost,
    decimal Weight,
    string Status,
    // inventory
    decimal QuantityOnHand,
    decimal QuantityReserved,
    decimal QuantityAvailable,
    decimal ReorderPoint,
    bool NeedsReorder,
    string? Location
);

public record CreateVariantRequest(
    string Size,
    string? Color = null,
    string? Material = null,
    string? Barcode = null,
    decimal? PriceOverride = null,
    decimal? CostOverride = null,
    decimal Weight = 0m,
    // initial inventory
    decimal InitialStock = 0m,
    decimal ReorderPoint = 5m,
    decimal MinimumStock = 0m,
    decimal MaximumStock = 100m,
    string? Location = null
);

public record UpdateVariantPricingRequest(
    decimal? PriceOverride,
    decimal? CostOverride
);

// ── Inventory ─────────────────────────────────────────────────────────────────

public record InventoryDto(
    Guid Id,
    Guid ProductVariantId,
    int VariantNumber,
    string VariantSku,
    string VariantName,
    string ProductName,
    string? CategoryName,
    string Size,
    string? Color,
    string? Material,
    decimal EffectivePrice,
    string VariantStatus,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    decimal QuantityAvailable,
    decimal ReorderPoint,
    decimal MinimumStock,
    decimal MaximumStock,
    bool NeedsReorder,
    string? Location,
    DateTime? LastCountDate
);

public record PagedProductInventoryDto(
    IReadOnlyList<InventoryDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    int ReorderCount
);

public record AdjustInventoryRequest(
    decimal Delta,
    string Reason
);

public record SetInventoryRequest(
    decimal Quantity,
    DateTime CountDate
);

public record UpdateThresholdsRequest(
    decimal ReorderPoint,
    decimal MinimumStock,
    decimal MaximumStock,
    string? Location
);

// ── Lookup (for AR sales order line picker + AP PO line picker) ───────────────

public record VariantLookupDto(
    Guid VariantId,
    string Sku,
    string? Barcode,
    string ProductName,
    string Size,
    string? Color,
    string? Material,
    decimal EffectivePrice,
    decimal EffectiveCost,   // used by AP to pre-fill unit cost
    decimal TaxRate,
    string UnitOfMeasure,
    decimal QuantityAvailable,
    Guid? PreferredVendorId,
    string? PreferredVendorName
);

// ── Preferred Vendor ──────────────────────────────────────────────────────────
public record SetPreferredVendorRequest(Guid? VendorId);
