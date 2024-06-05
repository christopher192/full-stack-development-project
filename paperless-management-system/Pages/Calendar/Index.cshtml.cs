using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.Calendar
{
    public class IndexModel : CalendarPageModel
    {
        private readonly ApplicationDbContext _context;
        public UserManager<ApplicationUser> UserManager { get; set; }

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> mgr)
        {
            _context = context;
            UserManager = mgr;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await UserManager.GetUserAsync(HttpContext.User);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var User = GetCurrentUser().Result;

            var CalendarLists = _context.CalendarLists.Where(x => x.UserId == User.UserName).ToList().Select(x => new { id = x.Id, title = (x.Title == string.Empty || x.Title == null ? "" : x.Title), description = (x.Description == string.Empty || x.Description == null ? "" : x.Description), start = x.StartDateTime.Date.ToString("yyyy-MM-dd") + "T" + x.StartDateTime.TimeOfDay.ToString(), end = x.EndDateTime.Date.ToString("yyyy-MM-dd") + "T" + x.EndDateTime.TimeOfDay.ToString(), className = "fc-event-light fc-event-solid-primary" });

            ViewData["CalendarData"] = CalendarLists;

            return Page();
        }
    }
}
