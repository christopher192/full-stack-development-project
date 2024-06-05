using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WD_ERECORD_CORE.Data.Migrations
{
    public partial class asdasdasdasdasdasdasd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailGroupUserLists_EmailGroupLists_EmailGroupListId",
                table: "EmailGroupUserLists");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailGroupUserLists_EmailGroupLists_EmailGroupListId",
                table: "EmailGroupUserLists",
                column: "EmailGroupListId",
                principalTable: "EmailGroupLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailGroupUserLists_EmailGroupLists_EmailGroupListId",
                table: "EmailGroupUserLists");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailGroupUserLists_EmailGroupLists_EmailGroupListId",
                table: "EmailGroupUserLists",
                column: "EmailGroupListId",
                principalTable: "EmailGroupLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
