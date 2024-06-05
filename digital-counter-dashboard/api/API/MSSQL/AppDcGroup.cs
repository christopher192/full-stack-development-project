using System;
using System.Collections.Generic;

namespace API.MSSQL;

public partial class AppDcGroup
{
    public int Id { get; set; }

    public string? GroupName { get; set; }

    public string? DateCreated { get; set; }

    public int? Status { get; set; }
}
