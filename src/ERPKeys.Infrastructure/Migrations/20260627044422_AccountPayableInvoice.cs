using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccountPayableInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "purchase_order_line_id",
                table: "inbound_order_lines",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_inbound_orders_purchase_order_id",
                table: "inbound_orders",
                column: "purchase_order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inbound_order_lines_purchase_order_line_id",
                table: "inbound_order_lines",
                column: "purchase_order_line_id");

            migrationBuilder.AddForeignKey(
                name: "fk_inbound_order_lines_purchase_order_lines_purchase_order_lin",
                table: "inbound_order_lines",
                column: "purchase_order_line_id",
                principalTable: "purchase_order_lines",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_inbound_order_lines_purchase_order_lines_purchase_order_lin",
                table: "inbound_order_lines");

            migrationBuilder.DropIndex(
                name: "ix_inbound_orders_purchase_order_id",
                table: "inbound_orders");

            migrationBuilder.DropIndex(
                name: "ix_inbound_order_lines_purchase_order_line_id",
                table: "inbound_order_lines");

            migrationBuilder.DropColumn(
                name: "purchase_order_line_id",
                table: "inbound_order_lines");
        }
    }
}
