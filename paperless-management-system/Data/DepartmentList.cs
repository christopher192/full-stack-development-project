using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class DepartmentList
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Department Code")]
        [Column(TypeName = "VARCHAR2(10)")]
        [Required(ErrorMessage = "Department Code is required")]
        public string DepartmentCode { get; set; } = String.Empty;

        [Display(Name = "Department Description")]
        [Column(TypeName = "VARCHAR2(200)")]
        public string? DepartmentDescription { get; set; }

        public ICollection<SubDepartmentList>? SubDepartmentLists { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
