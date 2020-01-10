using Microsoft.EntityFrameworkCore.Migrations;

namespace BotZeitNot.DAL.Migrations
{
    public partial class fixSeasons1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsCompleted",
                table: "Series",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsCompleted",
                table: "Series",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
