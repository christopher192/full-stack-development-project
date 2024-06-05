using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WD_ERECORD_CORE.Data.Migrations
{
    public partial class AddFormSubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormSubmissionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserId = table.Column<string>(type: "VARCHAR2(450)", nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR2(256)", nullable: false),
                    DispalyName = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    FormSubmission = table.Column<string>(type: "CLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubmissionHistories", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormSubmissionHistories");
        }
    }
}
