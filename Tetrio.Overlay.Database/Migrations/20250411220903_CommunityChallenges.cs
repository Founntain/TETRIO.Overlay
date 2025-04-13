using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class CommunityChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunityChallenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConditionType = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetValue = table.Column<double>(type: "REAL", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Finished = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityContributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<double>(type: "REAL", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommunityChallengeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityContributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityContributions_CommunityChallenges_CommunityChallengeId",
                        column: x => x.CommunityChallengeId,
                        principalTable: "CommunityChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityContributions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111207"),
                column: "Max",
                value: 1.6499999999999999);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111307"),
                column: "Min",
                value: 1.6499999999999999);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityChallenges_StartDate",
                table: "CommunityChallenges",
                column: "StartDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContributions_CommunityChallengeId",
                table: "CommunityContributions",
                column: "CommunityChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContributions_UserId",
                table: "CommunityContributions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityContributions");

            migrationBuilder.DropTable(
                name: "CommunityChallenges");

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111207"),
                column: "Max",
                value: 1.75);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111307"),
                column: "Min",
                value: 1.75);
        }
    }
}
