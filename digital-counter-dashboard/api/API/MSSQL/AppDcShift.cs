using System;
using System.Collections.Generic;

namespace API.MSSQL;

public partial class AppDcShift
{
    public int Id { get; set; }

    public string? ShiftName { get; set; }

    public DateTime? DateCreated { get; set; }

    public string? Status { get; set; }
}
