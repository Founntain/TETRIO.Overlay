using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommunityConditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111004"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 100000.0, 45000.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111005"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 1000000.0, 250000.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111006"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 7500000.0, 100000.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111007"),
                column: "Max",
                value: 15000.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"),
                column: "Min",
                value: 100.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111208"),
                column: "Max",
                value: 95.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111304"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 30.0, 15.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111308"),
                column: "Min",
                value: 80.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111401"),
                column: "Min",
                value: 650.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111405"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 100.0, 50.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111004"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 2000000.0, 1000000.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111005"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 4000000.0, 1000000.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111006"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 1000000.0, 500000.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111007"),
                column: "Max",
                value: 10000.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"),
                column: "Min",
                value: 50.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111208"),
                column: "Max",
                value: 100.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111304"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 20.0, 10.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111308"),
                column: "Min",
                value: 100.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111401"),
                column: "Min",
                value: 850.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111405"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 125.0, 75.0 });
        }
    }
}
