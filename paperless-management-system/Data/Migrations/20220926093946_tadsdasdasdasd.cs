using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WD_ERECORD_CORE.Data.Migrations
{
    public partial class tadsdasdasdasd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubDepartmentLists_DepartmentLists_DepartmentListId",
                table: "SubDepartmentLists");

            migrationBuilder.CreateTable(
                name: "EmailGroupLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    GroupName = table.Column<string>(type: "VARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailGroupLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailGroupUserLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Username = table.Column<string>(type: "VARCHAR2(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "VARCHAR2(50)", maxLength: 50, nullable: false),
                    EmailGroupListId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailGroupUserLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailGroupUserLists_EmailGroupLists_EmailGroupListId",
                        column: x => x.EmailGroupListId,
                        principalTable: "EmailGroupLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailGroupUserLists_EmailGroupListId",
                table: "EmailGroupUserLists",
                column: "EmailGroupListId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubDepartmentLists_DepartmentLists_DepartmentListId",
                table: "SubDepartmentLists",
                column: "DepartmentListId",
                principalTable: "DepartmentLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubDepartmentLists_DepartmentLists_DepartmentListId",
                table: "SubDepartmentLists");

            migrationBuilder.DropTable(
                name: "EmailGroupUserLists");

            migrationBuilder.DropTable(
                name: "EmailGroupLists");

            migrationBuilder.AddForeignKey(
                name: "FK_SubDepartmentLists_DepartmentLists_DepartmentListId",
                table: "SubDepartmentLists",
                column: "DepartmentListId",
                principalTable: "DepartmentLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
