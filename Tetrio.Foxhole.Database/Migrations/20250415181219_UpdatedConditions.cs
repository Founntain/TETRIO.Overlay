using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedConditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111401"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 1750.0, 850.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111402"),
                column: "Min",
                value: 2.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111403"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 1.0, 0.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111406"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 80.0, 50.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111407"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 1.75, 1.3999999999999999 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111408"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 125.0, 80.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111409"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 80.0, 60.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111401"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 2500.0, 1350.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111402"),
                column: "Min",
                value: 3.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111403"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 6.0, 3.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111406"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 130.0, 85.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111407"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 2.5, 2.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111408"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 200.0, 150.0 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111409"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 100.0, 80.0 });
        }
    }
}
