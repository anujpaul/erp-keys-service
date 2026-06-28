using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountsPayableParametersAndPOInboundLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts_payable_parameters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allow_purchase_order_over_receipt = table.Column<bool>(type: "boolean", nullable: false),
                    maximum_over_receipt_percent = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts_payable_parameters", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_payable_parameters_organization_id",
                table: "accounts_payable_parameters",
                column: "organization_id",
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO inbound_orders (
                    id,
                    organization_id,
                    warehouse_id,
                    order_number,
                    purchase_order_id,
                    vendor_id,
                    vendor_name,
                    expected_date,
                    received_date,
                    status,
                    notes,
                    created_at,
                    updated_at,
                    is_deleted
                )
                SELECT
                    md5(random()::text || clock_timestamp()::text || po.id::text)::uuid,
                    po.organization_id,
                    po.warehouse_id,
                    po.po_number,
                    po.id,
                    po.vendor_id,
                    vendor.name,
                    COALESCE(po.expected_date, CURRENT_DATE),
                    CASE WHEN po.status = 'FullyReceived' THEN po.updated_at ELSE NULL END,
                    CASE
                        WHEN po.status = 'FullyReceived' THEN 'Completed'
                        WHEN po.status = 'PartiallyReceived' THEN 'Receiving'
                        ELSE 'Confirmed'
                    END,
                    'Automatically backfilled from purchase order ' || po.po_number || '.',
                    now(),
                    now(),
                    false
                FROM purchase_orders po
                JOIN vendors vendor ON vendor.id = po.vendor_id
                WHERE po.status IN ('Sent', 'PartiallyReceived', 'FullyReceived')
                  AND po.warehouse_id IS NOT NULL
                  AND NOT po.is_deleted
                  AND NOT vendor.is_deleted
                  AND NOT EXISTS (
                      SELECT 1
                      FROM inbound_orders inbound_order
                      WHERE inbound_order.purchase_order_id = po.id
                         OR (
                             inbound_order.organization_id = po.organization_id
                             AND inbound_order.order_number = po.po_number
                         )
                  );

                INSERT INTO inbound_order_lines (
                    id,
                    inbound_order_id,
                    purchase_order_line_id,
                    line_number,
                    product_id,
                    product_name,
                    product_sku,
                    location_id,
                    ordered_quantity,
                    received_quantity,
                    unit_of_measure,
                    lot_number,
                    expiry_date,
                    notes,
                    created_at,
                    updated_at,
                    is_deleted
                )
                SELECT
                    md5(random()::text || clock_timestamp()::text || line.id::text)::uuid,
                    inbound_order.id,
                    line.id,
                    (ROW_NUMBER() OVER (
                        PARTITION BY inbound_order.id
                        ORDER BY line.created_at, line.id
                    ))::integer,
                    line.product_variant_id,
                    line.description,
                    line.product_code,
                    NULL,
                    line.ordered_qty,
                    line.received_qty,
                    line.unit_of_measure,
                    NULL,
                    NULL,
                    NULL,
                    now(),
                    now(),
                    false
                FROM inbound_orders inbound_order
                JOIN purchase_order_lines line
                  ON line.purchase_order_id = inbound_order.purchase_order_id
                WHERE inbound_order.purchase_order_id IS NOT NULL
                  AND NOT inbound_order.is_deleted
                  AND NOT line.is_deleted
                  AND NOT EXISTS (
                      SELECT 1
                      FROM inbound_order_lines inbound_line
                      WHERE inbound_line.purchase_order_line_id = line.id
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts_payable_parameters");
        }
    }
}
