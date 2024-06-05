using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WD_ERECORD_CORE.Data
{
    public class APISettingLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "NUMBER(10, 0)")]
        public int FormId { get; set; }

        [Column(TypeName = "VARCHAR2(250)")]
        public string? APISettingIds { get; set; }
    }
}
