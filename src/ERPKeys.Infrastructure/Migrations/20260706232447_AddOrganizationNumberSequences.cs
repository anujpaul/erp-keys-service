using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationNumberSequences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "number_sequences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    area = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    display_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    prefix = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    include_year = table.Column<bool>(type: "boolean", nullable: false),
                    separator = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    padding = table.Column<int>(type: "integer", nullable: false),
                    next_number = table.Column<long>(type: "bigint", nullable: false),
                    allow_manual_override = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_number_sequences", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_number_sequences_organization_id_area",
                table: "number_sequences",
                columns: new[] { "organization_id", "area" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_number_sequences_organization_id_display_name",
                table: "number_sequences",
                columns: new[] { "organization_id", "display_name" });

            migrationBuilder.Sql("""
                INSERT INTO number_sequences (
                    id, organization_id, area, display_name, prefix, include_year,
                    separator, padding, next_number, allow_manual_override, is_active,
                    created_at, updated_at, is_deleted)
                SELECT
                    (substr(md5(o.id::text || ':' || defaults.area), 1, 8) || '-' ||
                     substr(md5(o.id::text || ':' || defaults.area), 9, 4) || '-' ||
                     substr(md5(o.id::text || ':' || defaults.area), 13, 4) || '-' ||
                     substr(md5(o.id::text || ':' || defaults.area), 17, 4) || '-' ||
                     substr(md5(o.id::text || ':' || defaults.area), 21, 12))::uuid,
                    o.id,
                    defaults.area,
                    defaults.display_name,
                    defaults.prefix,
                    defaults.include_year,
                    '-',
                    defaults.padding,
                    CASE defaults.area
                        WHEN 'customer' THEN (SELECT count(*) + 1 FROM customers x WHERE x.organization_id = o.id)
                        WHEN 'vendor' THEN (SELECT count(*) + 1 FROM vendors x WHERE x.organization_id = o.id)
                        WHEN 'sales-order' THEN (SELECT count(*) + 1 FROM sales_orders x WHERE x.organization_id = o.id)
                        WHEN 'sales-quotation' THEN (SELECT count(*) + 1 FROM sales_quotations x WHERE x.organization_id = o.id)
                        WHEN 'ar-invoice' THEN (SELECT count(*) + 1 FROM ar_invoices x WHERE x.organization_id = o.id)
                        WHEN 'ar-payment' THEN (SELECT count(*) + 1 FROM ar_payments x WHERE x.organization_id = o.id)
                        WHEN 'customer-credit-note' THEN (SELECT count(*) + 1 FROM customer_credit_notes x WHERE x.organization_id = o.id)
                        WHEN 'dunning' THEN (SELECT count(*) + 1 FROM dunning_records x WHERE x.organization_id = o.id)
                        WHEN 'purchase-order' THEN (SELECT count(*) + 1 FROM purchase_orders x WHERE x.organization_id = o.id)
                        WHEN 'purchase-requisition' THEN (SELECT count(*) + 1 FROM purchase_requisitions x WHERE x.organization_id = o.id)
                        WHEN 'goods-receipt' THEN (SELECT count(*) + 1 FROM purchase_order_receipts x WHERE x.organization_id = o.id)
                        WHEN 'ap-invoice' THEN (SELECT count(*) + 1 FROM ap_invoices x WHERE x.organization_id = o.id)
                        WHEN 'ap-payment' THEN (SELECT count(*) + 1 FROM ap_payments x WHERE x.organization_id = o.id)
                        WHEN 'payment-proposal' THEN (SELECT count(*) + 1 FROM payment_proposals x WHERE x.organization_id = o.id)
                        WHEN 'vendor-credit-note' THEN (SELECT count(*) + 1 FROM vendor_credit_notes x WHERE x.organization_id = o.id)
                        WHEN 'journal-entry' THEN (SELECT count(*) + 1 FROM journal_entries x WHERE x.organization_id = o.id)
                        WHEN 'cash-journal' THEN (SELECT count(*) + 1 FROM cash_journals x WHERE x.organization_id = o.id)
                        WHEN 'bank-transaction' THEN (SELECT count(*) + 1 FROM bank_transactions x WHERE x.organization_id = o.id)
                        WHEN 'bank-reconciliation' THEN (SELECT count(*) + 1 FROM bank_reconciliations x WHERE x.organization_id = o.id)
                        WHEN 'expense-report' THEN (SELECT count(*) + 1 FROM expense_reports x WHERE x.organization_id = o.id)
                        ELSE 1
                    END,
                    false,
                    true,
                    now(),
                    now(),
                    false
                FROM organizations o
                CROSS JOIN (VALUES
                    ('customer', 'Customer', 'CUST', false, 5),
                    ('vendor', 'Vendor', 'VEND', false, 5),
                    ('sales-order', 'Sales order', 'SO', true, 5),
                    ('sales-quotation', 'Sales quotation', 'QUO', true, 5),
                    ('ar-invoice', 'Customer invoice', 'INV', false, 6),
                    ('ar-payment', 'Customer receipt', 'RCPT', false, 6),
                    ('customer-credit-note', 'Customer credit note', 'CN', false, 6),
                    ('dunning', 'Dunning notice', 'DUN', false, 5),
                    ('purchase-order', 'Purchase order', 'PO', true, 5),
                    ('purchase-requisition', 'Purchase requisition', 'PR', true, 5),
                    ('goods-receipt', 'Goods receipt', 'GRN', false, 6),
                    ('ap-invoice', 'Vendor invoice', 'APINV', false, 6),
                    ('ap-payment', 'Vendor payment', 'PMT', false, 6),
                    ('payment-proposal', 'Payment proposal', 'PAY', true, 5),
                    ('vendor-credit-note', 'Vendor credit note', 'VCN', false, 6),
                    ('journal-entry', 'Journal entry', 'JE', false, 6),
                    ('cash-journal', 'Cash journal', 'CJ', false, 6),
                    ('bank-transaction', 'Bank transaction', 'TXN', false, 7),
                    ('bank-reconciliation', 'Bank reconciliation', 'REC', false, 5),
                    ('expense-report', 'Expense report', 'EXP', false, 4)
                ) AS defaults(area, display_name, prefix, include_year, padding)
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM number_sequences existing
                    WHERE existing.organization_id = o.id
                      AND existing.area = defaults.area
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "number_sequences");
        }
    }
}
