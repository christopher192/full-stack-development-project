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

namespace WD_ERECORD_CORE.Pages.FactoryCode
{
    [Authorize(Roles = "System Admin")]
    public class EditModel : FactoryCodePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;

        public EditModel(ApplicationDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("DataProtection");
        }

        [BindProperty]
        public FactoryList FactoryList { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FactoryList = await _context.FactoryLists.FirstOrDefaultAsync(m => m.Id == int.Parse(_protector.Unprotect(id)));

            if (FactoryList == null)
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
                return Page();
            }

            _context.Attach(FactoryList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FactoryListExists(FactoryList.Id))
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

        private bool FactoryListExists(int id)
        {
            return _context.FactoryLists.Any(e => e.Id == id);
        }
    }
}
