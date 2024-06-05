using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class MasterFormDepartment
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR2(100)")]
        public string DepartmentName { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(50)")]
        public string Status { get; set; } = "no action";

        public int? MasterFormListId { get; set; }

        public virtual MasterFormList? MasterFormList { get; set; }

        public ICollection<MasterFormCCBApprovalLevel>? MasterFormCCBApprovalLevels { get; set; }
    }
}
