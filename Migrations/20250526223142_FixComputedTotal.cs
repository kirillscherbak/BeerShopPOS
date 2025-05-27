using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeerShopPOS.Migrations
{
    /// <inheritdoc />
    public partial class FixComputedTotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashGiven",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ShiftNumber",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "VoidedAt",
                table: "Receipts");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Receipts",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "Receipts",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Receipts",
                newName: "Created");

            migrationBuilder.AlterColumn<string>(
                name: "VoidReason",
                table: "Receipts",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "FiscalNumber",
                table: "Receipts",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "EGAISCheckNumber",
                table: "Receipts",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CashierName",
                table: "Receipts",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "Receipts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrinted",
                table: "Receipts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "VoidedQuantity",
                table: "ReceiptItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EGAISVolume",
                table: "Products",
                type: "TEXT",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "EGAISCode",
                table: "Products",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "IsPrinted",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "VoidedQuantity",
                table: "ReceiptItems");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Receipts",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Receipts",
                newName: "PaymentType");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Receipts",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "VoidReason",
                table: "Receipts",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FiscalNumber",
                table: "Receipts",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EGAISCheckNumber",
                table: "Receipts",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CashierName",
                table: "Receipts",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CashGiven",
                table: "Receipts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShiftNumber",
                table: "Receipts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VoidedAt",
                table: "Receipts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EGAISVolume",
                table: "Products",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EGAISCode",
                table: "Products",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
