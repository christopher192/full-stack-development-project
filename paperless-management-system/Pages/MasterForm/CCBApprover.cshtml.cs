using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using IdentityApp.Pages.Identity;

namespace WD_ERECORD_CORE.Pages.MasterForm
{
    public class CCBApproverModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public string? FormMode { get; set; }

        [BindProperty]
        public int CCBApprovalLevelId { get; set; }

        [BindProperty]
        public int MasterFormId { get; set; }

        [BindProperty]
        public MasterFormCCBApprovalLevel MasterFormCCBApprovalLevel { get; set; } = new MasterFormCCBApprovalLevel();

        [BindProperty]
        public List<MasterFormCCBApprover> MasterFormCCBApprovers { get; set; } = new List<MasterFormCCBApprover>();

        [BindProperty]
        [Required(ErrorMessage = "Email address is required.")]
        public string? InputEmail { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "User name is required.")]
        public string? InputName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "User id is required.")]
        public string? InputUserId { get; set; }

        public CCBApproverModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? CCBApprovalLevelId, int? MasterFormId)
        {
            if (TempData["RequestFormMode"] as string == "Create" && CCBApprovalLevelId != null && MasterFormId != null)
            {
                this.FormMode = "Create";
                this.CCBApprovalLevelId = CCBApprovalLevelId ?? -1;
                this.MasterFormId = MasterFormId ?? -1;
            }
            else if (TempData["RequestFormMode"] as string == "Edit" && CCBApprovalLevelId != null && MasterFormId != null)
            {
                this.FormMode = "Edit";
                this.CCBApprovalLevelId = CCBApprovalLevelId ?? -1;
                this.MasterFormId = MasterFormId ?? -1;
            }
            else if (TempData["RequestFormMode"] as string == "EditOnly" && CCBApprovalLevelId != null && MasterFormId != null)
            {
                this.FormMode = "EditOnly";
                this.CCBApprovalLevelId = CCBApprovalLevelId ?? -1;
                this.MasterFormId = MasterFormId ?? -1;
            }

            if (String.IsNullOrEmpty(this.FormMode) || this.CCBApprovalLevelId == -1 || this.MasterFormId == -1)
            {
                return NotFound();
            }

            var masterFormCCBApprovalLevel = _context.MasterFormCCBApprovalLevels.Include(x => x.MasterFormCCBApprovers).Where(x => x.Id == CCBApprovalLevelId).FirstOrDefault();

            if (masterFormCCBApprovalLevel != null)
            {
                this.MasterFormCCBApprovalLevel = masterFormCCBApprovalLevel;
                this.MasterFormCCBApprovers = masterFormCCBApprovalLevel.MasterFormCCBApprovers.OrderBy(x => x.Id).ToList();
            }
            else
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPostBack()
        {
            ModelState.Clear();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("CCBApprovalLevel", new { DepartmentId = this.MasterFormCCBApprovalLevel.MasterFormDepartmentId });
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int CCBApproverId)
        {
            ModelState.Clear();

            var deleteCCBApprover = _context.MasterFormCCBApprovers?.Where(x => x.Id == CCBApproverId).FirstOrDefault();
            
            if (deleteCCBApprover != null) {
                _context.MasterFormCCBApprovers?.Remove(deleteCCBApprover);
                await _context.SaveChangesAsync();

                TempData["RequestFormMode"] = this.FormMode;
                return RedirectToPage(new { CCBApprovalLevelId = this.CCBApprovalLevelId, MasterFormId = this.MasterFormId });
            }

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage(new { CCBApprovalLevelId = this.CCBApprovalLevelId, MasterFormId = this.MasterFormId });
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            ModelState.Clear();

            var updateCCBApprovalLevel = _context.MasterFormCCBApprovalLevels.Include(x => x.MasterFormCCBApprovers).Where(x => x.Id == this.CCBApprovalLevelId).FirstOrDefault();
            
            if (updateCCBApprovalLevel != null)
            {
                updateCCBApprovalLevel.ReminderType = this.MasterFormCCBApprovalLevel.ReminderType;
                updateCCBApprovalLevel.EmailReminder = this.MasterFormCCBApprovalLevel.EmailReminder;
                updateCCBApprovalLevel.ApproveCondition = this.MasterFormCCBApprovalLevel.ApproveCondition;
                updateCCBApprovalLevel.NotificationType = this.MasterFormCCBApprovalLevel.NotificationType;

                if (updateCCBApprovalLevel.NotificationType == "By Superior" && updateCCBApprovalLevel.MasterFormCCBApprovers.ToList().Count() == 0)
                {
                    var masterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

                    if (masterForm != null)
                    {
                        var getUserManagerId = _context.ApplicationUsers.Where(x => x.UserName == masterForm.Owner).AsNoTracking().FirstOrDefault();
                        if (getUserManagerId != null)
                        {
                            var userManagerId = getUserManagerId.ManagerId;
                            var getManagerInfo = _context.ApplicationUsers.Where(x => x.UserName == userManagerId).AsNoTracking().FirstOrDefault();

                            if (getManagerInfo != null)
                            {
                                var newApprover = new MasterFormCCBApprover() { ApproverName = getManagerInfo.DisplayName, ApproverEmail = getManagerInfo.Email, EmployeeId = getManagerInfo.UserName };
                                updateCCBApprovalLevel.MasterFormCCBApprovers.Add(newApprover);
                            }
                        }
                    }
                }

                _context.MasterFormCCBApprovalLevels.Update(updateCCBApprovalLevel);
                await _context.SaveChangesAsync();

                TempData["RequestFormMode"] = this.FormMode;
                return RedirectToPage("CCBApprovalLevel", new { DepartmentId = this.MasterFormCCBApprovalLevel.MasterFormDepartmentId });
            }

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("CCBApprovalLevel", new { DepartmentId = this.MasterFormCCBApprovalLevel.MasterFormDepartmentId });
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!String.IsNullOrEmpty(this.InputEmail) && !String.IsNullOrEmpty(this.InputName))
            {
                // find duplicate user
                var getDepartment = _context.MasterFormDepartments.Where(x => x.Id == this.MasterFormCCBApprovalLevel.MasterFormDepartmentId).Include(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).AsNoTracking().FirstOrDefault();
                var foundDuplicatedUserAtOtherLevel = false;

                if (getDepartment != null)
                {
                    if ((getDepartment.MasterFormCCBApprovalLevels != null && getDepartment.MasterFormCCBApprovalLevels.Count() > 0) && (getDepartment.MasterFormCCBApprovalLevels.SelectMany(x => x.MasterFormCCBApprovers) != null && getDepartment.MasterFormCCBApprovalLevels.SelectMany(x => x.MasterFormCCBApprovers).Count() > 0))
                    {
                        foundDuplicatedUserAtOtherLevel = getDepartment.MasterFormCCBApprovalLevels.SelectMany(x => x.MasterFormCCBApprovers).Any(x => x.ApproverName == this.InputName && x.ApproverEmail == this.InputEmail && x.EmployeeId == this.InputUserId);
                    }
                }

                if (foundDuplicatedUserAtOtherLevel)
                {
                    ModelState.AddModelError("InputEmail", "Found same approver, kindly choose different user.");

                    return Page();
                }

                var newCCBApprover = new MasterFormCCBApprover();
                newCCBApprover.ApproverEmail = this.InputEmail;
                newCCBApprover.ApproverName = this.InputName;
                newCCBApprover.EmployeeId = this.InputUserId;
                newCCBApprover.MasterFormCCBApprovalLevelId = this.CCBApprovalLevelId;

                _context.MasterFormCCBApprovers.Add(newCCBApprover);
                await _context.SaveChangesAsync();
                
            }

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage(new { CCBApprovalLevelId = this.CCBApprovalLevelId, MasterFormId = this.MasterFormId });
        }
    }
}
