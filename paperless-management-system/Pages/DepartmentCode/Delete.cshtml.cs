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

namespace WD_ERECORD_CORE.Pages.DepartmentCode
{
    [Authorize(Roles = "System Admin")]
    public class DeleteModel : DepartmentCodePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;

        public DeleteModel(ApplicationDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("DataProtection");
        }

        [BindProperty]
        public DepartmentList DepartmentList { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DepartmentList = await _context.DepartmentLists.FirstOrDefaultAsync(m => m.Id == int.Parse(_protector.Unprotect(id)));

            if (DepartmentList == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (DepartmentList.Id == null)
            {
                return NotFound();
            }

            DepartmentList = await _context.DepartmentLists.FindAsync(DepartmentList.Id);

            if (DepartmentList != null)
            {
                _context.DepartmentLists.Remove(DepartmentList);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
