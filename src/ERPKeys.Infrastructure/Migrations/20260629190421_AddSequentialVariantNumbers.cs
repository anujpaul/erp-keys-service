using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSequentialVariantNumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "variant_number_block_seq",
                startValue: 1000000L,
                incrementBy: 1000,
                maxValue: 9999000L);

            migrationBuilder.AddColumn<int>(
                name: "next_variant_number_offset",
                table: "catalog_products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "variant_number_base",
                table: "catalog_products",
                type: "integer",
                nullable: false,
                defaultValueSql: "nextval('variant_number_block_seq')");

            migrationBuilder.AddColumn<int>(
                name: "variant_number",
                table: "product_variants",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM product_variants
                        GROUP BY product_id
                        HAVING COUNT(*) > 999
                    ) THEN
                        RAISE EXCEPTION 'A product has more than 999 variants and cannot fit in a variant-number block.';
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql(
                """
                WITH ranked AS (
                    SELECT
                        id,
                        product_id,
                        ROW_NUMBER() OVER (
                            PARTITION BY product_id
                            ORDER BY created_at, id
                        )::integer AS variant_offset
                    FROM product_variants
                )
                UPDATE product_variants AS variant
                SET
                    variant_number = product.variant_number_base + ranked.variant_offset,
                    sku = LPAD(
                        (product.variant_number_base + ranked.variant_offset)::text,
                        7,
                        '0')
                FROM ranked
                INNER JOIN catalog_products AS product
                    ON product.id = ranked.product_id
                WHERE variant.id = ranked.id;
                """);

            migrationBuilder.Sql(
                """
                UPDATE catalog_products AS product
                SET next_variant_number_offset = COALESCE((
                    SELECT MAX(variant.variant_number - product.variant_number_base)
                    FROM product_variants AS variant
                    WHERE variant.product_id = product.id
                ), 0);
                """);

            migrationBuilder.AlterColumn<int>(
                name: "variant_number",
                table: "product_variants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_variants_organization_id_variant_number",
                table: "product_variants",
                columns: new[] { "organization_id", "variant_number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_product_variants_organization_id_variant_number",
                table: "product_variants");

            migrationBuilder.DropColumn(
                name: "variant_number",
                table: "product_variants");

            migrationBuilder.DropColumn(
                name: "next_variant_number_offset",
                table: "catalog_products");

            migrationBuilder.DropColumn(
                name: "variant_number_base",
                table: "catalog_products");

            migrationBuilder.DropSequence(
                name: "variant_number_block_seq");
        }
    }
}
