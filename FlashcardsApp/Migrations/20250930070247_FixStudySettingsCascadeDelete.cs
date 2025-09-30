using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class FixStudySettingsCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySettings_Groups_GroupId",
                table: "StudySettings");

            migrationBuilder.DropIndex(
                name: "IX_StudySettings_GroupId",
                table: "StudySettings");

            migrationBuilder.CreateIndex(
                name: "IX_StudySettings_GroupId",
                table: "StudySettings",
                column: "GroupId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySettings_Groups_GroupId",
                table: "StudySettings",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySettings_Groups_GroupId",
                table: "StudySettings");

            migrationBuilder.DropIndex(
                name: "IX_StudySettings_GroupId",
                table: "StudySettings");

            migrationBuilder.CreateIndex(
                name: "IX_StudySettings_GroupId",
                table: "StudySettings",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySettings_Groups_GroupId",
                table: "StudySettings",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
