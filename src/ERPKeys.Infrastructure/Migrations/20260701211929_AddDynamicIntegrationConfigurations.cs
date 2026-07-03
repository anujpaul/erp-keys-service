using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicIntegrationConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "integration_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    service_category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    connector_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    field_definitions_json = table.Column<string>(type: "jsonb", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_configured = table.Column<bool>(type: "boolean", nullable: false),
                    settings_json = table.Column<string>(type: "jsonb", nullable: false),
                    encrypted_secrets = table.Column<string>(type: "text", nullable: true),
                    pending_settings_json = table.Column<string>(type: "jsonb", nullable: true),
                    pending_encrypted_secrets = table.Column<string>(type: "text", nullable: true),
                    review_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    submitted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    reviewed_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    review_notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_integration_configurations", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_integration_configurations_organization_id_code",
                table: "integration_configurations",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "NOT is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_integration_configurations_organization_id_service_category",
                table: "integration_configurations",
                columns: new[] { "organization_id", "service_category" },
                unique: true,
                filter: "is_enabled AND NOT is_deleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "integration_configurations");
        }
    }
}
