using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;

namespace WD_ERECORD_CORE.Pages.Calendar
{
    public class EditModel : CalendarPageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CalendarListViewModel CalendarListViewModel { get; set; } = new CalendarListViewModel();

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var CalendarList = await _context.CalendarLists.FirstOrDefaultAsync(m => m.Id == Id);

            if (CalendarList == null)
            {
                return NotFound();
            }
            else
            {
                this.CalendarListViewModel.Id = CalendarList.Id;
                this.CalendarListViewModel.Title = CalendarList.Title;
                this.CalendarListViewModel.Description = CalendarList.Description;
                this.CalendarListViewModel.StartDate = CalendarList.StartDateTime.Date;
                this.CalendarListViewModel.EndDate = CalendarList.EndDateTime.Date;
                this.CalendarListViewModel.StartTime = CalendarList.StartDateTime.TimeOfDay;
                this.CalendarListViewModel.Endtime = CalendarList.EndDateTime.TimeOfDay;
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var CalendarList = _context.CalendarLists.Where(x => x.Id == this.CalendarListViewModel.Id).FirstOrDefault();
            
            if (CalendarList == null)
            {
                return NotFound();
            }

            var StartDateTimeResult = this.CalendarListViewModel.StartDate + this.CalendarListViewModel.StartTime;
            var EndDateTimeResult = this.CalendarListViewModel.EndDate + this.CalendarListViewModel.Endtime;

            CalendarList.StartDateTime = StartDateTimeResult;
            CalendarList.EndDateTime = EndDateTimeResult;
            CalendarList.Title = this.CalendarListViewModel.Title;
            CalendarList.Description = this.CalendarListViewModel.Description;

            _context.CalendarLists.Attach(CalendarList).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var CalendarList = _context.CalendarLists.Where(x => x.Id == this.CalendarListViewModel.Id).FirstOrDefault();

            if (CalendarList == null)
            {
                return NotFound();
            }

            _context.CalendarLists.Remove(CalendarList);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
