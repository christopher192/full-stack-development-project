using System;
using System.Collections.Generic;

namespace API.MSSQL;

public partial class AppDcMachine
{
    public int Id { get; set; }

    public string? MachineName { get; set; }

    public string? Dimension { get; set; }

    public DateTime? DateCreated { get; set; }

    public int? Status { get; set; }

    public DateTime? LastUpdated { get; set; }
}
