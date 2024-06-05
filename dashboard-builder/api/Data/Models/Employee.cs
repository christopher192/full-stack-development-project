using System.ComponentModel.DataAnnotations;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Data.Models
{
    public class Employee
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public string Office { get; set; } = string.Empty;

        public string Age { get; set; } = string.Empty;

        public string Salary { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public string Sex { get; set; } = string.Empty;
    }
}
