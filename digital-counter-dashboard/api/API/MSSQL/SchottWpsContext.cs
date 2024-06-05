using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.MSSQL;

public partial class SchottWpsContext : DbContext
{
    public SchottWpsContext()
    {
    }

    public SchottWpsContext(DbContextOptions<SchottWpsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppDcGroup> AppDcGroups { get; set; }

    public virtual DbSet<AppDcMachine> AppDcMachines { get; set; }

    public virtual DbSet<AppDcShift> AppDcShifts { get; set; }

    public virtual DbSet<AppDcTarget> AppDcTargets { get; set; }

    public virtual DbSet<AppProcessWutgBreakingInspection> AppProcessWutgBreakingInspections { get; set; }

    public virtual DbSet<AppProcessWutgSasmMachine> AppProcessWutgSasmMachines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=SCHOTT_WPS;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<AppDcGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Group");

            entity.ToTable("app_DC_Group");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("date_created");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("group_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<AppDcMachine>(entity =>
        {
            entity.ToTable("app_DC_Machine");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasColumnType("date")
                .HasColumnName("date_created");
            entity.Property(e => e.Dimension)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("dimension");
            entity.Property(e => e.LastUpdated)
                .HasColumnType("date")
                .HasColumnName("last_updated");
            entity.Property(e => e.MachineName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("machine_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<AppDcShift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Shift");

            entity.ToTable("app_DC_Shift");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_created");
            entity.Property(e => e.ShiftName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("shift_name");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("status");
        });

        modelBuilder.Entity<AppDcTarget>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("app_DC_Target");

            entity.Property(e => e.Date)
                .HasColumnType("date")
                .HasColumnName("date");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.TargetAfternoon).HasColumnName("target_afternoon");
            entity.Property(e => e.TargetMorning).HasColumnName("target_morning");
            entity.Property(e => e.TargetNight).HasColumnName("target_night");
        });

        modelBuilder.Entity<AppProcessWutgBreakingInspection>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("app_Process_WUTG_Breaking_Inspection");

            entity.Property(e => e.Article)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("article");
            entity.Property(e => e.ArticleGrouping)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("article_grouping");
            entity.Property(e => e.BalanceReturn)
                .HasDefaultValueSql("((0))")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("balance_return");
            entity.Property(e => e.Batch)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("batch");
            entity.Property(e => e.BrokenDuringHandling)
                .HasDefaultValueSql("((0))")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("broken_during_handling");
            entity.Property(e => e.BrokenDuringPacking)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("broken_during_packing");
            entity.Property(e => e.BrokenDuringSeperating)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("broken_during_seperating");
            entity.Property(e => e.BubblePlatinum)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("bubble_platinum");
            entity.Property(e => e.Crack)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("crack");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Day).HasColumnName("day");
            entity.Property(e => e.Diameter)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("diameter");
            entity.Property(e => e.EdgeDefect)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("edge_defect");
            entity.Property(e => e.Foogy)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("foogy");
            entity.Property(e => e.Frits)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("frits");
            entity.Property(e => e.GlassType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("glass_type");
            entity.Property(e => e.GlassTypeDesc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("glass_type_desc");
            entity.Property(e => e.GoodOperator1)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("good_operator_1");
            entity.Property(e => e.GoodOperator2)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("good_operator_2");
            entity.Property(e => e.GoodTotal)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("good_total");
            entity.Property(e => e.InspectionOperator1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("inspection_operator_1");
            entity.Property(e => e.InspectionOperator2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("inspection_operator_2");
            entity.Property(e => e.IntoInspection)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("into_inspection");
            entity.Property(e => e.Length)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("length");
            entity.Property(e => e.LongBubble)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("long_bubble");
            entity.Property(e => e.LotNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lot_number");
            entity.Property(e => e.Machine)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("machine");
            entity.Property(e => e.MachineOperator)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("machine_operator");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.OpShift)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("op_shift");
            entity.Property(e => e.Operator)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("operator");
            entity.Property(e => e.OperatorCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("operator_code");
            entity.Property(e => e.Others)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("others");
            entity.Property(e => e.PlatiniumNeedle)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("platinium_needle");
            entity.Property(e => e.RawGlassPCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("raw_glass_p_code");
            entity.Property(e => e.RawGlassPCode6Dig).HasColumnName("raw_glass_p_code_6_dig");
            entity.Property(e => e.RecordTime)
                .IsUnicode(false)
                .HasColumnName("record_time");
            entity.Property(e => e.Remark)
                .IsUnicode(false)
                .HasColumnName("remark");
            entity.Property(e => e.Scratches)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("scratches");
            entity.Property(e => e.Shift)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("shift");
            entity.Property(e => e.SortReturn)
                .HasDefaultValueSql("((0))")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("sort_return");
            entity.Property(e => e.Stain)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("stain");
            entity.Property(e => e.StartDateTime)
                .HasColumnType("datetime")
                .HasColumnName("start_date_time");
            entity.Property(e => e.StartEndTime)
                .HasColumnType("datetime")
                .HasColumnName("start_end_time");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StrieStrip)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("strie_strip");
            entity.Property(e => e.Thickness)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("thickness");
            entity.Property(e => e.TotalGood)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("total_good");
            entity.Property(e => e.TotalInput)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("total_input");
            entity.Property(e => e.TotalReject)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("total_reject");
            entity.Property(e => e.VerifiedSupervisor)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("verified_supervisor");
            entity.Property(e => e.Width)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("width");
            entity.Property(e => e.Wo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("wo");
            entity.Property(e => e.Year)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("year");
        });

        modelBuilder.Entity<AppProcessWutgSasmMachine>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("app_Process_WUTG_SASM_Machine");

            entity.Property(e => e.Article)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("article");
            entity.Property(e => e.ArticleGrouping)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("article_grouping");
            entity.Property(e => e.BalanceReturn)
                .HasDefaultValueSql("((0))")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("balance_return");
            entity.Property(e => e.Batch)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("batch");
            entity.Property(e => e.BrokenDuringCutting)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("broken_during_cutting");
            entity.Property(e => e.BrokenDuringHandling)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("broken_during_handling");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Day).HasColumnName("day");
            entity.Property(e => e.Diameter)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("diameter");
            entity.Property(e => e.GlassType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("glass_type");
            entity.Property(e => e.GlassTypeDesc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("glass_type_desc");
            entity.Property(e => e.InspectionOperator1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("inspection_operator_1");
            entity.Property(e => e.InspectionOperator2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("inspection_operator_2");
            entity.Property(e => e.Length)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("length");
            entity.Property(e => e.LotNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lot_number");
            entity.Property(e => e.Machine)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("machine");
            entity.Property(e => e.MachineOperator)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("machine_operator");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.OpShift)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("op_shift");
            entity.Property(e => e.Operator)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("operator");
            entity.Property(e => e.OperatorCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("operator_code");
            entity.Property(e => e.Others)
                .HasDefaultValueSql("((0))")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("others");
            entity.Property(e => e.RawGlassPCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("raw_glass_p_code");
            entity.Property(e => e.RawGlassPCode6Dig).HasColumnName("raw_glass_p_code_6_dig");
            entity.Property(e => e.RecordTime)
                .IsUnicode(false)
                .HasColumnName("record_time");
            entity.Property(e => e.RejectSensor)
                .HasDefaultValueSql("((0))")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("reject_sensor");
            entity.Property(e => e.Remark)
                .IsUnicode(false)
                .HasColumnName("remark");
            entity.Property(e => e.SasmSetup)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("sasm_setup");
            entity.Property(e => e.Shift)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("shift");
            entity.Property(e => e.SortReturn)
                .HasDefaultValueSql("((0))")
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("sort_return");
            entity.Property(e => e.StartDateTime)
                .HasColumnType("datetime")
                .HasColumnName("start_date_time");
            entity.Property(e => e.StartEndTime)
                .HasColumnType("datetime")
                .HasColumnName("start_end_time");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Thickness)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("thickness");
            entity.Property(e => e.TotalGood)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("total_good");
            entity.Property(e => e.TotalInput)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("total_input");
            entity.Property(e => e.TotalReject)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("total_reject");
            entity.Property(e => e.VerifiedSupervisor)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("verified_supervisor");
            entity.Property(e => e.Width)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("width");
            entity.Property(e => e.Wo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("wo");
            entity.Property(e => e.Year)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("year");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
