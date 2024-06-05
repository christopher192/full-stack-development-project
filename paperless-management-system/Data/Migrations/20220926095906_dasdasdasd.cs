using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WD_ERECORD_CORE.Data.Migrations
{
    public partial class dasdasdasd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DepartmentListId",
                table: "SubDepartmentLists",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DepartmentListId",
                table: "SubDepartmentLists",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);
        }
    }
}
