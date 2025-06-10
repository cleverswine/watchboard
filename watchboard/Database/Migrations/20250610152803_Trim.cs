using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchBoard.Database.Migrations
{
    /// <inheritdoc />
    public partial class Trim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackdropBase64",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BackdropUrl",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SeasonsJson",
                table: "Items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeasonsJson",
                table: "Items",
                type: "TEXT",
                nullable: true);
        }
    }
}
