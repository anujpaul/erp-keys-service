using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRagDocumentMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "content_hash",
                table: "document_chunks",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "required_permission",
                table: "document_chunks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_document_chunks_organization_id_content_hash",
                table: "document_chunks",
                columns: new[] { "organization_id", "content_hash" });

            migrationBuilder.CreateIndex(
                name: "ix_document_chunks_organization_id_required_permission",
                table: "document_chunks",
                columns: new[] { "organization_id", "required_permission" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_document_chunks_organization_id_content_hash",
                table: "document_chunks");

            migrationBuilder.DropIndex(
                name: "ix_document_chunks_organization_id_required_permission",
                table: "document_chunks");

            migrationBuilder.DropColumn(
                name: "content_hash",
                table: "document_chunks");

            migrationBuilder.DropColumn(
                name: "required_permission",
                table: "document_chunks");
        }
    }
}
