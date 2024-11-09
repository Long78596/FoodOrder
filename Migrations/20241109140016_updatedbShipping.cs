using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrder.Migrations
{
    /// <inheritdoc />
    public partial class updatedbShipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Foods_FoodId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_FoodId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FoodId",
                table: "Orders");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Shippings",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Shippings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "FoodId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FoodId",
                table: "Orders",
                column: "FoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Foods_FoodId",
                table: "Orders",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id");
        }
    }
}
