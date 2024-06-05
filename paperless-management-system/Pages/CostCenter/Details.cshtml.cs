using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.CostCenter
{
    public class DetailsModel : CostCenterPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;

        public DetailsModel(ApplicationDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("DataProtection");
        }

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
    }
}
