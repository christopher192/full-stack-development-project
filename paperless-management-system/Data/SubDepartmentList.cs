using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class SubDepartmentList
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Sub Department Code")]
        [Column(TypeName = "NVARCHAR2(10)")]
        [Required(ErrorMessage = "Sub Department Code is required")]
        public string SubDepartmentCode { get; set; } = String.Empty;

        [Display(Name = "Sub Department Description")]
        [Column(TypeName = "NVARCHAR2(200)")]
        public string? SubDepartmentDescription { get; set; }

        [NotMapped]
        [Display(Name = "Department Code")]
        [Required(ErrorMessage = "Department Code is required")]
        public int? DepartmentListInputId { get; set; }

        public int? DepartmentListId { get; set; }
        public virtual DepartmentList? DepartmentList { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }

        [NotMapped]
        public string? DepartmentCode { get; set; }
    }
}
