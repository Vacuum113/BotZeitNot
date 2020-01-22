using Microsoft.EntityFrameworkCore.Migrations;

namespace BotZeitNot.DAL.Migrations
{
    public partial class ReworSubSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramUserId",
                table: "SubscriptionSeries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TelegramUserId",
                table: "SubscriptionSeries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
