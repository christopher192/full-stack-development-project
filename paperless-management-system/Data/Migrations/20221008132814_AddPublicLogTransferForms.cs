using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WD_ERECORD_CORE.Data.Migrations
{
    public partial class AddPublicLogTransferForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicLogTransferForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DateTime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    LogDetail = table.Column<string>(type: "VARCHAR2(1000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicLogTransferForms", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicLogTransferForms");
        }
    }
}
