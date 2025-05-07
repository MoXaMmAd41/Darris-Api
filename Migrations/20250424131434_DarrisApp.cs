using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darris_Api.Migrations
{
    /// <inheritdoc />
    public partial class DarrisApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserInfo",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordResetTokenExpiration",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserInfo",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordResetTokenExpiration",
                value: new DateTime(2025, 4, 24, 13, 9, 18, 691, DateTimeKind.Utc).AddTicks(3359));
        }
    }
}
