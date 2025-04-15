using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchBoard.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeriesStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeriesNextEpisodeDate",
                table: "Items",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesNextEpisodeNumber",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesNextEpisodeSeason",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesStatus",
                table: "Items",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeriesNextEpisodeDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SeriesNextEpisodeNumber",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SeriesNextEpisodeSeason",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SeriesStatus",
                table: "Items");
        }
    }
}
