using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCardRatingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardRatings");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CardRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardRatings", x => x.Id);
                    table.CheckConstraint("CK_CardRating_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");
                    table.ForeignKey(
                        name: "FK_CardRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardRatings_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardRatings_CardId",
                table: "CardRatings",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardRatings_UserId",
                table: "CardRatings",
                column: "UserId");
        }
    }
}
