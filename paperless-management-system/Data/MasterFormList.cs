using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WD_ERECORD_CORE.Data
{
    public class MasterFormList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        [Display(Name = "Master Form Name")]
        public string MasterFormName { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(250)")]
        [Display(Name = "Master Form Description")]
        public string? MasterFormDescription { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public string MasterFormStatus { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(20)")]
        public string MasterFormRevision { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(250)")]
        public string? GuidelineFile { get; set; }

        [Column(TypeName = "VARCHAR2(250)")]
        public string? UniqueGuidelineFile { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public string DepartmentCode { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public string SubDepartmentCode { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public string FactoryCode { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public string DocumentCategoryCode { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "NUMBER(10)")]
        public int AssignedNumberCode { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(150)")]
        public string Owner { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public string OwnerCostCenter { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public string OwnerEmailAddress { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(150)")]
        public string CreatedBy { get; set; } = String.Empty;

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? ModifiedDate { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        public string? ModifiedBy { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? SubmittedDate { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        public string? SubmittedBy { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        public string? CurrentEditor { get; set; }

        [Column(TypeName = "CLOB")]
        [Display(Name = "Change Log")]
        public string? ChangeLog { get; set; }

        [Column(TypeName = "NUMBER(1)")]
        public bool AllowUpRevision { get; set; } = false;

        [Column(TypeName = "NUMBER(10)")]
        public int MasterFormParentId { get; set; }

        [Column(TypeName = "NUMBER(10)")]
        public int RunningNumber { get; set; } = 1;

        [Column(TypeName = "CLOB")]
        public string FormApprovalJSON { get; set; } = JsonConvert.SerializeObject(new FormApproval());

        [Column(TypeName = "CLOB")]
        public string? JSON { get; set; }

        [Column(TypeName = "CLOB")]
        public string? PermittedDepartments { get; set; }

        [Column(TypeName = "CLOB")]
        public string MasterFormData { get; set; } = String.Format("{{{0}}}", "\"display\":\"form\",\"components\":[]");

        public ICollection<MasterFormDepartment>? MasterFormDepartments { get; set; }
    }

    public class FormApproval 
    {
        public List<FormApprovalLevel> FixFormApproval { get; set; } = new List<FormApprovalLevel>();
        public List<FormApprovalLevel> EditableFormApproval { get; set; } = new List<FormApprovalLevel>();
    }

    public class FormApprovalLevel
    {
        public int Id { get; set; }

        [Display(Name = "Email Reminder")]
        public string EmailReminder { get; set; } = "60";

        [Display(Name = "Reminder Type")]
        public string ReminderType { get; set; } = "Minutes";

        [Display(Name = "Approve Condition")]
        public string ApproveCondition { get; set; } = "single";

        [Display(Name = "Notification Type")]
        public string NotificationType { get; set; } = "By Name/ Employee";

        [BindProperty]
        public List<FormApprover> FormApprovers { get; set; } = new List<FormApprover>();
    }

    public class FormApprover
    {
        public int Id { get; set; }

        [Display(Name = "Approver Name")]
        public string ApproverName { get; set; } = String.Empty;

        [Display(Name = "Approver Email")]
        public string ApproverEmail { get; set; } = String.Empty;

        public string EmployeeId { get; set; } = String.Empty;

        public int FormApprovalLevelId { get; set; }
    }
}
