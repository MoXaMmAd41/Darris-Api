using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darris_Api.Migrations
{
    /// <inheritdoc />
    public partial class Emailverify_code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailVerificationToken",
                table: "UserInfo",
                newName: "EmailVerificationCode");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationCodeExpiration",
                table: "UserInfo",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserInfo",
                keyColumn: "Id",
                keyValue: 1,
                column: "EmailVerificationCodeExpiration",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationCodeExpiration",
                table: "UserInfo");

            migrationBuilder.RenameColumn(
                name: "EmailVerificationCode",
                table: "UserInfo",
                newName: "EmailVerificationToken");
        }
    }
}
