using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.SubDepartmentCode
{
    [Authorize(Roles = "System Admin")]
    public class CreateModel : SubDepartmentCodePageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["DepartmentListId"] = new SelectList(_context.DepartmentLists.Select(x => new { x.Id, DepartmentCodeDescription = x.DepartmentCode + " - " + x.DepartmentDescription }), "Id", "DepartmentCodeDescription");
           
            return Page();
        }

        [BindProperty]
        public SubDepartmentList SubDepartmentList { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["DepartmentListId"] = new SelectList(_context.DepartmentLists.Select(x => new { x.Id, DepartmentCodeDescription = x.DepartmentCode + " - " + x.DepartmentDescription }), "Id", "DepartmentCodeDescription");

                return Page();
            }

            this.SubDepartmentList.DepartmentListId = this.SubDepartmentList.DepartmentListInputId;
            _context.SubDepartmentLists.Add(this.SubDepartmentList);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
