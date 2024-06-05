using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WD_ERECORD_CORE.Data.Migrations
{
    public partial class removeformsubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormSubmissionHistories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormSubmissionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DispalyName = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    FormSubmission = table.Column<string>(type: "CLOB", nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR2(450)", nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR2(256)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubmissionHistories", x => x.Id);
                });
        }
    }
}
