using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchBoard.Database.Migrations
{
    /// <inheritdoc />
    public partial class Runtime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RunTime",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RunTime",
                table: "Items");
        }
    }
}
