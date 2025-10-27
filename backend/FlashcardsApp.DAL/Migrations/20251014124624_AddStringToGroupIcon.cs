using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStringToGroupIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupIcon",
                table: "Groups",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupIcon",
                table: "Groups");
        }
    }
}
