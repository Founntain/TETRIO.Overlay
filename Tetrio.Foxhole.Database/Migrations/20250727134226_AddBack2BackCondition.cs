using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBack2BackCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ushort>(
                name: "Back2Back",
                table: "Runs",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.InsertData(
                table: "ConditionRanges",
                columns: new[] { "Id", "ConditionType", "CreatedAt", "Difficulty", "Max", "Min", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111010"), 9, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1000000.0, 250000.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111110"), 9, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 7.0, 3.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111210"), 9, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 25.0, 7.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111310"), 9, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 50.0, 25.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111410"), 9, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 80.0, 20.0, new DateTime(2020, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111010"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111110"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111210"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111310"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111410"));

            migrationBuilder.DropColumn(
                name: "Back2Back",
                table: "Runs");
        }
    }
}
