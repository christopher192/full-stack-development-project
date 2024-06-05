using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WD_ERECORD_CORE.Data.Migrations
{
    public partial class AddInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnouncementLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Label1 = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    Label2 = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    Image = table.Column<string>(type: "CLOB", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    EmployeeId = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    EmployeeType = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    Title = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    Department = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    ManagerName = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    ManagerId = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    ReportChainID = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    CostCenterName = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    CostCenterID = table.Column<string>(type: "NVARCHAR2(250)", nullable: true),
                    ProfilePicture = table.Column<string>(type: "CLOB", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserId = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    Title = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostCenterLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Code = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    JobTitle = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    JobClassificationTrack = table.Column<string>(type: "VARCHAR2(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenterLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DepartmentCode = table.Column<string>(type: "VARCHAR2(10)", nullable: false),
                    DepartmentDescription = table.Column<string>(type: "VARCHAR2(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentCategoryLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CategoryCode = table.Column<string>(type: "VARCHAR2(10)", nullable: false),
                    CategoryDescription = table.Column<string>(type: "VARCHAR2(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategoryLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FactoryLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    LocationCode = table.Column<string>(type: "VARCHAR2(10)", nullable: false),
                    LocationDescription = table.Column<string>(type: "VARCHAR2(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactoryLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormListHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FormId = table.Column<int>(type: "NUMBER(10,0)", nullable: false),
                    FormName = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    FormDescription = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    FormData = table.Column<string>(type: "CLOB", nullable: false),
                    FormSubmittedData = table.Column<string>(type: "CLOB", nullable: true),
                    FormStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    FormRevision = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    Owner = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    OwnerCostCenter = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    RunningNumber = table.Column<int>(type: "NUMBER(10,0)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    CreatedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    ModifiedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    SubmittedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    ArchievedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    JSON = table.Column<string>(type: "CLOB", nullable: false),
                    MasterFormDetail = table.Column<string>(type: "VARCHAR2(300)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormListHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MasterFormId = table.Column<int>(type: "NUMBER(10,0)", nullable: false),
                    FormName = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    FormDescription = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    FormData = table.Column<string>(type: "CLOB", nullable: false),
                    FormSubmittedData = table.Column<string>(type: "CLOB", nullable: false),
                    FormStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    FormRevision = table.Column<string>(type: "VARCHAR2(20)", nullable: false),
                    GuidelineFile = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    UniqueGuidelineFile = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    JSON = table.Column<string>(type: "CLOB", nullable: true),
                    MasterFormDetail = table.Column<string>(type: "VARCHAR2(300)", nullable: false),
                    RunningNumber = table.Column<int>(type: "NUMBER(10,0)", nullable: false),
                    Owner = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    OwnerCostCenter = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    OwnerEmailAddress = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    CreatedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    ModifiedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    SubmittedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    ExpiredDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasterFormListHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MasterFormId = table.Column<int>(type: "NUMBER(10,0)", nullable: false),
                    MasterFormName = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    MasterFormDescription = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    MasterFormData = table.Column<string>(type: "CLOB", nullable: false),
                    MasterFormStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    MasterFormRevision = table.Column<string>(type: "VARCHAR2(20)", nullable: false),
                    Owner = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    CCBApprovalJSON = table.Column<string>(type: "CLOB", nullable: false),
                    JSON = table.Column<string>(type: "CLOB", nullable: false),
                    ChangeLog = table.Column<string>(type: "CLOB", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    CreatedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    ModifiedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    SubmittedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    ArchievedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterFormListHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasterFormLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MasterFormName = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    MasterFormDescription = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    MasterFormStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    MasterFormRevision = table.Column<string>(type: "VARCHAR2(20)", nullable: false),
                    GuidelineFile = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    UniqueGuidelineFile = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    DepartmentCode = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    SubDepartmentCode = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    FactoryCode = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    DocumentCategoryCode = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    AssignedNumberCode = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Owner = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    OwnerCostCenter = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    OwnerEmailAddress = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    CreatedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    ModifiedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    SubmittedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    CurrentEditor = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    ChangeLog = table.Column<string>(type: "CLOB", nullable: true),
                    AllowUpRevision = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MasterFormParentId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RunningNumber = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FormApprovalJSON = table.Column<string>(type: "CLOB", nullable: false),
                    JSON = table.Column<string>(type: "CLOB", nullable: true),
                    PermittedDepartments = table.Column<string>(type: "CLOB", nullable: true),
                    MasterFormData = table.Column<string>(type: "CLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterFormLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicFormApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DateTime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    LogDetail = table.Column<string>(type: "VARCHAR2(300)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicFormApprovals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicLogActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DateTime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    LogDetail = table.Column<string>(type: "VARCHAR2(300)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicLogActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicLogEmailNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DateTime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    LogDetail = table.Column<string>(type: "VARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicLogEmailNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicLogSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DateTime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    LogDetail = table.Column<string>(type: "VARCHAR2(300)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicLogSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserManualLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserManualName = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    UserManualDescription = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    UserManualFilePath = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    CreatedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    LatestUpdatedBy = table.Column<string>(type: "VARCHAR2(150)", nullable: true),
                    LatestUpdatedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserManualLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemRoles_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubDepartmentLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SubDepartmentCode = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    SubDepartmentDescription = table.Column<string>(type: "NVARCHAR2(200)", nullable: true),
                    DepartmentListId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubDepartmentLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubDepartmentLists_DepartmentLists_DepartmentListId",
                        column: x => x.DepartmentListId,
                        principalTable: "DepartmentLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormListApprovalLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EmailReminder = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ReminderType = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ApproveCondition = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    NotificationType = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    LastSend = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    FormListId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormListApprovalLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormListApprovalLevels_FormLists_FormListId",
                        column: x => x.FormListId,
                        principalTable: "FormLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterFormDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DepartmentName = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    MasterFormListId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterFormDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterFormDepartments_MasterFormLists_MasterFormListId",
                        column: x => x.MasterFormListId,
                        principalTable: "MasterFormLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormListApprovers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ApproverName = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ApproverEmail = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    EmployeeId = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ApproverStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    ApproveDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    Remark = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    FormListApprovalLevelId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormListApprovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormListApprovers_FormListApprovalLevels_FormListApprovalLevelId",
                        column: x => x.FormListApprovalLevelId,
                        principalTable: "FormListApprovalLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterFormCCBApprovalLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EmailReminder = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ReminderType = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ApproveCondition = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    NotificationType = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    LastSend = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    MasterFormDepartmentId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterFormCCBApprovalLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterFormCCBApprovalLevels_MasterFormDepartments_MasterFormDepartmentId",
                        column: x => x.MasterFormDepartmentId,
                        principalTable: "MasterFormDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterFormCCBApprovers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ApproverName = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ApproverEmail = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    EmployeeId = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    ApproverStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    ApproveDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    Remark = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    MasterFormCCBApprovalLevelId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterFormCCBApprovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterFormCCBApprovers_MasterFormCCBApprovalLevels_MasterFormCCBApprovalLevelId",
                        column: x => x.MasterFormCCBApprovalLevelId,
                        principalTable: "MasterFormCCBApprovalLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "\"NormalizedName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "\"NormalizedUserName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FormListApprovalLevels_FormListId",
                table: "FormListApprovalLevels",
                column: "FormListId");

            migrationBuilder.CreateIndex(
                name: "IX_FormListApprovers_FormListApprovalLevelId",
                table: "FormListApprovers",
                column: "FormListApprovalLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterFormCCBApprovalLevels_MasterFormDepartmentId",
                table: "MasterFormCCBApprovalLevels",
                column: "MasterFormDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterFormCCBApprovers_MasterFormCCBApprovalLevelId",
                table: "MasterFormCCBApprovers",
                column: "MasterFormCCBApprovalLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterFormDepartments_MasterFormListId",
                table: "MasterFormDepartments",
                column: "MasterFormListId");

            migrationBuilder.CreateIndex(
                name: "IX_SubDepartmentLists_DepartmentListId",
                table: "SubDepartmentLists",
                column: "DepartmentListId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemRoles_ApplicationUserId",
                table: "SystemRoles",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementLists");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CalendarLists");

            migrationBuilder.DropTable(
                name: "CostCenterLists");

            migrationBuilder.DropTable(
                name: "DocumentCategoryLists");

            migrationBuilder.DropTable(
                name: "FactoryLists");

            migrationBuilder.DropTable(
                name: "FormListApprovers");

            migrationBuilder.DropTable(
                name: "FormListHistories");

            migrationBuilder.DropTable(
                name: "MasterFormCCBApprovers");

            migrationBuilder.DropTable(
                name: "MasterFormListHistories");

            migrationBuilder.DropTable(
                name: "PublicFormApprovals");

            migrationBuilder.DropTable(
                name: "PublicLogActivities");

            migrationBuilder.DropTable(
                name: "PublicLogEmailNotifications");

            migrationBuilder.DropTable(
                name: "PublicLogSystems");

            migrationBuilder.DropTable(
                name: "SubDepartmentLists");

            migrationBuilder.DropTable(
                name: "SystemRoles");

            migrationBuilder.DropTable(
                name: "UserManualLists");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "FormListApprovalLevels");

            migrationBuilder.DropTable(
                name: "MasterFormCCBApprovalLevels");

            migrationBuilder.DropTable(
                name: "DepartmentLists");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FormLists");

            migrationBuilder.DropTable(
                name: "MasterFormDepartments");

            migrationBuilder.DropTable(
                name: "MasterFormLists");
        }
    }
}
