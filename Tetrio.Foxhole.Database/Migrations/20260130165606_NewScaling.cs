using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class NewScaling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "App",
                table: "Runs",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111107"),
                column: "Min",
                value: 0.59999999999999998);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111108"),
                column: "Min",
                value: 20.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111206"),
                column: "Max",
                value: 50.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111207"),
                column: "Max",
                value: 1.6000000000000001);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111304"),
                column: "Max",
                value: 40.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111306"),
                column: "Min",
                value: 50.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111307"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 2.2000000000000002, 1.6000000000000001 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111308"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 190.0, 90.0 });

            migrationBuilder.InsertData(
                table: "ConditionRanges",
                columns: new[] { "Id", "ConditionType", "Difficulty", "Max", "Min" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 11, 2, 0.40000000000000002, 0.20000000000000001 },
                    { new Guid("11111111-1111-1111-1111-111111111112"), 12, 2, 150.0, 40.0 },
                    { new Guid("11111111-1111-1111-1111-111111111211"), 11, 3, 0.59999999999999998, 0.40000000000000002 },
                    { new Guid("11111111-1111-1111-1111-111111111212"), 12, 3, 0.0, 0.0 },
                    { new Guid("11111111-1111-1111-1111-111111111311"), 11, 5, 0.80000000000000004, 0.59999999999999998 },
                    { new Guid("11111111-1111-1111-1111-111111111312"), 12, 5, 0.0, 0.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111211"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111212"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111311"));

            migrationBuilder.DeleteData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111312"));

            migrationBuilder.DropColumn(
                name: "App",
                table: "Runs");

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111107"),
                column: "Min",
                value: 0.75);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111108"),
                column: "Min",
                value: 30.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111206"),
                column: "Max",
                value: 55.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111207"),
                column: "Max",
                value: 1.6499999999999999);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111304"),
                column: "Max",
                value: 30.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111306"),
                column: "Min",
                value: 55.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111307"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 2.1000000000000001, 1.6499999999999999 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111308"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 175.0, 80.0 });
        }
    }
}
