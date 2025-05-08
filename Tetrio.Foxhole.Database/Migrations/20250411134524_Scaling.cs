using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tetrio.Overlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class Scaling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Scaling",
                table: "Mods",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

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
                column: "Min",
                value: 150.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111409"),
                column: "Min",
                value: 80.0);

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"),
                columns: new[] { "Scaling", "Weight" },
                values: new object[] { 0.75, (byte)60 });

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111102"),
                column: "Scaling",
                value: 0.90000000000000002);

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111103"),
                column: "Scaling",
                value: 1.0);

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"),
                column: "Scaling",
                value: 1.0);

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111105"),
                column: "Scaling",
                value: 1.0);

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111106"),
                column: "Scaling",
                value: 1.0);

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111107"),
                column: "Scaling",
                value: 1.0);

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111108"),
                column: "Scaling",
                value: 1.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scaling",
                table: "Mods");

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111407"),
                columns: new[] { "Max", "Min" },
                values: new object[] { 2.25, 1.75 });

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111408"),
                column: "Min",
                value: 100.0);

            migrationBuilder.UpdateData(
                table: "ConditionRanges",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111409"),
                column: "Min",
                value: 75.0);

            migrationBuilder.UpdateData(
                table: "Mods",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"),
                column: "Weight",
                value: (byte)50);
        }
    }
}
