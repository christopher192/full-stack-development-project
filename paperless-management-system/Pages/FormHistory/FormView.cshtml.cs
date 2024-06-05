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

namespace WD_ERECORD_CORE.Pages.FormHistory
{
    public class FormViewModel : FormHistoryPageModel
    {
        private readonly ApplicationDbContext _context;

        public FormViewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FormListHistory FormListHistory { get; set; }

        public IActionResult OnGetAsync(int? FormHistoryId)
        {
            if (FormHistoryId == null)
            {
                return NotFound();
            }

            this.FormListHistory = _context.FormListHistories.Where(m => m.Id == FormHistoryId).FirstOrDefault();

            if (this.FormListHistory == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
