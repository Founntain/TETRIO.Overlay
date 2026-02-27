using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class RunChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ushort>(
                name: "FinalPlayerCount",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "FinalPosition",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "Floor",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "GameOverReason",
                table: "Runs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "PeakPlayerCount",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "PeakPosition",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<uint>(
                name: "PiecesPlaces",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "Score",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalPlayerCount",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "FinalPosition",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "Floor",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "GameOverReason",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "PeakPlayerCount",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "PeakPosition",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "PiecesPlaces",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Runs");
        }
    }
}
