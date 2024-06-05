using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class WidgetBox
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int TotalCategory { get; set; }

        [Required]
        public int TotalListing { get; set; }

        [Required]
        public int ClaimedListing { get; set; }

        [Required]
        public int ReportedListing { get; set; }
    }
}
