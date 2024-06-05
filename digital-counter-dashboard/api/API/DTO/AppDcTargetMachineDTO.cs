using System;
using System.Collections.Generic;

namespace API.DTO;

public class AppDcTargetMachineDTO
{
    public int Id { get; set; }

    [GraphQLName("target_morning")]
    public int? Target_Morning { get; set; }

    [GraphQLName("target_afternoon")]
    public int? Target_Afternoon { get; set; }

    [GraphQLName("target_night")]
    public int? Target_Night { get; set; }

    [GraphQLName("machine_name")]
    public string? Machine_Name { get; set; }

    public string? Dimension { get; set; }

    public int? Status { get; set; }

    [GraphQLName("sum_total_input")]
    public string? Sum_Total_Input { get; set; }

    [GraphQLName("sum_good_operator_1")]
    public string? Sum_Good_Operator_1 { get; set; }

    [GraphQLName("sum_good_operator_2")]
    public string? Sum_Good_Operator_2 { get; set; }

    [GraphQLName("sum_total_good")]
    public string? Sum_Total_Good { get; set; }

    [GraphQLName("sum_total_good_cutting")]
    public string? Sum_Total_Good_Cutting { get; set; }
}
