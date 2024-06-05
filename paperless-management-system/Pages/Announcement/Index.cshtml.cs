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
    public class IndexModel : AnnouncementPageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<AnnouncementList> AnnouncementList { get;set; }

        public async Task OnGetAsync()
        {
            AnnouncementList = await _context.AnnouncementLists.ToListAsync();
        }
    }
}
