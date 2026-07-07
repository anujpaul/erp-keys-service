using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesOrderPaginationIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_organization_id_customer_id_order_date_created",
                table: "sales_orders",
                columns: new[] { "organization_id", "customer_id", "order_date", "created_at", "id" },
                descending: new[] { false, false, true, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_organization_id_order_date_created_at_id",
                table: "sales_orders",
                columns: new[] { "organization_id", "order_date", "created_at", "id" },
                descending: new[] { false, true, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_organization_id_status_order_date_created_at_id",
                table: "sales_orders",
                columns: new[] { "organization_id", "status", "order_date", "created_at", "id" },
                descending: new[] { false, false, true, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sales_orders_organization_id_customer_id_order_date_created",
                table: "sales_orders");

            migrationBuilder.DropIndex(
                name: "ix_sales_orders_organization_id_order_date_created_at_id",
                table: "sales_orders");

            migrationBuilder.DropIndex(
                name: "ix_sales_orders_organization_id_status_order_date_created_at_id",
                table: "sales_orders");
        }
    }
}
