using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WebStoreV116 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activity_CreateDate",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Activity_IsActive",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Activity_UpdateDate",
                table: "Carts");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderProcessedDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ItemAddedDate",
                table: "CartItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderProcessedDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ItemAddedDate",
                table: "CartItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "Activity_CreateDate",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Activity_IsActive",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Activity_UpdateDate",
                table: "Carts",
                type: "datetime2",
                nullable: true);
        }
    }
}
