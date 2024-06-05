using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.MasterFormHistory
{
    public class MasterFormViewModel : MasterFormHistoryPageModel
    {
        private readonly ApplicationDbContext _context;

        public MasterFormViewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MasterFormListHistory MasterFormHistory { get; set; }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            this.MasterFormHistory = await _context.MasterFormListHistories.FirstOrDefaultAsync(m => m.Id == Id);

            if (this.MasterFormHistory == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
