using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class DashboardBuilderData
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Data { get; set; } = string.Empty;

        [Required]
        public string ProjectData { get; set; } = string.Empty;

        [Required]
        public string ComponentsData { get; set; } = string.Empty;
    }
}
