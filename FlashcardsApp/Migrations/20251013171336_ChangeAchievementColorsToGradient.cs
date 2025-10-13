using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAchievementColorsToGradient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorEnd",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "ColorStart",
                table: "Achievements",
                newName: "Gradient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gradient",
                table: "Achievements",
                newName: "ColorStart");

            migrationBuilder.AddColumn<string>(
                name: "ColorEnd",
                table: "Achievements",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
