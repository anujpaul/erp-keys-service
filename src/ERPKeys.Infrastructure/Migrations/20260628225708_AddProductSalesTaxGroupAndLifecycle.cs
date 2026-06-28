using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSalesTaxGroupAndLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sales_tax_group",
                table: "catalog_products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE catalog_products SET status = 'Exiting' WHERE status = 'Inactive';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE catalog_products SET status = 'Inactive' WHERE status = 'Exiting';");

            migrationBuilder.DropColumn(
                name: "sales_tax_group",
                table: "catalog_products");
        }
    }
}
