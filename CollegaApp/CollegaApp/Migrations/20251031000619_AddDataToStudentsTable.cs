using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CollegaApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDataToStudentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "StudentsDb",
                columns: new[] { "Id", "Address", "DOB", "Email", "Name" },
                values: new object[,]
                {
                    { 1, "Porsgrunn", new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "ada@uni.edu", "Ada" },
                    { 2, "Skien", new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "linus@uni.edu", "Linus" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StudentsDb",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StudentsDb",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
