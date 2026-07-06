using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureAccountsReceivablePostingAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "bank_account_id",
                table: "accounts_receivable_parameters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "cash_account_id",
                table: "accounts_receivable_parameters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "cost_of_goods_sold_account_id",
                table: "accounts_receivable_parameters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "inventory_account_id",
                table: "accounts_receivable_parameters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sales_revenue_account_id",
                table: "accounts_receivable_parameters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sales_tax_payable_account_id",
                table: "accounts_receivable_parameters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "trade_receivable_account_id",
                table: "accounts_receivable_parameters",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_bank_account_id",
                table: "accounts_receivable_parameters",
                column: "bank_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_cash_account_id",
                table: "accounts_receivable_parameters",
                column: "cash_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_cost_of_goods_sold_account_id",
                table: "accounts_receivable_parameters",
                column: "cost_of_goods_sold_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_inventory_account_id",
                table: "accounts_receivable_parameters",
                column: "inventory_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_sales_revenue_account_id",
                table: "accounts_receivable_parameters",
                column: "sales_revenue_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_sales_tax_payable_account_id",
                table: "accounts_receivable_parameters",
                column: "sales_tax_payable_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_trade_receivable_account_id",
                table: "accounts_receivable_parameters",
                column: "trade_receivable_account_id");

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_bank_account_id",
                table: "accounts_receivable_parameters",
                column: "bank_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_cash_account_id",
                table: "accounts_receivable_parameters",
                column: "cash_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_cost_of_goods_sold_",
                table: "accounts_receivable_parameters",
                column: "cost_of_goods_sold_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_inventory_account_id",
                table: "accounts_receivable_parameters",
                column: "inventory_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_sales_revenue_accou",
                table: "accounts_receivable_parameters",
                column: "sales_revenue_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_sales_tax_payable_a",
                table: "accounts_receivable_parameters",
                column: "sales_tax_payable_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_trade_receivable_ac",
                table: "accounts_receivable_parameters",
                column: "trade_receivable_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_bank_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_cash_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_cost_of_goods_sold_",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_inventory_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_sales_revenue_accou",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_sales_tax_payable_a",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropForeignKey(
                name: "fk_accounts_receivable_parameters_accounts_trade_receivable_ac",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropIndex(
                name: "ix_accounts_receivable_parameters_bank_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropIndex(
                name: "ix_accounts_receivable_parameters_cash_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropIndex(
                name: "ix_accounts_receivable_parameters_cost_of_goods_sold_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropIndex(
                name: "ix_accounts_receivable_parameters_inventory_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropIndex(
                name: "ix_accounts_receivable_parameters_sales_revenue_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropIndex(
                name: "ix_accounts_receivable_parameters_sales_tax_payable_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropIndex(
                name: "ix_accounts_receivable_parameters_trade_receivable_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropColumn(
                name: "bank_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropColumn(
                name: "cash_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropColumn(
                name: "cost_of_goods_sold_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropColumn(
                name: "inventory_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropColumn(
                name: "sales_revenue_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropColumn(
                name: "sales_tax_payable_account_id",
                table: "accounts_receivable_parameters");

            migrationBuilder.DropColumn(
                name: "trade_receivable_account_id",
                table: "accounts_receivable_parameters");
        }
    }
}
