using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccrualSchemes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accrual_schemes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ledger_id = table.Column<Guid>(type: "uuid", nullable: false),
                    debit_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credit_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    allocation_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    default_period_count = table.Column<int>(type: "integer", nullable: false),
                    financial_dimension_set_id = table.Column<Guid>(type: "uuid", nullable: true),
                    financial_dimension_value_ids_json = table.Column<string>(type: "jsonb", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accrual_schemes", x => x.id);
                    table.ForeignKey(
                        name: "fk_accrual_schemes_accounts_credit_account_id",
                        column: x => x.credit_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accrual_schemes_accounts_debit_account_id",
                        column: x => x.debit_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accrual_schemes_financial_dimension_sets_financial_dimensio",
                        column: x => x.financial_dimension_set_id,
                        principalTable: "financial_dimension_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accrual_schemes_ledgers_ledger_id",
                        column: x => x.ledger_id,
                        principalTable: "ledgers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accrual_posting_runs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accrual_scheme_id = table.Column<Guid>(type: "uuid", nullable: false),
                    posted_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    start_fiscal_period_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    posted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accrual_posting_runs", x => x.id);
                    table.ForeignKey(
                        name: "fk_accrual_posting_runs_accrual_schemes_accrual_scheme_id",
                        column: x => x.accrual_scheme_id,
                        principalTable: "accrual_schemes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accrual_scheme_allocations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accrual_scheme_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_offset = table.Column<int>(type: "integer", nullable: false),
                    percentage = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accrual_scheme_allocations", x => x.id);
                    table.ForeignKey(
                        name: "fk_accrual_scheme_allocations_accrual_schemes_accrual_scheme_id",
                        column: x => x.accrual_scheme_id,
                        principalTable: "accrual_schemes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accrual_posting_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accrual_posting_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fiscal_period_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_offset = table.Column<int>(type: "integer", nullable: false),
                    percentage = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accrual_posting_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_accrual_posting_lines_accrual_posting_runs_accrual_posting_",
                        column: x => x.accrual_posting_run_id,
                        principalTable: "accrual_posting_runs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_accrual_posting_lines_fiscal_periods_fiscal_period_id",
                        column: x => x.fiscal_period_id,
                        principalTable: "fiscal_periods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accrual_posting_lines_journal_entries_journal_entry_id",
                        column: x => x.journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_lines_accrual_posting_run_id_period_offset",
                table: "accrual_posting_lines",
                columns: new[] { "accrual_posting_run_id", "period_offset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_lines_fiscal_period_id",
                table: "accrual_posting_lines",
                column: "fiscal_period_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_lines_journal_entry_id",
                table: "accrual_posting_lines",
                column: "journal_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_runs_accrual_scheme_id",
                table: "accrual_posting_runs",
                column: "accrual_scheme_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_runs_organization_id_accrual_scheme_id_refe",
                table: "accrual_posting_runs",
                columns: new[] { "organization_id", "accrual_scheme_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accrual_scheme_allocations_accrual_scheme_id_period_offset",
                table: "accrual_scheme_allocations",
                columns: new[] { "accrual_scheme_id", "period_offset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_credit_account_id",
                table: "accrual_schemes",
                column: "credit_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_debit_account_id",
                table: "accrual_schemes",
                column: "debit_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_financial_dimension_set_id",
                table: "accrual_schemes",
                column: "financial_dimension_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_ledger_id",
                table: "accrual_schemes",
                column: "ledger_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_organization_id_code",
                table: "accrual_schemes",
                columns: new[] { "organization_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accrual_posting_lines");

            migrationBuilder.DropTable(
                name: "accrual_scheme_allocations");

            migrationBuilder.DropTable(
                name: "accrual_posting_runs");

            migrationBuilder.DropTable(
                name: "accrual_schemes");

        }
    }
}
