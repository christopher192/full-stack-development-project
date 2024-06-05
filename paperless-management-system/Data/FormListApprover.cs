using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class FormListApprover
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Approver Name")]
        [Column(TypeName = "VARCHAR2(100)")]
        public string ApproverName { get; set; } = String.Empty;

        [Display(Name = "Approver Email")]
        [Column(TypeName = "VARCHAR2(100)")]
        public string ApproverEmail { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(100)")]
        public string EmployeeId { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(50)")]
        public string ApproverStatus { get; set; } = "none";

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? ApproveDate { get; set; }

        [Column(TypeName = "VARCHAR2(250)")]
        public string? Remark { get; set; }

        public int? FormListApprovalLevelId { get; set; }
        public virtual FormListApprovalLevel? FormListApprovalLevel { get; set; }
    }
}
