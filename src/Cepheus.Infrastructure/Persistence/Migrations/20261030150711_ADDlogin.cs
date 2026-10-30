
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    public partial class ADDlogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Login",
                schema: "admin",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            // Asignar login a los usuarios existentes usando el correo.
            migrationBuilder.Sql(@"
                UPDATE admin.Users
                SET Login = LOWER(
                    LEFT(Email, CHARINDEX('@', Email) - 1)
                )
                WHERE Email IS NOT NULL
                  AND CHARINDEX('@', Email) > 1;
            ");

            // Crear índice único después de asignar los logins.
            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                schema: "admin",
                table: "Users",
                column: "Login",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Login",
                schema: "admin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Login",
                schema: "admin",
                table: "Users");
        }
    }
}
