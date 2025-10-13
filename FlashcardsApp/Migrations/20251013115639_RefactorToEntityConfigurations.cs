using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToEntityConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_StudyHistory_UserId_CardId_StudiedAt",
                table: "StudyHistory",
                newName: "IX_StudyHistory_User_Card_Date");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_UserId_GroupName",
                table: "Groups",
                newName: "IX_Groups_User_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_UserId_Question",
                table: "Cards",
                newName: "IX_Cards_User_Question");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_StudyHistory_User_Card_Date",
                table: "StudyHistory",
                newName: "IX_StudyHistory_UserId_CardId_StudiedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_User_Name",
                table: "Groups",
                newName: "IX_Groups_UserId_GroupName");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_User_Question",
                table: "Cards",
                newName: "IX_Cards_UserId_Question");
        }
    }
}
