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

namespace WD_ERECORD_CORE.Pages.CostCenter
{
    [Authorize(Roles = "System Admin")]
    public class EditModel : CostCenterPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;

        public EditModel(ApplicationDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("DataProtection");
        }

        [BindProperty]
        public CostCenterList CostCenterList { get; set; }

        public class JobClassificationTrack
        {
            public string? Name { get; set; }
            public string? Value { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            var jobClassificationTrack = new List<JobClassificationTrack> {
                new JobClassificationTrack { Name = "Select Job Classification/ Track", Value = null },
                new JobClassificationTrack { Name = "IDL", Value = "IDL" },
                new JobClassificationTrack { Name = "DL", Value = "DL" }
            };

            ViewData["jobClassificationTrack"] = new SelectList(jobClassificationTrack, "Value", "Name", null);

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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var jobClassificationTrack = new List<JobClassificationTrack> {
                    new JobClassificationTrack { Name = "Select Job Classification/ Track", Value = null },
                    new JobClassificationTrack { Name = "IDL", Value = "IDL" },
                    new JobClassificationTrack { Name = "DL", Value = "DL" }
                };

                ViewData["jobClassificationTrack"] = new SelectList(jobClassificationTrack, "Value", "Name", null);

                return Page();
            }

            _context.Attach(CostCenterList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CostCenterListExists(CostCenterList.Id))
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

        private bool CostCenterListExists(int id)
        {
            return _context.CostCenterLists.Any(e => e.Id == id);
        }
    }
}
