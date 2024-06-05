using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.CostCenter
{
    [Authorize(Roles = "System Admin")]
    public class DeleteModel : CostCenterPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;

        public DeleteModel(ApplicationDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("DataProtection");
        }

        [BindProperty]
        public CostCenterList CostCenterList { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CostCenterList = await _context.CostCenterLists.FirstOrDefaultAsync(m => m.Id == int.Parse(_protector.Unprotect(id)));

            if (CostCenterList == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (CostCenterList.Id == null)
            {
                return NotFound();
            }

            CostCenterList = await _context.CostCenterLists.FindAsync(CostCenterList.Id);

            if (CostCenterList != null)
            {
                _context.CostCenterLists.Remove(CostCenterList);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
