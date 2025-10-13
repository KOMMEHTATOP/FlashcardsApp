using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyStudySettingsToGlobal : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_StudySettings_UserId",
                table: "StudySettings");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "StudySettings");

            migrationBuilder.DropColumn(
                name: "PresetName",
                table: "StudySettings");

            migrationBuilder.CreateIndex(
                name: "IX_StudySettings_UserId",
                table: "StudySettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudySettings_UserId",
                table: "StudySettings");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "StudySettings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresetName",
                table: "StudySettings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudySettings_GroupId",
                table: "StudySettings",
                column: "GroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudySettings_UserId",
                table: "StudySettings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySettings_Groups_GroupId",
                table: "StudySettings",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
