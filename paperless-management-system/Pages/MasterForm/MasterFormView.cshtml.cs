using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.MasterForm
{
    public class MasterFormViewModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;

        public MasterFormViewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public MasterFormList MasterFormList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            MasterFormList = await _context.MasterFormLists.FirstOrDefaultAsync(m => m.Id == Id);

            if (MasterFormList == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
