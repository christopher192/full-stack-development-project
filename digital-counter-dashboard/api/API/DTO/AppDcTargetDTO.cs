using System;
using System.Collections.Generic;

namespace API.DTO;

public class AppDcTargetDTO
{
    public int Id { get; set; }

    [GraphQLName("machine_id")]
    public int? Machine_Id { get; set; }

    [GraphQLName("target_morning")]
    public int? Target_Morning { get; set; }

    [GraphQLName("target_afternoon")]
    public int? Target_Afternoon { get; set; }

    [GraphQLName("target_night")]
    public int? Target_Night { get; set; }

    [GraphQLName("date")]
    public DateTime? Date { get; set; }
}
