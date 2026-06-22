using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address_line1",
                table: "app_users",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_line2",
                table: "app_users",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "app_users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "app_users",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                table: "app_users",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "app_users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address_line1",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "address_line2",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "city",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "country",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "postal_code",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "state",
                table: "app_users");
        }
    }
}
