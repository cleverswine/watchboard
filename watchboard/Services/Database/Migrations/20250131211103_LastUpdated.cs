using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchBoard.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class LastUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "Items",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Items");
        }
    }
}
