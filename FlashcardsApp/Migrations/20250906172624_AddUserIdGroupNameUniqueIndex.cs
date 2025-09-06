using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdGroupNameUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Groups_UserId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Cards_UserId",
                table: "Cards");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_UserId_GroupName",
                table: "Groups",
                columns: new[] { "UserId", "GroupName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserId_Question",
                table: "Cards",
                columns: new[] { "UserId", "Question" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Groups_UserId_GroupName",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Cards_UserId_Question",
                table: "Cards");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_UserId",
                table: "Groups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserId",
                table: "Cards",
                column: "UserId");
        }
    }
}
