using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WD_ERECORD_CORE.Data.Migrations
{
    public partial class asdadasdasdas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APISettingLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FormId = table.Column<int>(type: "NUMBER(10,0)", nullable: false),
                    APISettingIds = table.Column<string>(type: "VARCHAR2(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APISettingLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APISettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Title = table.Column<string>(type: "VARCHAR2(200)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    Url = table.Column<string>(type: "VARCHAR2(500)", nullable: false),
                    Condition = table.Column<string>(type: "VARCHAR2(500)", nullable: true),
                    Port = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    ServiceName = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    APIType = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    PasswordHash = table.Column<string>(type: "VARCHAR2(300)", nullable: true),
                    DatabaseName = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    DatabaseScript = table.Column<string>(type: "VARCHAR2(1000)", nullable: true),
                    Param = table.Column<string>(type: "VARCHAR2(500)", nullable: true),
                    KeyName = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    Format = table.Column<string>(type: "VARCHAR2(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APISettings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APISettingLogs");

            migrationBuilder.DropTable(
                name: "APISettings");
        }
    }
}
