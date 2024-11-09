using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrder.Migrations
{
    /// <inheritdoc />
    public partial class editdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_AspNetUsers_AppUserId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_AppUserId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "OrderDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "OrderDetails",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_AppUserId",
                table: "OrderDetails",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_AspNetUsers_AppUserId",
                table: "OrderDetails",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
