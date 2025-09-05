using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class MoreRunData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRankPoints",
                table: "Runs",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<uint>(
                name: "GarbageAttack",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "GarbageCleared",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "GarbageMaxSpike",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "GarbageMaxSpikeNoMult",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "GarbageReceived",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "GarbageSendNoMult",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "GarbageSent",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "Holds",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "Inputs",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "LinesCleared",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<double>(
                name: "PeakRank",
                table: "Runs",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Rank",
                table: "Runs",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TargetingFactor",
                table: "Runs",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TargetingGrace",
                table: "Runs",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<byte>(
                name: "TopCombo",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<double>(
                name: "TotalBonus",
                table: "Runs",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CommunityChallenges",
                type: "TEXT",
                maxLength: 4096,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CommunityChallenges",
                type: "TEXT",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRankPoints",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "GarbageAttack",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "GarbageCleared",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "GarbageMaxSpike",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "GarbageMaxSpikeNoMult",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "GarbageReceived",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "GarbageSendNoMult",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "GarbageSent",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "Holds",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "Inputs",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "LinesCleared",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "PeakRank",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "TargetingFactor",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "TargetingGrace",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "TopCombo",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "TotalBonus",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CommunityChallenges");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CommunityChallenges");
        }
    }
}
