using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchBoard.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    BoardId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lists_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Overview = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    TagLine = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ReleaseDate = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    EndDate = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    NumberOfSeasons = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    BackdropUrl = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    BackdropBase64 = table.Column<string>(type: "TEXT", maxLength: 16384, nullable: true),
                    PosterUrl = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    PosterBase64 = table.Column<string>(type: "TEXT", maxLength: 16384, nullable: true),
                    ProviderNamesCsv = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    SelectedProviderName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ImdbId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    TmdbId = table.Column<int>(type: "INTEGER", maxLength: 255, nullable: false),
                    Images = table.Column<string>(type: "TEXT", nullable: true),
                    ListId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Lists_ListId",
                        column: x => x.ListId,
                        principalTable: "Lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ListId",
                table: "Items",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_Lists_BoardId",
                table: "Lists",
                column: "BoardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Lists");

            migrationBuilder.DropTable(
                name: "Boards");
        }
    }
}
