using System;
using System.Collections.Generic;

namespace API.DTO;

public class AppDcGroupShiftDTO
{
    public int Id { get; set; }

    [GraphQLName("date")]
    public DateTime? Date { get; set; }

    [GraphQLName("shift_id")]
    public int? Shift_Id { get; set; }

    [GraphQLName("group_id")]
    public int? Group_Id { get; set; }

    public int? Status { get; set; }
}
