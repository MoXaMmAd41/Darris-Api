using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darris_Api.Migrations
{
    /// <inheritdoc />
    public partial class Darris : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "UserInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpiration",
                table: "UserInfo",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserInfo",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordResetToken", "PasswordResetTokenExpiration" },
                values: new object[] { "", new DateTime(2025, 4, 24, 13, 9, 18, 691, DateTimeKind.Utc).AddTicks(3359) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpiration",
                table: "UserInfo");
        }
    }
}
