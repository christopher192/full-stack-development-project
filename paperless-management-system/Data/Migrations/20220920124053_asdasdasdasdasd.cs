using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WD_ERECORD_CORE.Data.Migrations
{
    public partial class asdasdasdasdasd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "APISettings",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "SID",
                table: "APISettings",
                type: "VARCHAR2(150)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SID",
                table: "APISettings");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "APISettings",
                newName: "PasswordHash");
        }
    }
}
