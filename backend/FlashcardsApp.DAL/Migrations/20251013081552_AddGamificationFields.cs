using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddGamificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "UserStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StudyHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    XPEarned = table.Column<int>(type: "integer", nullable: false),
                    StudiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyHistory", x => x.Id);
                    table.CheckConstraint("CK_StudyHistory_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");
                    table.ForeignKey(
                        name: "FK_StudyHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyHistory_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyHistory_CardId",
                table: "StudyHistory",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyHistory_UserId_CardId_StudiedAt",
                table: "StudyHistory",
                columns: new[] { "UserId", "CardId", "StudiedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyHistory");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "UserStatistics");
        }
    }
}
