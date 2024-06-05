using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class MasterFormCCBApprovalLevel
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR2(100)")]
        public string EmailReminder { get; set; } = "60";

        [Column(TypeName = "VARCHAR2(100)")]
        public string ReminderType { get; set; } = "Minutes";

        [Column(TypeName = "VARCHAR2(100)")]
        [Display(Name = "Approval Type")]
        public string ApproveCondition { get; set; } = "single";

        [Column(TypeName = "VARCHAR2(100)")]
        public string NotificationType { get; set; } = "By Name/ Employee";

        [Column(TypeName = "VARCHAR2(50)")]
        [Display(Name = "Approval Status")]
        public string ApprovalStatus { get; set; } = "none";

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? LastSend { get; set; }

        public int? MasterFormDepartmentId { get; set; }

        public virtual MasterFormDepartment? MasterFormDepartment { get; set; }

        public ICollection<MasterFormCCBApprover>? MasterFormCCBApprovers { get; set; }
    }
}
