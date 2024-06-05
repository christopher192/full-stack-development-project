using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WD_ERECORD_CORE.Data
{
    public class AnnouncementList
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Label 1")]
        [Column(TypeName = "VARCHAR2(150)")]
        public string? Label1 { get; set; }

        [Display(Name = "Label 2")]
        [Column(TypeName = "VARCHAR2(150)")]
        public string? Label2 { get; set; }

        [Column(TypeName = "CLOB")]
        [Display(Name = "Announcement Image")]
        public string Image { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(150)")]
        public string Status { get; set; } = String.Empty;

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        public DateTime UploadDate { get; set; }
    }
}
