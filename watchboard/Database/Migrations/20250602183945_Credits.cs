using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchBoard.Database.Migrations
{
    /// <inheritdoc />
    public partial class Credits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreditsJson",
                table: "Items",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditsJson",
                table: "Items");
        }
    }
}
