using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchBoard.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProviderStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderNamesCsv",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SelectedProviderName",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "ProvidersJson",
                table: "Items",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvidersJson",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "ProviderNamesCsv",
                table: "Items",
                type: "TEXT",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedProviderName",
                table: "Items",
                type: "TEXT",
                maxLength: 255,
                nullable: true);
        }
    }
}
