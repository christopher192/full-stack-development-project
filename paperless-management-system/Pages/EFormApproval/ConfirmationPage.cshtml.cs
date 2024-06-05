using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.EFormApproval
{
    public class ConfirmationPageModel : EFormApprovalPageModel
    {
        private readonly ApplicationDbContext _context;

        public ConfirmationPageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FormListApprovalLevel FormListApprovalLevel { get; set; }
        
        public IActionResult OnGet(int? ApprovalLevelId)
        {
            if (ApprovalLevelId == null)
            {
                return NotFound();
            }

            this.FormListApprovalLevel = _context.FormListApprovalLevels.Include(x => x.FormListApprovers).Include(x => x.FormList).Where(x => x.Id == ApprovalLevelId).FirstOrDefault();

            if (this.FormListApprovalLevel == null)
            {
                return NotFound();
            }
            else if (this.FormListApprovalLevel.ApprovalStatus != "approved" && this.FormListApprovalLevel.ApprovalStatus != "rejected") 
            {
                return NotFound();
            }

            return Page();
        }
    }
}
