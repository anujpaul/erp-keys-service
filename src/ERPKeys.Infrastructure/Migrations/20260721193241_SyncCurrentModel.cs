using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncCurrentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ix_journal_entries_organization_id_source_module_source_document_type_source_document_id",
                table: "journal_entries",
                newName: "ix_journal_entries_organization_id_source_module_source_docume");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ix_journal_entries_organization_id_source_module_source_docume",
                table: "journal_entries",
                newName: "ix_journal_entries_organization_id_source_module_source_document_type_source_document_id");
        }
    }
}
