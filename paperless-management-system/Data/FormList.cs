using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class FormList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "NUMBER(10, 0)")]
        public int MasterFormId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        [Display(Name = "Form Name")]
        public string FormName { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(250)")]
        [Display(Name = "Form Description")]
        public string? FormDescription { get; set; }

        [Required]
        [Column(TypeName = "CLOB")]
        public string FormData { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "CLOB")]
        public string? FormSubmittedData { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public string FormStatus { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(20)")]
        public string FormRevision { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(250)")]
        public string? GuidelineFile { get; set; }

        [Column(TypeName = "VARCHAR2(250)")]
        public string? UniqueGuidelineFile { get; set; }

        [Column(TypeName = "CLOB")]
        public string? JSON { get; set; }

        [Column(TypeName = "VARCHAR2(300)")]
        public string MasterFormDetail { get; set; } = JsonConvert.SerializeObject(new MasterFormDetail());

        [Column(TypeName = "NUMBER(10, 0)")]
        public int RunningNumber { get; set; }

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

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? ExpiredDate { get; set; }

        public ICollection<FormListApprovalLevel>? FormListApprovalLevels { get; set; }
    }

    public class MasterFormDetail
    {
        public string MasterFormOwner { get; set; } = String.Empty;
        public string MasterFormOwnerDepartment { get; set; } = String.Empty;
    }
}
