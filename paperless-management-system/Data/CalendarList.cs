using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WD_ERECORD_CORE.Data
{
    public class CalendarList
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR2(100)")]
        public string UserId { get; set; } = String.Empty;

        [Display(Name = "Calendar Title")]
        [Column(TypeName = "VARCHAR2(150)")]
        public string Title { get; set; } = String.Empty;

        [Display(Name = "Calendar Description")]
        [Column(TypeName = "VARCHAR2(150)")]
        public string? Description { get; set; }

        [Display(Name = "Start Date")]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "End Date")]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime EndDateTime { get; set; }
    }
}
