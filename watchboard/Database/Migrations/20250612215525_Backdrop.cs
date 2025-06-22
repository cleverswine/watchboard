using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchBoard.Database.Migrations
{
    /// <inheritdoc />
    public partial class Backdrop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackdropBase64",
                table: "Items",
                type: "TEXT",
                maxLength: 16384,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackdropUrl",
                table: "Items",
                type: "TEXT",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackdropBase64",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BackdropUrl",
                table: "Items");
        }
    }
}
