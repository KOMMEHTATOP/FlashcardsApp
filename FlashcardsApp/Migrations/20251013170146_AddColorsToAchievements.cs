using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddColorsToAchievements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PerfectRatingsStreak",
                table: "UserStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCardsCreated",
                table: "UserStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCardsStudied",
                table: "UserStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ColorEnd",
                table: "Achievements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ColorStart",
                table: "Achievements",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerfectRatingsStreak",
                table: "UserStatistics");

            migrationBuilder.DropColumn(
                name: "TotalCardsCreated",
                table: "UserStatistics");

            migrationBuilder.DropColumn(
                name: "TotalCardsStudied",
                table: "UserStatistics");

            migrationBuilder.DropColumn(
                name: "ColorEnd",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "ColorStart",
                table: "Achievements");
        }
    }
}
