using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.DTO;

public class AppDcMachineDTO
{
    public int Id { get; set; }

    [GraphQLName("machine_name")]
    public string? Machine_Name { get; set; }

    public string? Dimension { get; set; }

    [GraphQLName("date_created")]
    public DateTime? Date_Created { get; set; }

    public int? Status { get; set; }

    [GraphQLName("last_updated")]
    public DateTime? Last_Updated { get; set; }
}
