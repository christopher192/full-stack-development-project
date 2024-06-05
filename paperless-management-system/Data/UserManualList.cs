using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class UserManualList
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User manual name is required")]
        [Column(TypeName = "VARCHAR2(250)")]
        [Display(Name = "Name")]
        public string UserManualName { get; set; } = String.Empty;

        [Required(ErrorMessage = "User manual description is required")]
        [Column(TypeName = "VARCHAR2(250)")]
        [Display(Name = "Description")]
        public string? UserManualDescription { get; set; }

        [Column(TypeName = "VARCHAR2(100)")]
        public string Status { get; set; } = "active";

        [Column(TypeName = "VARCHAR2(250)")]
        [Display(Name = "Upload File")]
        public string UserManualFilePath { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(150)")]
        public string CreatedBy { get; set; } = String.Empty;

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? DateCreated { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        public string? LatestUpdatedBy { get; set; } = String.Empty;

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? LatestUpdatedDate { get; set; }

    }
}
