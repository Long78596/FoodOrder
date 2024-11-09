using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrder.Migrations
{
    /// <inheritdoc />
    public partial class updatefood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Foods",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sold",
                table: "Foods",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_FoodId",
                table: "Ratings",
                column: "FoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Foods_FoodId",
                table: "Ratings",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Foods_FoodId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_FoodId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "Sold",
                table: "Foods");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Foods",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
