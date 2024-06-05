using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.MasterFormCCBApproval
{
    public class CCBApproverPageModel : MasterFormCCBApprovalPageModel
    {
        private readonly ApplicationDbContext _context;

        public CCBApproverPageModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
