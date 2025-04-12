using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class CommunityChallengeContributions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRestricted",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TotalTime",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "ConditionRanges",
                columns: new[] { "Id", "ConditionType", "CreatedAt", "Difficulty", "Max", "Min", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111001"), 0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 5000000.0, 1000000.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111002"), 1, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 15000.0, 10000.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111003"), 4, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 100000.0, 50000.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111004"), 2, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2000000.0, 1000000.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111005"), 3, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 4000000.0, 1000000.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111006"), 5, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1000000.0, 500000.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111007"), 6, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 10000.0, 5000.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111008"), 7, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1500000.0, 750000.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111009"), 8, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0.0, 0.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"),
                column: "Weight",
                value: (byte)30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111001"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111002"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111003"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111004"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111005"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111006"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111007"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111008"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111009"));

            migrationBuilder.DropColumn(
                name: "IsRestricted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalTime",
                table: "Runs");

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"),
                column: "Weight",
                value: (byte)25);
        }
    }
}
