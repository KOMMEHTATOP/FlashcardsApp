using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_CardRating_Rating",
                table: "CardRatings",
                sql: "\"Rating\" >= 1 AND \"Rating\" <= 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CardRating_Rating",
                table: "CardRatings");
        }
    }
}
