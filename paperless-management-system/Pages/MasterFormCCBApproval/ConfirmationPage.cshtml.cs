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

namespace WD_ERECORD_CORE.Pages.MasterFormCCBApproval
{
    public class ConfirmationPageModel : MasterFormCCBApprovalPageModel
    {
        private readonly ApplicationDbContext _context;

        public ConfirmationPageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MasterFormCCBApprovalLevel MasterFormCCBApprovalLevel { get; set; }
        
        public IActionResult OnGet(int? ApprovalLevelId)
        {
            if (ApprovalLevelId == null)
            {
                return NotFound();
            }

            this.MasterFormCCBApprovalLevel = _context.MasterFormCCBApprovalLevels.Include(x => x.MasterFormCCBApprovers).Include(x => x.MasterFormDepartment).ThenInclude(x => x.MasterFormList).Where(x => x.Id == ApprovalLevelId).FirstOrDefault();

            if (this.MasterFormCCBApprovalLevel == null)
            {
                return NotFound();
            }
            else if (this.MasterFormCCBApprovalLevel.ApprovalStatus != "approved" && this.MasterFormCCBApprovalLevel.ApprovalStatus != "rejected") 
            {
                return NotFound();
            }

            return Page();
        }
    }
}
