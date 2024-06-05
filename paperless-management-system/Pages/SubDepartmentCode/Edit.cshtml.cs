using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.SubDepartmentCode
{
    [Authorize(Roles = "System Admin")]
    public class EditModel : SubDepartmentCodePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;

        public EditModel(ApplicationDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("DataProtection");
        }

        [BindProperty]
        public SubDepartmentList SubDepartmentList { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.SubDepartmentList = await _context.SubDepartmentLists
                .Include(s => s.DepartmentList).FirstOrDefaultAsync(m => m.Id == int.Parse(_protector.Unprotect(id)));
            this.SubDepartmentList.DepartmentListInputId = this.SubDepartmentList.DepartmentListId;

            if (SubDepartmentList == null)
            {
                return NotFound();
            }

            ViewData["DepartmentListId"] = new SelectList(_context.DepartmentLists.Select(x => new { x.Id, DepartmentCodeDescription = x.DepartmentCode + " - " + x.DepartmentDescription }), "Id", "DepartmentCodeDescription");
            
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            this.SubDepartmentList.DepartmentListId = this.SubDepartmentList.DepartmentListInputId;
            _context.Attach(SubDepartmentList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubDepartmentListExists(SubDepartmentList.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SubDepartmentListExists(int id)
        {
            return _context.SubDepartmentLists.Any(e => e.Id == id);
        }
    }
}
