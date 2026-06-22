using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferredOrganizationToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "preferred_organization_id",
                table: "app_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_users_preferred_organization_id",
                table: "app_users",
                column: "preferred_organization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_app_users_organizations_preferred_organization_id",
                table: "app_users",
                column: "preferred_organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_app_users_organizations_preferred_organization_id",
                table: "app_users");

            migrationBuilder.DropIndex(
                name: "ix_app_users_preferred_organization_id",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "preferred_organization_id",
                table: "app_users");
        }
    }
}
