using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanVang.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductIdNullTableStone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stones_Products_ProductId",
                table: "Stones");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Stones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Stones_Products_ProductId",
                table: "Stones",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stones_Products_ProductId",
                table: "Stones");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Stones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stones_Products_ProductId",
                table: "Stones",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
