using System.ComponentModel.DataAnnotations;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Data.Models
{
    public class Project
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Task { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;

        public string AssignedTo { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;
    }
}
