using ERPKeys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260614010000_AddGeneralLedgerParameters")]
public partial class AddGeneralLedgerParameters : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "reporting_currency_id",
            table: "ledgers",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "general_ledger_parameters",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                default_ledger_id = table.Column<Guid>(type: "uuid", nullable: false),
                default_financial_dimension_set_id = table.Column<Guid>(type: "uuid", nullable: true),
                retained_earnings_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                rounding_difference_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                realized_gain_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                realized_loss_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                unrealized_gain_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                unrealized_loss_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                allow_posting_to_closed_periods = table.Column<bool>(type: "boolean", nullable: false),
                require_dimensions_on_journal_lines = table.Column<bool>(type: "boolean", nullable: false),
                maximum_penny_difference = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                default_journal_type = table.Column<string>(
                    type: "character varying(50)", maxLength: 50, nullable: false),
                created_at = table.Column<DateTime>(
                    type: "timestamp without time zone", nullable: false),
                updated_at = table.Column<DateTime>(
                    type: "timestamp without time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_general_ledger_parameters", x => x.id);
                table.ForeignKey(
                    name: "fk_general_ledger_parameters_financial_dimension_sets_default_",
                    column: x => x.default_financial_dimension_set_id,
                    principalTable: "financial_dimension_sets",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_general_ledger_parameters_ledgers_default_ledger_id",
                    column: x => x.default_ledger_id,
                    principalTable: "ledgers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_general_ledger_parameters_accounts_realized_gain_account_id",
                    column: x => x.realized_gain_account_id,
                    principalTable: "accounts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_general_ledger_parameters_accounts_realized_loss_account_id",
                    column: x => x.realized_loss_account_id,
                    principalTable: "accounts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_general_ledger_parameters_accounts_retained_earnings_accoun",
                    column: x => x.retained_earnings_account_id,
                    principalTable: "accounts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_general_ledger_parameters_accounts_rounding_difference_acco",
                    column: x => x.rounding_difference_account_id,
                    principalTable: "accounts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_general_ledger_parameters_accounts_unrealized_gain_account_",
                    column: x => x.unrealized_gain_account_id,
                    principalTable: "accounts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_general_ledger_parameters_accounts_unrealized_loss_account_",
                    column: x => x.unrealized_loss_account_id,
                    principalTable: "accounts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.Sql(
            """
            INSERT INTO general_ledger_parameters
                (id, organization_id, default_ledger_id,
                 allow_posting_to_closed_periods, require_dimensions_on_journal_lines,
                 maximum_penny_difference, default_journal_type,
                 created_at, updated_at, is_deleted)
            SELECT
                md5(l.organization_id::text || ':gl-parameters')::uuid,
                l.organization_id,
                l.id,
                FALSE,
                FALSE,
                0.01,
                'General',
                NOW(),
                NOW(),
                FALSE
            FROM ledgers l
            WHERE l.is_default = TRUE
              AND l.is_deleted = FALSE
              AND NOT EXISTS (
                  SELECT 1
                  FROM general_ledger_parameters p
                  WHERE p.organization_id = l.organization_id
              );
            """);

        migrationBuilder.CreateIndex(
            name: "ix_ledgers_reporting_currency_id",
            table: "ledgers",
            column: "reporting_currency_id");

        migrationBuilder.CreateIndex(
            name: "ix_general_ledger_parameters_default_financial_dimension_set_id",
            table: "general_ledger_parameters",
            column: "default_financial_dimension_set_id");

        migrationBuilder.CreateIndex(
            name: "ix_general_ledger_parameters_default_ledger_id",
            table: "general_ledger_parameters",
            column: "default_ledger_id");

        migrationBuilder.CreateIndex(
            name: "ix_general_ledger_parameters_organization_id",
            table: "general_ledger_parameters",
            column: "organization_id",
            unique: true);

        foreach (var (name, column) in new[]
        {
            ("ix_general_ledger_parameters_realized_gain_account_id", "realized_gain_account_id"),
            ("ix_general_ledger_parameters_realized_loss_account_id", "realized_loss_account_id"),
            ("ix_general_ledger_parameters_retained_earnings_account_id", "retained_earnings_account_id"),
            ("ix_general_ledger_parameters_rounding_difference_account_id", "rounding_difference_account_id"),
            ("ix_general_ledger_parameters_unrealized_gain_account_id", "unrealized_gain_account_id"),
            ("ix_general_ledger_parameters_unrealized_loss_account_id", "unrealized_loss_account_id")
        })
        {
            migrationBuilder.CreateIndex(
                name: name,
                table: "general_ledger_parameters",
                column: column);
        }

        migrationBuilder.AddForeignKey(
            name: "fk_ledgers_currencies_reporting_currency_id",
            table: "ledgers",
            column: "reporting_currency_id",
            principalTable: "currencies",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "general_ledger_parameters");

        migrationBuilder.DropForeignKey(
            name: "fk_ledgers_currencies_reporting_currency_id",
            table: "ledgers");

        migrationBuilder.DropIndex(
            name: "ix_ledgers_reporting_currency_id",
            table: "ledgers");

        migrationBuilder.DropColumn(
            name: "reporting_currency_id",
            table: "ledgers");
    }
}
