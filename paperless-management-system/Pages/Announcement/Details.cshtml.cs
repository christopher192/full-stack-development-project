using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.Announcement
{
    public class DetailsModel : AnnouncementPageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public AnnouncementList AnnouncementList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AnnouncementList = await _context.AnnouncementLists.FirstOrDefaultAsync(m => m.Id == id);

            if (AnnouncementList == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
