using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAnswerUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Удаляем старый constraint если он есть
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 
                        FROM pg_constraint 
                        WHERE conname = 'IX_Cards_User_Answer'
                    ) THEN
                        ALTER TABLE ""Cards"" DROP CONSTRAINT ""IX_Cards_User_Answer"";
                    END IF;
                END $$;
            ");
        
            // Удаляем индекс если он есть
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ""IX_Cards_User_Answer"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Откат — ничего не делаем
        }
    }
}
