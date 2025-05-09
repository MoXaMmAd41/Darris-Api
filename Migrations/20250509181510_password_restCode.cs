using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darris_Api.Migrations
{
    /// <inheritdoc />
    public partial class password_restCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetCode",
                table: "UserInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetCodeExpiration",
                table: "UserInfo",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserInfo",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordResetCode", "PasswordResetCodeExpiration" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetCode",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "PasswordResetCodeExpiration",
                table: "UserInfo");
        }
    }
}
