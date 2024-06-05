using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WD_ERECORD_CORE.ViewModels
{
    public class AnnouncementListEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Label 1")]
        public string? Label1 { get; set; }

        [Display(Name = "Label 2")]
        public string? Label2 { get; set; }

        [Display(Name = "Announcement Image")]
        public IFormFile? UploadImage { get; set; }

        public string? Image { get; set; }

        public string Status { get; set; } = String.Empty;
    }
}
