using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanVang.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTypeAndIsPrimaryFromStones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Stones");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "Stones");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Stones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "Stones",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }

}
