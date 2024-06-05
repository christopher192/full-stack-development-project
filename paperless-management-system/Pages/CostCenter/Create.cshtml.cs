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

namespace WD_ERECORD_CORE.Pages.CostCenter
{
    [Authorize(Roles = "System Admin")]
    public class CreateModel : CostCenterPageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class JobClassificationTrack
        {
            public string? Name { get; set; }
            public string? Value { get; set; }
        }

        public IActionResult OnGet()
        {
            var jobClassificationTrack = new List<JobClassificationTrack> {
                new JobClassificationTrack { Name = "Select Job Classification/ Track", Value = null },
                new JobClassificationTrack { Name = "IDL", Value = "IDL" },
                new JobClassificationTrack { Name = "DL", Value = "DL" }
            };

            ViewData["jobClassificationTrack"] = new SelectList(jobClassificationTrack, "Value", "Name", null);

            return Page();
        }

        [BindProperty]
        public CostCenterList CostCenterList { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
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

            _context.CostCenterLists.Add(CostCenterList);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
