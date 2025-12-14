using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReverseWOM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReverse",
                table: "MasteryChallengeCondition",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllSpinReversedCompleted",
                table: "MasteryAttempts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DoubleHoleReversedCompleted",
                table: "MasteryAttempts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExpertReversedCompleted",
                table: "MasteryAttempts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GravityReversedCompleted",
                table: "MasteryAttempts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InvisibleReversedCompleted",
                table: "MasteryAttempts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MessyReversedCompleted",
                table: "MasteryAttempts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoHoldReversedCompleted",
                table: "MasteryAttempts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VolatileReversedCompleted",
                table: "MasteryAttempts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReverse",
                table: "MasteryChallengeCondition");

            migrationBuilder.DropColumn(
                name: "AllSpinReversedCompleted",
                table: "MasteryAttempts");

            migrationBuilder.DropColumn(
                name: "DoubleHoleReversedCompleted",
                table: "MasteryAttempts");

            migrationBuilder.DropColumn(
                name: "ExpertReversedCompleted",
                table: "MasteryAttempts");

            migrationBuilder.DropColumn(
                name: "GravityReversedCompleted",
                table: "MasteryAttempts");

            migrationBuilder.DropColumn(
                name: "InvisibleReversedCompleted",
                table: "MasteryAttempts");

            migrationBuilder.DropColumn(
                name: "MessyReversedCompleted",
                table: "MasteryAttempts");

            migrationBuilder.DropColumn(
                name: "NoHoldReversedCompleted",
                table: "MasteryAttempts");

            migrationBuilder.DropColumn(
                name: "VolatileReversedCompleted",
                table: "MasteryAttempts");
        }
    }
}
