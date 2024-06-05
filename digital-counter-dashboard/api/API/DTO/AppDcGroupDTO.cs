using System;
using System.Collections.Generic;

namespace API.DTO;

public class AppDcGroupDTO
{
    public int Id { get; set; }

    [GraphQLName("group_name")]
    public string? Group_Name { get; set; }

    [GraphQLName("date_created")]
    public string? Date_Created { get; set; }

    public int? Status { get; set; }
}
