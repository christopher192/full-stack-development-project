using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        // User
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }

        // Master Form
        public DbSet<MasterFormList> MasterFormLists { get; set; }
        public DbSet<MasterFormDepartment> MasterFormDepartments { get; set; }
        public DbSet<MasterFormCCBApprovalLevel> MasterFormCCBApprovalLevels { get; set; }
        public DbSet<MasterFormCCBApprover> MasterFormCCBApprovers { get; set; }

        // Form
        public DbSet<FormList> FormLists { get; set; }
        public DbSet<FormListApprovalLevel> FormListApprovalLevels { get; set; }
        public DbSet<FormListApprover> FormListApprovers { get; set; }

        // Numberic Schema
        public DbSet<DepartmentList> DepartmentLists { get; set; }
        public DbSet<SubDepartmentList> SubDepartmentLists { get; set; }
        public DbSet<FactoryList> FactoryLists { get; set; }
        public DbSet<DocumentCategoryList> DocumentCategoryLists { get; set; }
        public DbSet<CostCenterList> CostCenterLists { get; set; }

        // User Manual
        public DbSet<UserManualList> UserManualLists { get; set; }

        // History
        public DbSet<MasterFormListHistory> MasterFormListHistories { get; set; }
        public DbSet<FormListHistory> FormListHistories { get; set; }

        // Public Log
        public DbSet<PublicLogSystem> PublicLogSystems { get; set; }
        public DbSet<PublicLogApproval> PublicFormApprovals { get; set; }
        public DbSet<PublicLogEmailNotification> PublicLogEmailNotifications { get; set; }
        public DbSet<PublicLogActivity> PublicLogActivities { get; set; }
        public DbSet<PublicLogTransferForm> PublicLogTransferForms { get; set; }

/*        public DbSet<FormSubmissionHistory> FormSubmissionHistories { get; set; }*/

        // Basic
        public DbSet<AnnouncementList> AnnouncementLists { get; set; }
        public DbSet<CalendarList> CalendarLists { get; set; }

        // API
/*        public DbSet<APISetting> APISettings { get; set; }
        public DbSet<APISettingLog> APISettingLogs { get; set; }*/

        // Email Grouping
        public DbSet<EmailGroupList> EmailGroupLists { get; set; }
        public DbSet<EmailGroupUserList> EmailGroupUserLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(s => s.SystemRoles)
                .WithOne(a => a.ApplicationUser)
                .HasForeignKey(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DepartmentList>()
                .HasMany(c => c.SubDepartmentLists)
                .WithOne(p => p.DepartmentList)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmailGroupList>()
                .HasMany(c => c.EmailGroupUserLists)
                .WithOne(p => p.EmailGroupList)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MasterFormList>()
                .HasMany(c => c.MasterFormDepartments)
                .WithOne(p => p.MasterFormList)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MasterFormDepartment>()
                .HasMany(c => c.MasterFormCCBApprovalLevels)
                .WithOne(p => p.MasterFormDepartment)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MasterFormCCBApprovalLevel>()
                .HasMany(c => c.MasterFormCCBApprovers)
                .WithOne(p => p.MasterFormCCBApprovalLevel)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FormList>()
                .HasMany(c => c.FormListApprovalLevels)
                .WithOne(p => p.FormList)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FormListApprovalLevel>()
                .HasMany(c => c.FormListApprovers)
                .WithOne(p => p.FormListApprovalLevel)
                .OnDelete(DeleteBehavior.Cascade);

/*            modelBuilder.Entity<FormSubmissionHistory>()
                .Property(e => e.FormSubmission)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<FormSubmissionJSON>>(v, (JsonSerializerOptions)null));*/
        }
    }
}