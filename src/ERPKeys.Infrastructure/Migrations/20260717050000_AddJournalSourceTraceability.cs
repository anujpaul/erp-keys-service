using ERPKeys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260717050000_AddJournalSourceTraceability")]
public partial class AddJournalSourceTraceability : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "source_document_id", table: "journal_entries",
            type: "uuid", nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "source_document_number", table: "journal_entries",
            type: "character varying(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "source_document_type", table: "journal_entries",
            type: "character varying(50)", maxLength: 50, nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "source_module", table: "journal_entries",
            type: "character varying(50)", maxLength: 50, nullable: true);

        migrationBuilder.Sql(
            """
            UPDATE journal_entries AS journal
            SET source_module = 'AccountsReceivable',
                source_document_type = 'ARInvoice',
                source_document_id = invoice.id,
                source_document_number = invoice.invoice_number
            FROM ar_invoices AS invoice
            WHERE invoice.journal_entry_id = journal.id
              AND journal.source_document_id IS NULL;

            UPDATE journal_entries AS journal
            SET source_module = 'AccountsReceivable',
                source_document_type = 'ARPayment',
                source_document_id = payment.id,
                source_document_number = payment.payment_number
            FROM ar_payments AS payment
            WHERE payment.journal_entry_id = journal.id
              AND journal.source_document_id IS NULL;

            UPDATE journal_entries AS journal
            SET source_module = 'AccountsPayable',
                source_document_type = 'APInvoice',
                source_document_id = invoice.id,
                source_document_number = invoice.invoice_number
            FROM ap_invoices AS invoice
            WHERE invoice.journal_entry_id = journal.id
              AND journal.source_document_id IS NULL;

            UPDATE journal_entries AS journal
            SET source_module = 'AccountsPayable',
                source_document_type = 'APPayment',
                source_document_id = payment.id,
                source_document_number = payment.payment_number
            FROM ap_payments AS payment
            WHERE payment.journal_entry_id = journal.id
              AND journal.source_document_id IS NULL;

            UPDATE journal_entries AS journal
            SET source_module = 'AccountsReceivable',
                source_document_type = 'SalesOrder',
                source_document_id = sales_order.id,
                source_document_number = sales_order.order_number
            FROM sales_orders AS sales_order
            WHERE journal.reference = sales_order.order_number
              AND journal.journal_type = 'Inventory'
              AND journal.source_document_id IS NULL;
            """);

        migrationBuilder.CreateIndex(
            name: "ix_journal_entries_organization_id_source_module_source_document_type_source_document_id",
            table: "journal_entries",
            columns: new[] { "organization_id", "source_module", "source_document_type", "source_document_id" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_journal_entries_organization_id_source_module_source_document_type_source_document_id",
            table: "journal_entries");
        migrationBuilder.DropColumn(name: "source_document_id", table: "journal_entries");
        migrationBuilder.DropColumn(name: "source_document_number", table: "journal_entries");
        migrationBuilder.DropColumn(name: "source_document_type", table: "journal_entries");
        migrationBuilder.DropColumn(name: "source_module", table: "journal_entries");
    }
}
