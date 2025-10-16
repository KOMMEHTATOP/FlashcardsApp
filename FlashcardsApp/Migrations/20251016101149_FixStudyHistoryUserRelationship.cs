using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class FixStudyHistoryUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyHistory_AspNetUsers_UserId1",
                table: "StudyHistory");

            migrationBuilder.DropIndex(
                name: "IX_StudyHistory_UserId1",
                table: "StudyHistory");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "StudyHistory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "StudyHistory",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyHistory_UserId1",
                table: "StudyHistory",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyHistory_AspNetUsers_UserId1",
                table: "StudyHistory",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
