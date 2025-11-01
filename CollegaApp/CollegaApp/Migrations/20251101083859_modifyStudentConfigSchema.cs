using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegaApp.Migrations
{
    /// <inheritdoc />
    public partial class modifyStudentConfigSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsDb",
                table: "StudentsDb");

            migrationBuilder.RenameTable(
                name: "StudentsDb",
                newName: "Students");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Email", "Name" },
                values: new object[] { "Porsgrunn2", "ada2@uni.edu", "Ada2" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Email", "Name" },
                values: new object[] { "Skien2", "linus2@uni.edu", "Linus2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "StudentsDb");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsDb",
                table: "StudentsDb",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "StudentsDb",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Email", "Name" },
                values: new object[] { "Porsgrunn", "ada@uni.edu", "Ada" });

            migrationBuilder.UpdateData(
                table: "StudentsDb",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Email", "Name" },
                values: new object[] { "Skien", "linus@uni.edu", "Linus" });
        }
    }
}
