using Microsoft.EntityFrameworkCore.Migrations;

namespace BotZeitNot.DAL.Migrations
{
    public partial class fixSeasons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Name",
                table: "Seasons",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Seasons");
        }
    }
}
