using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebStore.OrderInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UPD0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemPrice",
                table: "OrderItems",
                newName: "UnitPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "OrderItems",
                newName: "ItemPrice");
        }
    }
}
