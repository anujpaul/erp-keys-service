using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLedgerAccountingStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_accounts_organization_id_account_number",
                table: "accounts");

            migrationBuilder.AddColumn<Guid>(
                name: "ledger_id",
                table: "journal_entries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "chart_of_accounts_id",
                table: "accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "charts_of_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_charts_of_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ledgers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    functional_currency_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fiscal_calendar_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chart_of_accounts_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ledgers", x => x.id);
                    table.ForeignKey(
                        name: "fk_ledgers_charts_of_accounts_chart_of_accounts_id",
                        column: x => x.chart_of_accounts_id,
                        principalTable: "charts_of_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ledgers_currencies_functional_currency_id",
                        column: x => x.functional_currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ledgers_fiscal_calendars_fiscal_calendar_id",
                        column: x => x.fiscal_calendar_id,
                        principalTable: "fiscal_calendars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO charts_of_accounts
                    (id, organization_id, code, name, description, is_default, is_active,
                     created_at, updated_at, is_deleted)
                SELECT
                    md5(o.id::text || ':default-coa')::uuid,
                    o.id,
                    'CORP',
                    'Corporate Chart of Accounts',
                    'Default chart created during ledger migration',
                    TRUE,
                    TRUE,
                    NOW(),
                    NOW(),
                    FALSE
                FROM organizations o
                WHERE NOT EXISTS (
                    SELECT 1 FROM charts_of_accounts c WHERE c.organization_id = o.id
                );

                INSERT INTO ledgers
                    (id, organization_id, code, name, description,
                     functional_currency_id, fiscal_calendar_id, chart_of_accounts_id,
                     is_default, is_active, created_at, updated_at, is_deleted)
                SELECT
                    md5(o.id::text || ':default-ledger')::uuid,
                    o.id,
                    'CORP',
                    'Corporate Ledger',
                    'Default ledger created during ledger migration',
                    currency.id,
                    calendar.id,
                    chart.id,
                    TRUE,
                    TRUE,
                    NOW(),
                    NOW(),
                    FALSE
                FROM organizations o
                JOIN LATERAL (
                    SELECT c.id
                    FROM currencies c
                    WHERE c.organization_id = o.id AND c.is_deleted = FALSE
                    ORDER BY c.is_base DESC, c.created_at
                    LIMIT 1
                ) currency ON TRUE
                JOIN LATERAL (
                    SELECT fc.id
                    FROM fiscal_calendars fc
                    WHERE fc.organization_id = o.id AND fc.is_deleted = FALSE
                    ORDER BY fc.is_default DESC, fc.created_at
                    LIMIT 1
                ) calendar ON TRUE
                JOIN LATERAL (
                    SELECT coa.id
                    FROM charts_of_accounts coa
                    WHERE coa.organization_id = o.id AND coa.is_deleted = FALSE
                    ORDER BY coa.is_default DESC, coa.created_at
                    LIMIT 1
                ) chart ON TRUE
                WHERE NOT EXISTS (
                    SELECT 1 FROM ledgers l WHERE l.organization_id = o.id
                );

                UPDATE accounts a
                SET chart_of_accounts_id = c.id
                FROM charts_of_accounts c
                WHERE c.organization_id = a.organization_id
                  AND c.is_default = TRUE
                  AND c.is_deleted = FALSE;

                UPDATE journal_entries j
                SET ledger_id = l.id
                FROM ledgers l
                WHERE l.organization_id = j.organization_id
                  AND l.is_default = TRUE
                  AND l.is_deleted = FALSE;

                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM accounts WHERE chart_of_accounts_id = '00000000-0000-0000-0000-000000000000') THEN
                        RAISE EXCEPTION 'Ledger migration cannot map accounts because an organization has no chart of accounts.';
                    END IF;
                    IF EXISTS (SELECT 1 FROM journal_entries WHERE ledger_id = '00000000-0000-0000-0000-000000000000') THEN
                        RAISE EXCEPTION 'Ledger migration cannot map journal entries because an organization has no currency or fiscal calendar.';
                    END IF;
                END $$;
                """);

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_ledger_id",
                table: "journal_entries",
                column: "ledger_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_chart_of_accounts_id_account_number",
                table: "accounts",
                columns: new[] { "chart_of_accounts_id", "account_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_charts_of_accounts_organization_id",
                table: "charts_of_accounts",
                column: "organization_id",
                unique: true,
                filter: "is_default = TRUE AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_charts_of_accounts_organization_id_code",
                table: "charts_of_accounts",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_chart_of_accounts_id",
                table: "ledgers",
                column: "chart_of_accounts_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_fiscal_calendar_id",
                table: "ledgers",
                column: "fiscal_calendar_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_functional_currency_id",
                table: "ledgers",
                column: "functional_currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_organization_id",
                table: "ledgers",
                column: "organization_id",
                unique: true,
                filter: "is_default = TRUE AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_organization_id_code",
                table: "ledgers",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_charts_of_accounts_chart_of_accounts_id",
                table: "accounts",
                column: "chart_of_accounts_id",
                principalTable: "charts_of_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_journal_entries_ledgers_ledger_id",
                table: "journal_entries",
                column: "ledger_id",
                principalTable: "ledgers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_accounts_charts_of_accounts_chart_of_accounts_id",
                table: "accounts");

            migrationBuilder.DropForeignKey(
                name: "fk_journal_entries_ledgers_ledger_id",
                table: "journal_entries");

            migrationBuilder.DropTable(
                name: "ledgers");

            migrationBuilder.DropTable(
                name: "charts_of_accounts");

            migrationBuilder.DropIndex(
                name: "ix_journal_entries_ledger_id",
                table: "journal_entries");

            migrationBuilder.DropIndex(
                name: "ix_accounts_chart_of_accounts_id_account_number",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "ledger_id",
                table: "journal_entries");

            migrationBuilder.DropColumn(
                name: "chart_of_accounts_id",
                table: "accounts");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_organization_id_account_number",
                table: "accounts",
                columns: new[] { "organization_id", "account_number" },
                unique: true);
        }
    }
}
