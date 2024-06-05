using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class MasterFormListHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "NUMBER(10,0)")]
        public int MasterFormId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public string MasterFormName { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(250)")]
        public string? MasterFormDescription { get; set; }

        [Column(TypeName = "CLOB")]
        public string MasterFormData { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public string MasterFormStatus { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(20)")]
        public string MasterFormRevision { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR2(150)")]
        public string Owner { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "CLOB")]
        public string? CCBApprovalJSON { get; set; }

        [Required]
        [Column(TypeName = "CLOB")]
        public string JSON { get; set; } = String.Empty;

        [Column(TypeName = "CLOB")]
        public string? ChangeLog { get; set; } = String.Empty;

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
        public DateTime ArchievedDate { get; set; }
    }
}
