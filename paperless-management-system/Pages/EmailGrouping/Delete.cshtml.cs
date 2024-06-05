using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.EmailGrouping
{
    public class DeleteModel : EmailGroupingPageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EmailGroupList EmailGroupList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            EmailGroupList = await _context.EmailGroupLists.Include(x => x.EmailGroupUserLists).FirstOrDefaultAsync(m => m.Id == Id);

            if (EmailGroupList == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            EmailGroupList = await _context.EmailGroupLists.FindAsync(Id);

            if (EmailGroupList != null)
            {
                _context.EmailGroupLists.Remove(EmailGroupList);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
