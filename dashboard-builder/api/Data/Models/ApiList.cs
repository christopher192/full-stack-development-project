using System.ComponentModel.DataAnnotations;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Data.Models
{
    public class ApiList
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string URL { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;
    }
}
