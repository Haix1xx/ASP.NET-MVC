using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    public partial class EditColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeAddress",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "BirthDay",
                table: "Users",
                newName: "BirthDate");

            migrationBuilder.AddColumn<string>(
                name: "HomeAdress",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeAdress",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "Users",
                newName: "BirthDay");

            migrationBuilder.AddColumn<string>(
                name: "HomeAddress",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
