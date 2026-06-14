using System;
using ERPKeys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260614020000_AddGeneralJournalVoucherTemplates")]
public partial class AddGeneralJournalVoucherTemplates : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "general_journal_voucher_templates",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                ledger_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(
                    type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(
                    type: "character varying(500)", maxLength: 500, nullable: false),
                reference = table.Column<string>(
                    type: "character varying(100)", maxLength: 100, nullable: false),
                journal_type = table.Column<string>(
                    type: "character varying(50)", maxLength: 50, nullable: false),
                lines_json = table.Column<string>(type: "jsonb", nullable: false),
                created_at = table.Column<DateTime>(
                    type: "timestamp without time zone", nullable: false),
                updated_at = table.Column<DateTime>(
                    type: "timestamp without time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_general_journal_voucher_templates", x => x.id);
                table.ForeignKey(
                    name: "fk_general_journal_voucher_templates_ledgers_ledger_id",
                    column: x => x.ledger_id,
                    principalTable: "ledgers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_general_journal_voucher_templates_ledger_id",
            table: "general_journal_voucher_templates",
            column: "ledger_id");

        migrationBuilder.CreateIndex(
            name: "ix_general_journal_voucher_templates_organization_id_user_id_n",
            table: "general_journal_voucher_templates",
            columns: new[] { "organization_id", "user_id", "name" },
            unique: true,
            filter: "is_deleted = FALSE");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "general_journal_voucher_templates");
    }
}
