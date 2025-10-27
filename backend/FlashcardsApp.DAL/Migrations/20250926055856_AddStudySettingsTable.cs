using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStudySettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudySettings",
                columns: table => new
                {
                    StudySettingsId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    StudyOrder = table.Column<int>(type: "integer", nullable: false),
                    PresetName = table.Column<string>(type: "text", nullable: true),
                    MinRating = table.Column<int>(type: "integer", nullable: false),
                    MaxRating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySettings", x => x.StudySettingsId);
                    table.ForeignKey(
                        name: "FK_StudySettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudySettings_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudySettings_GroupId",
                table: "StudySettings",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySettings_UserId",
                table: "StudySettings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudySettings");
        }
    }
}
