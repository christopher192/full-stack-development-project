using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WD_ERECORD_CORE.ViewModels
{
    public class CalendarListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Calendar Title")]
        public string Title { get; set; } = String.Empty;

        [Display(Name = "Calendar Description")]
        public string? Description { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan Endtime { get; set; }
    }
}
