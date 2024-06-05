using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;

namespace WD_ERECORD_CORE.Pages.Calendar
{
    public class CreateModel : CalendarPageModel
    {
        private readonly ApplicationDbContext _context;
        public UserManager<ApplicationUser> UserManager { get; set; }

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            UserManager = userManager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await UserManager.GetUserAsync(HttpContext.User);
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CalendarListViewModel CalendarListViewModel { get; set; } = new CalendarListViewModel();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var User = await GetCurrentUser();

            var StartDateTimeResult = this.CalendarListViewModel.StartDate + this.CalendarListViewModel.StartTime;
            var EndDateTimeResult = this.CalendarListViewModel.EndDate + this.CalendarListViewModel.Endtime;

            var CalendarList = new CalendarList();
            CalendarList.StartDateTime = StartDateTimeResult;
            CalendarList.EndDateTime = EndDateTimeResult;
            CalendarList.Title = this.CalendarListViewModel.Title;
            CalendarList.Description = this.CalendarListViewModel.Description;
            CalendarList.UserId = User.UserName;

            _context.CalendarLists.Add(CalendarList);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
