using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoncotesLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddOutOfCirculationData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 1,
                column: "OutOfCirculation",
                value: new DateTime(1976, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 2,
                column: "OutOfCirculation",
                value: new DateTime(1984, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 8,
                column: "OutOfCirculation",
                value: new DateTime(1933, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 1,
                column: "OutOfCirculation",
                value: null);

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 2,
                column: "OutOfCirculation",
                value: null);

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 8,
                column: "OutOfCirculation",
                value: null);
        }
    }
}
