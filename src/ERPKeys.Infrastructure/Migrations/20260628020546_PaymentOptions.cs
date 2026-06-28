using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payment_processor_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    provider_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    environment = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    endpoint_base_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    merchant_account_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    credential_secret_reference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    timeout_seconds = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_processor_configurations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "methods_of_payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    usage = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tender_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    processor_configuration_id = table.Column<Guid>(type: "uuid", nullable: true),
                    settlement_bank_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    clearing_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fee_expense_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    requires_external_authorization = table.Column<bool>(type: "boolean", nullable: false),
                    auto_capture = table.Column<bool>(type: "boolean", nullable: false),
                    allow_refunds = table.Column<bool>(type: "boolean", nullable: false),
                    allow_manual_entry = table.Column<bool>(type: "boolean", nullable: false),
                    settlement_mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    settlement_delay_days = table.Column<int>(type: "integer", nullable: false),
                    settlement_cutoff_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_methods_of_payment", x => x.id);
                    table.ForeignKey(
                        name: "fk_methods_of_payment_accounts_clearing_account_id",
                        column: x => x.clearing_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_methods_of_payment_accounts_fee_expense_account_id",
                        column: x => x.fee_expense_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_methods_of_payment_bank_accounts_settlement_bank_account_id",
                        column: x => x.settlement_bank_account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_methods_of_payment_payment_processor_configurations_process",
                        column: x => x.processor_configuration_id,
                        principalTable: "payment_processor_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_clearing_account_id",
                table: "methods_of_payment",
                column: "clearing_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_fee_expense_account_id",
                table: "methods_of_payment",
                column: "fee_expense_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_organization_id_code",
                table: "methods_of_payment",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_processor_configuration_id",
                table: "methods_of_payment",
                column: "processor_configuration_id");

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_settlement_bank_account_id",
                table: "methods_of_payment",
                column: "settlement_bank_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_processor_configurations_organization_id_code",
                table: "payment_processor_configurations",
                columns: new[] { "organization_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "methods_of_payment");

            migrationBuilder.DropTable(
                name: "payment_processor_configurations");
        }
    }
}
