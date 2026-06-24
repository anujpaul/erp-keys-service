using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRagDocumentUploader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "uploaded_by_user_id",
                table: "document_chunks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_document_chunks_organization_id_uploaded_by_user_id",
                table: "document_chunks",
                columns: new[] { "organization_id", "uploaded_by_user_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_document_chunks_organization_id_uploaded_by_user_id",
                table: "document_chunks");

            migrationBuilder.DropColumn(
                name: "uploaded_by_user_id",
                table: "document_chunks");
        }
    }
}
