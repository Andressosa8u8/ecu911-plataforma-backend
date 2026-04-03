using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.AuthService.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleTypeToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleType",
                table: "roles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "roles");
        }
    }
}
