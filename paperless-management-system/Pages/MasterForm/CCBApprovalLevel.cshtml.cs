using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Pages.Identity;

namespace WD_ERECORD_CORE.Pages.MasterForm
{
    public class CCBApprovalLevelModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;
        
        [BindProperty]
        public string FormMode { get; set; }

        [BindProperty]
        public int DepartmentId { get; set; }

        [BindProperty]
        public int MasterFormId { get; set; }

        [BindProperty]
        public List<MasterFormCCBApprovalLevel> MasterFormCCBApprovalLevels { get; set; } = new List<MasterFormCCBApprovalLevel>();

        [BindProperty]
        public List<MasterFormCCBApprover> MasterFormCCBApprovers { get; set; } = new List<MasterFormCCBApprover>();

        public CCBApprovalLevelModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? DepartmentId)
        {
            if (TempData["RequestFormMode"] as string == "Create" && DepartmentId != null)
            {
                this.FormMode = "Create";
                this.MasterFormId = DepartmentId ?? -1;
            }
            else if (TempData["RequestFormMode"] as string == "Edit" && DepartmentId != null)
            {
                this.FormMode = "Edit";
                this.MasterFormId = DepartmentId ?? -1;
            }
            else if (TempData["RequestFormMode"] as string == "EditOnly" && DepartmentId != null)
            {
                this.FormMode = "EditOnly";
                this.MasterFormId = DepartmentId ?? -1;
            }

            if (String.IsNullOrEmpty(this.FormMode) || this.MasterFormId == -1)
            {
                return NotFound();
            }

            var department = _context.MasterFormDepartments.Include(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).Where(x => x.Id == DepartmentId).FirstOrDefault();

            if (department != null)
            {
                this.MasterFormCCBApprovalLevels = department.MasterFormCCBApprovalLevels.OrderBy(x => x.Id).ToList();
                this.MasterFormCCBApprovers = department.MasterFormCCBApprovalLevels.SelectMany(x => x.MasterFormCCBApprovers).ToList();
                this.MasterFormId = department.MasterFormListId ?? -1;
            }
            else
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateApprovalLevelAsync()
        {
            var newMasterFormCCBApprovalLevel = new MasterFormCCBApprovalLevel();

            var addCCBApprovalLevel = _context.MasterFormDepartments.Include(x => x.MasterFormCCBApprovalLevels).Where(x => x.Id == this.DepartmentId).FirstOrDefault();
            addCCBApprovalLevel.MasterFormCCBApprovalLevels.Add(newMasterFormCCBApprovalLevel);

            await _context.SaveChangesAsync();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("CCBApprover", new { CCBApprovalLevelId = newMasterFormCCBApprovalLevel.Id, MasterFormId = this.MasterFormId });
        }

        public IActionResult OnPostEditCCBApprovalLevel(int CCBApprovalLevelId)
        {
            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("CCBApprover", new { CCBApprovalLevelId = CCBApprovalLevelId, MasterFormId = this.MasterFormId });
        }

        public async Task<IActionResult> OnPostDeleteCCBApprovalLevelAsync(int CCBApprovalLevelId)
        {
            var deleteCCBApprovalLevel = _context.MasterFormCCBApprovalLevels.Where(x => x.Id == CCBApprovalLevelId).FirstOrDefault();
            _context.MasterFormCCBApprovalLevels.Remove(deleteCCBApprovalLevel);
            await _context.SaveChangesAsync();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("CCBApprovalLevel", new { DepartmentId = this.DepartmentId });
        }

        public IActionResult OnPostSubmit()
        {
            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("CCBApproval", new { Id = this.MasterFormId });
        }
    }
}
