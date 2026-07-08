using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGLreverseOnVoid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "reversal_of_journal_entry_id",
                table: "journal_entries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "reversed_by_journal_entry_id",
                table: "journal_entries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_reversal_of_journal_entry_id",
                table: "journal_entries",
                column: "reversal_of_journal_entry_id",
                unique: true,
                filter: "reversal_of_journal_entry_id IS NOT NULL AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_reversed_by_journal_entry_id",
                table: "journal_entries",
                column: "reversed_by_journal_entry_id",
                unique: true,
                filter: "reversed_by_journal_entry_id IS NOT NULL AND is_deleted = FALSE");

            migrationBuilder.AddForeignKey(
                name: "fk_journal_entries_journal_entries_reversal_of_journal_entry_id",
                table: "journal_entries",
                column: "reversal_of_journal_entry_id",
                principalTable: "journal_entries",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_journal_entries_journal_entries_reversed_by_journal_entry_id",
                table: "journal_entries",
                column: "reversed_by_journal_entry_id",
                principalTable: "journal_entries",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_journal_entries_journal_entries_reversal_of_journal_entry_id",
                table: "journal_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_journal_entries_journal_entries_reversed_by_journal_entry_id",
                table: "journal_entries");

            migrationBuilder.DropIndex(
                name: "ix_journal_entries_reversal_of_journal_entry_id",
                table: "journal_entries");

            migrationBuilder.DropIndex(
                name: "ix_journal_entries_reversed_by_journal_entry_id",
                table: "journal_entries");

            migrationBuilder.DropColumn(
                name: "reversal_of_journal_entry_id",
                table: "journal_entries");

            migrationBuilder.DropColumn(
                name: "reversed_by_journal_entry_id",
                table: "journal_entries");
        }
    }
}
