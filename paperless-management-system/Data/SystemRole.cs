using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class SystemRole
    {
        [Key]
        public int Id { get; set; }
        
        [Column(TypeName = "VARCHAR2(250)")]
        [Display(Name = "System Role")]
        public string? Name { get; set; }

        public string? ApplicationUserId { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
