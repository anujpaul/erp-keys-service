using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinancialDimension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "financial_dimension_set_id",
                table: "journal_lines",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "financial_dimension_sets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_dimension_sets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "financial_dimensions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_dimensions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "financial_dimension_set_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    financial_dimension_set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    financial_dimension_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_dimension_set_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_financial_dimension_set_members_financial_dimension_sets_fi",
                        column: x => x.financial_dimension_set_id,
                        principalTable: "financial_dimension_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_financial_dimension_set_members_financial_dimensions_financ",
                        column: x => x.financial_dimension_id,
                        principalTable: "financial_dimensions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "financial_dimension_values",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    financial_dimension_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_dimension_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_financial_dimension_values_financial_dimensions_financial_d",
                        column: x => x.financial_dimension_id,
                        principalTable: "financial_dimensions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_line_dimension_values",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    financial_dimension_value_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_line_dimension_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_journal_line_dimension_values_financial_dimension_values_fi",
                        column: x => x.financial_dimension_value_id,
                        principalTable: "financial_dimension_values",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_line_dimension_values_journal_lines_journal_line_id",
                        column: x => x.journal_line_id,
                        principalTable: "journal_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_journal_lines_financial_dimension_set_id",
                table: "journal_lines",
                column: "financial_dimension_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_set_members_financial_dimension_id",
                table: "financial_dimension_set_members",
                column: "financial_dimension_id");

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_set_members_financial_dimension_set_id_",
                table: "financial_dimension_set_members",
                columns: new[] { "financial_dimension_set_id", "financial_dimension_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_sets_organization_id",
                table: "financial_dimension_sets",
                column: "organization_id",
                unique: true,
                filter: "is_default = TRUE AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_sets_organization_id_name",
                table: "financial_dimension_sets",
                columns: new[] { "organization_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_values_financial_dimension_id_code",
                table: "financial_dimension_values",
                columns: new[] { "financial_dimension_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimensions_organization_id_code",
                table: "financial_dimensions",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_journal_line_dimension_values_financial_dimension_value_id",
                table: "journal_line_dimension_values",
                column: "financial_dimension_value_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_line_dimension_values_journal_line_id_financial_dim",
                table: "journal_line_dimension_values",
                columns: new[] { "journal_line_id", "financial_dimension_value_id" },
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT INTO financial_dimensions
                    (id, organization_id, code, name, description, is_active,
                     created_at, updated_at, is_deleted)
                SELECT
                    md5(o.id::text || ':financial-dimension:' || seed.code)::uuid,
                    o.id,
                    seed.code,
                    seed.name,
                    seed.description,
                    TRUE,
                    CURRENT_TIMESTAMP,
                    CURRENT_TIMESTAMP,
                    FALSE
                FROM organizations AS o
                CROSS JOIN (VALUES
                    ('COST_CENTER', 'Cost Center', 'Responsibility center used to track costs and profitability'),
                    ('DEPARTMENT', 'Department', 'Organizational department responsible for the transaction'),
                    ('BUSINESS_UNIT', 'Business Unit', 'Business or operating unit responsible for the transaction'),
                    ('PROJECT', 'Project', 'Project, initiative, or internal work stream')
                ) AS seed(code, name, description)
                WHERE o.is_deleted = FALSE;

                INSERT INTO financial_dimension_sets
                    (id, organization_id, name, description, is_default, is_active,
                     created_at, updated_at, is_deleted)
                SELECT
                    md5(o.id::text || ':financial-dimension-set:OPERATING')::uuid,
                    o.id,
                    'Operating Dimensions',
                    'Standard dimensions used for operating ledger entries',
                    TRUE,
                    TRUE,
                    CURRENT_TIMESTAMP,
                    CURRENT_TIMESTAMP,
                    FALSE
                FROM organizations AS o
                WHERE o.is_deleted = FALSE;

                INSERT INTO financial_dimension_set_members
                    (id, financial_dimension_set_id, financial_dimension_id,
                     display_order, is_required, created_at, updated_at, is_deleted)
                SELECT
                    md5(o.id::text || ':financial-dimension-member:' || seed.code)::uuid,
                    md5(o.id::text || ':financial-dimension-set:OPERATING')::uuid,
                    md5(o.id::text || ':financial-dimension:' || seed.code)::uuid,
                    seed.display_order,
                    seed.is_required,
                    CURRENT_TIMESTAMP,
                    CURRENT_TIMESTAMP,
                    FALSE
                FROM organizations AS o
                CROSS JOIN (VALUES
                    ('COST_CENTER', 1, TRUE),
                    ('DEPARTMENT', 2, FALSE),
                    ('BUSINESS_UNIT', 3, FALSE),
                    ('PROJECT', 4, FALSE)
                ) AS seed(code, display_order, is_required)
                WHERE o.is_deleted = FALSE;
                """);

            migrationBuilder.AddForeignKey(
                name: "fk_journal_lines_financial_dimension_sets_financial_dimension_",
                table: "journal_lines",
                column: "financial_dimension_set_id",
                principalTable: "financial_dimension_sets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_journal_lines_financial_dimension_sets_financial_dimension_",
                table: "journal_lines");

            migrationBuilder.DropTable(
                name: "financial_dimension_set_members");

            migrationBuilder.DropTable(
                name: "journal_line_dimension_values");

            migrationBuilder.DropTable(
                name: "financial_dimension_sets");

            migrationBuilder.DropTable(
                name: "financial_dimension_values");

            migrationBuilder.DropTable(
                name: "financial_dimensions");

            migrationBuilder.DropIndex(
                name: "ix_journal_lines_financial_dimension_set_id",
                table: "journal_lines");

            migrationBuilder.DropColumn(
                name: "financial_dimension_set_id",
                table: "journal_lines");
        }
    }
}
