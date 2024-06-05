using Microsoft.EntityFrameworkCore;
using API.Data.Models;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApiList> ApiLists => Set<ApiList>();
        public DbSet<DashboardBuilderData> DashboardBuilderDatas => Set<DashboardBuilderData>();
        public DbSet<WidgetBox> WidgetBoxs => Set<WidgetBox>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Finance> Finances => Set<Finance>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<PurchaseOrderHistory> PurchaseOrderHistories => Set<PurchaseOrderHistory>();
    }
}
