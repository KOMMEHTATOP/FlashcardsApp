using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompletionThreshold",
                table: "StudySettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShuffleOnRepeat",
                table: "StudySettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionThreshold",
                table: "StudySettings");

            migrationBuilder.DropColumn(
                name: "ShuffleOnRepeat",
                table: "StudySettings");
        }
    }
}
