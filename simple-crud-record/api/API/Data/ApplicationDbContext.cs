using Microsoft.EntityFrameworkCore;
using API.Data.Models;

namespace API.Data
{
    public class ApplicationDbContext : DbContext
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

            modelBuilder.Entity<Record>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 4)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 4)");
                entity.Property(e => e.TotalRevenue).HasColumnType("decimal(18, 4)");
                entity.Property(e => e.TotalCost).HasColumnType("decimal(18, 4)");
                entity.Property(e => e.TotalProfit).HasColumnType("decimal(18, 4)");
            });
        }

        public DbSet<Record> Records => Set<Record>();
    }
}
