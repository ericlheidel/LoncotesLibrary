using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoncotesLibrary.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReturnDate",
                value: new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReturnDate",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReturnDate",
                value: new DateTime(2024, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReturnDate",
                value: new DateTime(2024, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
