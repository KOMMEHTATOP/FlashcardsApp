using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixLeaderboardIndexDescending : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Удаляем старый индекс
            migrationBuilder.DropIndex(
                name: "IX_Users_Leaderboard",
                table: "AspNetUsers");

            // Создаем правильный индекс через Raw SQL
            migrationBuilder.Sql(
                @"CREATE INDEX ""IX_Users_Leaderboard"" 
          ON ""AspNetUsers"" (""TotalRating"" DESC, ""RatingLastUpdatedAt"" DESC)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Leaderboard",
                table: "AspNetUsers");
        
            // Восстанавливаем старый (неправильный) индекс
            migrationBuilder.CreateIndex(
                name: "IX_Users_Leaderboard",
                table: "AspNetUsers",
                columns: new[] { "TotalRating", "RatingLastUpdatedAt" });
        }
    }
}
