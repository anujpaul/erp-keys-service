using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantAttributeDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "variant_attribute_definition_id",
                table: "catalog_products",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "variant_attribute_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_variant_attribute_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "variant_attribute_values",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    variant_attribute_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribute_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_variant_attribute_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_variant_attribute_values_variant_attribute_definitions_vari",
                        column: x => x.variant_attribute_definition_id,
                        principalTable: "variant_attribute_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_catalog_products_variant_attribute_definition_id",
                table: "catalog_products",
                column: "variant_attribute_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_variant_attribute_definitions_organization_id_code",
                table: "variant_attribute_definitions",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_variant_attribute_values_variant_attribute_definition_id_at",
                table: "variant_attribute_values",
                columns: new[] { "variant_attribute_definition_id", "attribute_type", "value" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_catalog_products_variant_attribute_definitions_variant_attr",
                table: "catalog_products",
                column: "variant_attribute_definition_id",
                principalTable: "variant_attribute_definitions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_catalog_products_variant_attribute_definitions_variant_attr",
                table: "catalog_products");

            migrationBuilder.DropTable(
                name: "variant_attribute_values");

            migrationBuilder.DropTable(
                name: "variant_attribute_definitions");

            migrationBuilder.DropIndex(
                name: "ix_catalog_products_variant_attribute_definition_id",
                table: "catalog_products");

            migrationBuilder.DropColumn(
                name: "variant_attribute_definition_id",
                table: "catalog_products");
        }
    }
}
