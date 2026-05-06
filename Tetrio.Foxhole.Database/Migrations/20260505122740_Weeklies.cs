using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class Weeklies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeeklyChallenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Week = table.Column<byte>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Mods = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyChallengeConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WeeklyChallengeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyChallengeConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyChallengeConditions_WeeklyChallenges_WeeklyChallengeId",
                        column: x => x.WeeklyChallengeId,
                        principalTable: "WeeklyChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WeeklyChallengeId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyProgresses_WeeklyChallenges_WeeklyChallengeId",
                        column: x => x.WeeklyChallengeId,
                        principalTable: "WeeklyChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyConditionProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentProgress = table.Column<double>(type: "REAL", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    WeeklyProgressId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConditionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WeeklyChallengeConditionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyConditionProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyConditionProgresses_WeeklyChallengeConditions_WeeklyChallengeConditionId",
                        column: x => x.WeeklyChallengeConditionId,
                        principalTable: "WeeklyChallengeConditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyConditionProgresses_WeeklyProgresses_WeeklyProgressId",
                        column: x => x.WeeklyProgressId,
                        principalTable: "WeeklyProgresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyChallengeConditions_WeeklyChallengeId",
                table: "WeeklyChallengeConditions",
                column: "WeeklyChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyChallenges_StartDate",
                table: "WeeklyChallenges",
                column: "StartDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyConditionProgresses_WeeklyChallengeConditionId",
                table: "WeeklyConditionProgresses",
                column: "WeeklyChallengeConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyConditionProgresses_WeeklyProgressId_ConditionId",
                table: "WeeklyConditionProgresses",
                columns: new[] { "WeeklyProgressId", "ConditionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyProgresses_UserId_WeeklyChallengeId",
                table: "WeeklyProgresses",
                columns: new[] { "UserId", "WeeklyChallengeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyProgresses_WeeklyChallengeId",
                table: "WeeklyProgresses",
                column: "WeeklyChallengeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeeklyConditionProgresses");

            migrationBuilder.DropTable(
                name: "WeeklyChallengeConditions");

            migrationBuilder.DropTable(
                name: "WeeklyProgresses");

            migrationBuilder.DropTable(
                name: "WeeklyChallenges");
        }
    }
}
