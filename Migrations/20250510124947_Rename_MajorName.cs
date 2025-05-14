using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darris_Api.Migrations
{
    /// <inheritdoc />
    public partial class Rename_MajorName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_UserInfo_Id",
                table: "StudentCourses");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StudentCourses",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Majors",
                newName: "MajorName");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_UserInfo_UserId",
                table: "StudentCourses",
                column: "UserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_UserInfo_UserId",
                table: "StudentCourses");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "StudentCourses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "MajorName",
                table: "Majors",
                newName: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_UserInfo_Id",
                table: "StudentCourses",
                column: "Id",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
