using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class MasteryChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityContributions_Users_UserId",
                table: "CommunityContributions");

            migrationBuilder.CreateTable(
                name: "MasteryChallenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasteryChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasteryAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExpertCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoHoldCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MessyCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    GravityCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    VolatileCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DoubleHoleCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    InvisibleCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllSpinCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MasteryChallengeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasteryAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasteryAttempts_MasteryChallenges_MasteryChallengeId",
                        column: x => x.MasteryChallengeId,
                        principalTable: "MasteryChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasteryAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasteryChallengeCondition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChallengeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasteryChallengeCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasteryChallengeCondition_MasteryChallenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "MasteryChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MasteryAttempts_MasteryChallengeId",
                table: "MasteryAttempts",
                column: "MasteryChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_MasteryAttempts_UserId",
                table: "MasteryAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MasteryChallengeCondition_ChallengeId",
                table: "MasteryChallengeCondition",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_MasteryChallenges_Date",
                table: "MasteryChallenges",
                column: "Date",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityContributions_Users_UserId",
                table: "CommunityContributions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityContributions_Users_UserId",
                table: "CommunityContributions");

            migrationBuilder.DropTable(
                name: "MasteryAttempts");

            migrationBuilder.DropTable(
                name: "MasteryChallengeCondition");

            migrationBuilder.DropTable(
                name: "MasteryChallenges");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityContributions_Users_UserId",
                table: "CommunityContributions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
