using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WD_ERECORD_CORE.Data
{
    public class FormSubmissionHistory
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR2(450)")]
        public string UserId { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(256)")]
        public string UserName { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(250)")]
        public string DispalyName { get; set; } = String.Empty;

        [Column(TypeName = "CLOB")]
        public ICollection<FormSubmissionJSON> FormSubmission { get; set; } = new List<FormSubmissionJSON>();
    }

    public class FormSubmissionJSON
    {
        public string FormName { get; set; } = String.Empty;
        public string FormRevision { get; set; } = String.Empty;
        public DateTime SubmissionDate { get; set; }
        public int Count { get; set; }
    }
}
