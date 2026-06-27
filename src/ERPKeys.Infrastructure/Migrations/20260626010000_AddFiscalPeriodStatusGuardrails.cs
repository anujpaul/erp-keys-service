using ERPKeys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260626010000_AddFiscalPeriodStatusGuardrails")]
    public partial class AddFiscalPeriodStatusGuardrails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE fiscal_periods
                SET status = 'Closed'
                WHERE status = 'OnHold';
                """);

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_periods_fiscal_year_id_period_number",
                table: "fiscal_periods",
                columns: new[] { "fiscal_year_id", "period_number" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_fiscal_periods_period_number_range",
                table: "fiscal_periods",
                sql: "period_number BETWEEN 1 AND 13");

            migrationBuilder.AddCheckConstraint(
                name: "ck_fiscal_periods_status",
                table: "fiscal_periods",
                sql: "status IN ('Open', 'Closed', 'PermanentlyClosed')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_fiscal_periods_status",
                table: "fiscal_periods");

            migrationBuilder.DropCheckConstraint(
                name: "ck_fiscal_periods_period_number_range",
                table: "fiscal_periods");

            migrationBuilder.DropIndex(
                name: "ix_fiscal_periods_fiscal_year_id_period_number",
                table: "fiscal_periods");
        }
    }
}
