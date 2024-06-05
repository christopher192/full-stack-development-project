using System;
using System.Collections.Generic;

namespace API.MSSQL;

public partial class AppProcessWutgSasmMachine
{
    public DateTime? Date { get; set; }

    public string? Shift { get; set; }

    public string? Wo { get; set; }

    public string? Batch { get; set; }

    public string? LotNumber { get; set; }

    public string? Article { get; set; }

    public string? MachineOperator { get; set; }

    public string? InspectionOperator1 { get; set; }

    public string? InspectionOperator2 { get; set; }

    public string? OperatorCode { get; set; }

    public string? Machine { get; set; }

    public DateTime? StartDateTime { get; set; }

    public DateTime? StartEndTime { get; set; }

    public string? GlassTypeDesc { get; set; }

    public string? RawGlassPCode { get; set; }

    public float? RawGlassPCode6Dig { get; set; }

    public decimal? TotalReject { get; set; }

    public decimal? TotalInput { get; set; }

    public decimal? TotalGood { get; set; }

    public string? ArticleGrouping { get; set; }

    public decimal? BrokenDuringCutting { get; set; }

    public decimal? SasmSetup { get; set; }

    public string? Remark { get; set; }

    public string? VerifiedSupervisor { get; set; }

    public string? Year { get; set; }

    public short? Month { get; set; }

    public short? Day { get; set; }

    public string? Length { get; set; }

    public string? Width { get; set; }

    public string? Thickness { get; set; }

    public string? GlassType { get; set; }

    public string? RecordTime { get; set; }

    public int Status { get; set; }

    public string? Operator { get; set; }

    public string? Diameter { get; set; }

    public decimal? Others { get; set; }

    public decimal? BalanceReturn { get; set; }

    public decimal? SortReturn { get; set; }

    public string? OpShift { get; set; }

    public decimal BrokenDuringHandling { get; set; }

    public decimal? RejectSensor { get; set; }
}
