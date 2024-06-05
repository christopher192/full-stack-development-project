using System.ComponentModel.DataAnnotations;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Data.Models
{
    public class PurchaseOrderHistory
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string DataId { get; set; } = string.Empty;

        public string SerielNumber { get; set; } = string.Empty;

        public string PurchaseId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string User { get; set; } = string.Empty;

        public string AssignedTo { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;
    }
}
