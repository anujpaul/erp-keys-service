using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAPInvoiceLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ap_invoice_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ap_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ap_invoice_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_ap_invoice_lines_ap_invoices_ap_invoice_id",
                        column: x => x.ap_invoice_id,
                        principalTable: "ap_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ap_invoice_lines_purchase_order_lines_purchase_order_line_id",
                        column: x => x.purchase_order_line_id,
                        principalTable: "purchase_order_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                WITH invoice_allocations AS (
                    SELECT
                        i.id AS invoice_id,
                        pol.id AS purchase_order_line_id,
                        pol.received_qty,
                        pol.unit_cost,
                        pol.tax_rate,
                        i.total_amount,
                        SUM(
                            pol.received_qty * pol.unit_cost * (1 + pol.tax_rate / 100.0)
                        ) OVER (PARTITION BY i.id) AS received_total
                    FROM ap_invoices i
                    JOIN purchase_order_lines pol
                      ON pol.purchase_order_id = i.purchase_order_id
                    WHERE i.purchase_order_id IS NOT NULL
                      AND i.invoice_type = 'Standard'
                      AND i.status <> 'Voided'
                      AND NOT i.is_deleted
                      AND NOT pol.is_deleted
                      AND pol.received_qty > 0
                )
                INSERT INTO ap_invoice_lines (
                    id,
                    ap_invoice_id,
                    purchase_order_line_id,
                    quantity,
                    unit_cost,
                    tax_rate,
                    created_at,
                    updated_at,
                    is_deleted
                )
                SELECT
                    md5(random()::text || clock_timestamp()::text || invoice_id::text || purchase_order_line_id::text)::uuid,
                    invoice_id,
                    purchase_order_line_id,
                    ROUND(
                        received_qty * LEAST(1, total_amount / NULLIF(received_total, 0)),
                        4
                    ),
                    unit_cost,
                    tax_rate,
                    now(),
                    now(),
                    false
                FROM invoice_allocations
                WHERE received_total > 0
                  AND total_amount > 0;
                """);

            migrationBuilder.CreateIndex(
                name: "ix_ap_invoice_lines_ap_invoice_id_purchase_order_line_id",
                table: "ap_invoice_lines",
                columns: new[] { "ap_invoice_id", "purchase_order_line_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ap_invoice_lines_purchase_order_line_id",
                table: "ap_invoice_lines",
                column: "purchase_order_line_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ap_invoice_lines");

        }
    }
}
