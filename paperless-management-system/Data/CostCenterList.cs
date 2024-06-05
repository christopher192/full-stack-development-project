using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class CostCenterList
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Cost center code is required")]
        public string Code { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(150)")]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Cost center name is required")]
        public string Name { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(150)")]
        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        [Display(Name = "Job Classification/ Track")]
        public string? JobClassificationTrack { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
