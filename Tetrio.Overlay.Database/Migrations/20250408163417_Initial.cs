using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Mods = table.Column<string>(type: "TEXT", nullable: false),
                    Points = table.Column<byte>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConditionRanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConditionType = table.Column<int>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    Min = table.Column<double>(type: "REAL", nullable: false),
                    Max = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    MinDifficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<byte>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TetrioId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    SessionToken = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastSubmission = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DiscordId = table.Column<string>(type: "TEXT", nullable: false),
                    AccessToken = table.Column<string>(type: "TEXT", nullable: true),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeConditions",
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
                    table.PrimaryKey("PK_ChallengeConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChallengeConditions_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeUser",
                columns: table => new
                {
                    ChallengesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsersId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeUser", x => new { x.ChallengesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ChallengeUser_Challenges_ChallengesId",
                        column: x => x.ChallengesId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChallengeUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Runs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TetrioId = table.Column<string>(type: "TEXT", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Altitude = table.Column<double>(type: "REAL", nullable: false),
                    KOs = table.Column<byte>(type: "INTEGER", nullable: false),
                    AllClears = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Quads = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Spins = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Mods = table.Column<string>(type: "TEXT", nullable: false),
                    SpeedrunSeen = table.Column<bool>(type: "INTEGER", nullable: false),
                    SpeedrunCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Apm = table.Column<double>(type: "REAL", nullable: false),
                    Pps = table.Column<double>(type: "REAL", nullable: false),
                    Vs = table.Column<double>(type: "REAL", nullable: false),
                    Finesse = table.Column<double>(type: "REAL", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Runs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZenithSplits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TetrioId = table.Column<string>(type: "TEXT", nullable: false),
                    HotelReachedAt = table.Column<uint>(type: "INTEGER", nullable: false),
                    CasinoReachedAt = table.Column<uint>(type: "INTEGER", nullable: false),
                    ArenaReachedAt = table.Column<uint>(type: "INTEGER", nullable: false),
                    MuseumReachedAt = table.Column<uint>(type: "INTEGER", nullable: false),
                    OfficesReachedAt = table.Column<uint>(type: "INTEGER", nullable: false),
                    LaboratoryReachedAt = table.Column<uint>(type: "INTEGER", nullable: false),
                    CoreReachedAt = table.Column<uint>(type: "INTEGER", nullable: false),
                    CorruptionReachedAt = table.Column<uint>(type: "INTEGER", nullable: false),
                    PlatformOfTheGodsReachedAt = table.Column<uint>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZenithSplits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZenithSplits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeRun",
                columns: table => new
                {
                    ChallengesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RunsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeRun", x => new { x.ChallengesId, x.RunsId });
                    table.ForeignKey(
                        name: "FK_ChallengeRun_Challenges_ChallengesId",
                        column: x => x.ChallengesId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChallengeRun_Runs_RunsId",
                        column: x => x.RunsId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ConditionRanges",
                columns: new[] { "Id", "ConditionType", "CreatedAt", "Difficulty", "Max", "Min", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), 0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 350.0, 50.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111102"), 1, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1.0, 0.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111103"), 4, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0.0, 0.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111104"), 2, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10.0, 3.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111105"), 3, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0.0, 0.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111106"), 5, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 20.0, 10.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111107"), 6, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1.0, 0.75, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111108"), 7, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 40.0, 30.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111109"), 8, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 50.0, 35.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111201"), 0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 650.0, 350.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111202"), 1, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2.0, 1.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111203"), 4, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1.0, 0.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111204"), 2, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 15.0, 5.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111205"), 3, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 30.0, 5.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111206"), 5, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 55.0, 20.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111207"), 6, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1.75, 1.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111208"), 7, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 100.0, 40.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111209"), 8, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 65.0, 50.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111301"), 0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1350.0, 650.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111302"), 1, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 5.0, 2.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111303"), 4, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3.0, 1.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111304"), 2, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 20.0, 10.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111305"), 3, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 75.0, 30.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111306"), 5, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 85.0, 55.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111307"), 6, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 2.0, 1.75, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111308"), 7, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 200.0, 100.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111309"), 8, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 80.0, 65.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111401"), 0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 2500.0, 1350.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111402"), 1, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 5.0, 3.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111403"), 4, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 6.0, 3.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111404"), 2, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 30.0, 20.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111405"), 3, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 125.0, 75.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111406"), 5, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 130.0, 85.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111407"), 6, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 2.25, 1.75, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111408"), 7, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 200.0, 100.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111409"), 8, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 100.0, 75.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Mods",
                columns: new[] { "Id", "CreatedAt", "MinDifficulty", "Name", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "expert", new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)50 },
                    { new Guid("11111111-1111-1111-1111-111111111102"), new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "nohold", new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)25 },
                    { new Guid("11111111-1111-1111-1111-111111111103"), new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "messy", new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)25 },
                    { new Guid("11111111-1111-1111-1111-111111111104"), new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "gravity", new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)25 },
                    { new Guid("11111111-1111-1111-1111-111111111105"), new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "volatile", new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)25 },
                    { new Guid("11111111-1111-1111-1111-111111111106"), new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "doublehole", new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)25 },
                    { new Guid("11111111-1111-1111-1111-111111111107"), new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "invisible", new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)60 },
                    { new Guid("11111111-1111-1111-1111-111111111108"), new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "allspin", new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)40 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeConditions_ChallengeId",
                table: "ChallengeConditions",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRun_RunsId",
                table: "ChallengeRun",
                column: "RunsId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_Date_Points",
                table: "Challenges",
                columns: new[] { "Date", "Points" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeUser_UsersId",
                table: "ChallengeUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionRanges_ConditionType_Difficulty",
                table: "ConditionRanges",
                columns: new[] { "ConditionType", "Difficulty" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Runs_TetrioId",
                table: "Runs",
                column: "TetrioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Runs_UserId",
                table: "Runs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DiscordId",
                table: "Users",
                column: "DiscordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SessionToken",
                table: "Users",
                column: "SessionToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TetrioId",
                table: "Users",
                column: "TetrioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZenithSplits_TetrioId",
                table: "ZenithSplits",
                column: "TetrioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZenithSplits_UserId",
                table: "ZenithSplits",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChallengeConditions");

            migrationBuilder.DropTable(
                name: "ChallengeRun");

            migrationBuilder.DropTable(
                name: "ChallengeUser");

            migrationBuilder.DropTable(
                name: "ConditionRanges");

            migrationBuilder.DropTable(
                name: "Mods");

            migrationBuilder.DropTable(
                name: "ZenithSplits");

            migrationBuilder.DropTable(
                name: "Runs");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
