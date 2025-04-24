using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class LateContributions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLate",
                table: "CommunityContributions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLate",
                table: "CommunityContributions");
        }
    }
}
