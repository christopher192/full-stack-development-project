using System;
using System.Collections.Generic;

namespace API.DTO;

public class AppDcShiftDTO
{
    public int Id { get; set; }

    [GraphQLName("shift_name")]
    public string? Shift_Name { get; set; }

    [GraphQLName("date_created")]
    public DateTime? Date_created { get; set; }

    [GraphQLName("start_time")]
    public TimeSpan? Start_Time { get; set; }

    [GraphQLName("end_time")]
    public TimeSpan? End_Time { get; set; }

    public string? Status { get; set; }
}

