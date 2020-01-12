using Microsoft.EntityFrameworkCore.Migrations;

namespace BotZeitNot.DAL.Migrations
{
    public partial class fix_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameEu",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "TitleEu",
                table: "Episodes");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Series",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "Episodes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "Episodes");

            migrationBuilder.AddColumn<string>(
                name: "NameEu",
                table: "Series",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEu",
                table: "Episodes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
