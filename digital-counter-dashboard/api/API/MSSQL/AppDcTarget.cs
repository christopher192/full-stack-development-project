using System;
using System.Collections.Generic;

namespace API.MSSQL;

public partial class AppDcTarget
{
    public int Id { get; set; }

    public int? MachineId { get; set; }

    public int? TargetMorning { get; set; }

    public int? TargetAfternoon { get; set; }

    public int? TargetNight { get; set; }

    public DateTime? Date { get; set; }
}
