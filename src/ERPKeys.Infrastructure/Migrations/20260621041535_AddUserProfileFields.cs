using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "department",
                table: "app_users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "employee_id",
                table: "app_users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "job_title",
                table: "app_users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "locale",
                table: "app_users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "app_users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "timezone",
                table: "app_users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_users_organization_id_employee_id",
                table: "app_users",
                columns: new[] { "organization_id", "employee_id" },
                unique: true,
                filter: "employee_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_app_users_organization_id_employee_id",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "department",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "employee_id",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "job_title",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "locale",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "timezone",
                table: "app_users");
        }
    }
}
