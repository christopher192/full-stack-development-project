using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.MasterForm
{
    public class FormApprovalLevelModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public string FormMode { get; set; }

        [BindProperty]
        public int MasterFormId { get; set; }

        [BindProperty]
        public int ApprovaLevelId { get; set; }

        [BindProperty]
        public FormApprovalLevel FormApprovalLevel { get; set; }

        [BindProperty]
        public List<FormApprover> FormApprovers { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email address is required.")]
        public string InputEmail { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "User name is required.")]
        public string InputName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "User id is required.")]
        public string? InputUserId { get; set; }

        public FormApprovalLevelModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? Id, int? ApprovalLevelId)
        {
            if (TempData["RequestFormMode"] as string == "Create" && Id != null)
            {
                this.FormMode = "Create";
                this.MasterFormId = Id ?? -1;
                this.ApprovaLevelId = ApprovalLevelId ?? -1;
            }
            else if (TempData["RequestFormMode"] as string == "Edit" && Id != null)
            {
                this.FormMode = "Edit";
                this.MasterFormId = Id ?? -1;
                this.ApprovaLevelId = ApprovalLevelId ?? -1;
            }
            else if (TempData["RequestFormMode"] as string == "EditOnly" && Id != null)
            {
                this.FormMode = "EditOnly";
                this.MasterFormId = Id ?? -1;
                this.ApprovaLevelId = ApprovalLevelId ?? -1;
            }

            if (String.IsNullOrEmpty(this.FormMode) || this.MasterFormId == -1 || this.ApprovaLevelId == -1)
            {
                return NotFound();
            }

            var MasterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

            if (MasterForm != null)
            {
                var formApproval = JsonConvert.DeserializeObject<FormApproval>(MasterForm.FormApprovalJSON);
                this.FormApprovalLevel = formApproval.EditableFormApproval.Where(x => x.Id == this.ApprovaLevelId).FirstOrDefault();
                this.FormApprovers = this.FormApprovalLevel.FormApprovers.ToList();
            }
            else 
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!String.IsNullOrEmpty(this.InputEmail) && !String.IsNullOrEmpty(this.InputName) && !String.IsNullOrEmpty(this.InputUserId))
            {
                // find duplicate user at all levels
                var masterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

                if (masterForm != null)
                {
                    FormApproval DesFormApproval = JsonConvert.DeserializeObject<FormApproval>(masterForm.FormApprovalJSON);
                    var approvalLevelsList = DesFormApproval.EditableFormApproval.SelectMany(x => x.FormApprovers).ToList();

                    if (approvalLevelsList.Count() > 0)
                    {
                        // find duplicate user
                        var foundDuplicatedUserAtOtherLevels = approvalLevelsList.Any(x => x.ApproverName == this.InputName && x.ApproverEmail == this.InputEmail && x.EmployeeId == this.InputUserId);

                        if (foundDuplicatedUserAtOtherLevels)
                        {
                            ModelState.AddModelError("InputEmail", "Found same approver, kindly choose different user.");
                            return Page();
                        }
                    }

                    var masterFormApprover = new FormApprover();
                    var approverId = 1;

                    if (FormApprovers.Count > 0)
                    {
                        approverId = FormApprovers.Max(x => x.Id) + 1;
                        masterFormApprover.Id = approverId;
                        masterFormApprover.ApproverName = this.InputName;
                        masterFormApprover.ApproverEmail = this.InputEmail;
                        masterFormApprover.EmployeeId = this.InputUserId;
                        masterFormApprover.FormApprovalLevelId = this.ApprovaLevelId;

                        var MasterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

                        var FormApproval = JsonConvert.DeserializeObject<FormApproval>(MasterForm.FormApprovalJSON);
                        FormApproval.EditableFormApproval.Where(x => x.Id == this.ApprovaLevelId).FirstOrDefault().FormApprovers.Add(masterFormApprover);

                        MasterForm.FormApprovalJSON = JsonConvert.SerializeObject(FormApproval);
                        _context.Attach(MasterForm);
                        _context.Entry(MasterForm).Property(x => x.FormApprovalJSON).IsModified = true;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        masterFormApprover.Id = approverId;
                        masterFormApprover.ApproverName = this.InputName;
                        masterFormApprover.ApproverEmail = this.InputEmail;
                        masterFormApprover.EmployeeId = this.InputUserId;
                        masterFormApprover.FormApprovalLevelId = this.ApprovaLevelId;

                        var MasterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

                        var FormApproval = JsonConvert.DeserializeObject<FormApproval>(MasterForm.FormApprovalJSON);
                        FormApproval.EditableFormApproval.Where(x => x.Id == this.ApprovaLevelId).FirstOrDefault().FormApprovers.Add(masterFormApprover);

                        MasterForm.FormApprovalJSON = JsonConvert.SerializeObject(FormApproval);
                        _context.Attach(MasterForm);
                        _context.Entry(MasterForm).Property(x => x.FormApprovalJSON).IsModified = true;
                        await _context.SaveChangesAsync();
                    }

                    TempData["RequestFormMode"] = this.FormMode;
                    return RedirectToPage(new { Id = this.MasterFormId, ApprovalLevelId = this.ApprovaLevelId });
                }
            }
            

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage(new { Id = this.MasterFormId, ApprovalLevelId = this.ApprovaLevelId });
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int Id)
        {
            ModelState.Clear();

            var masterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

            var formApproval = JsonConvert.DeserializeObject<FormApproval>(masterForm.FormApprovalJSON);

            var approvalLevel = formApproval.EditableFormApproval.Where(x => x.Id == this.ApprovaLevelId).FirstOrDefault();

            var approversList = approvalLevel.FormApprovers.Where(x => x.Id != Id).ToList();

            approvalLevel.FormApprovers = approversList;
            formApproval.EditableFormApproval.Where(x => x.Id == this.ApprovaLevelId).Select(x => x = approvalLevel);

            masterForm.FormApprovalJSON = JsonConvert.SerializeObject(formApproval);
            _context.Attach(masterForm);
            _context.Entry(masterForm).Property(x => x.FormApprovalJSON).IsModified = true;
            await _context.SaveChangesAsync();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage(new { Id = this.MasterFormId, ApprovalLevelId = this.ApprovaLevelId });
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            ModelState.Clear();

            var masterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

            var formApproval = JsonConvert.DeserializeObject<FormApproval>(masterForm.FormApprovalJSON);

            var approvalLevel = formApproval.EditableFormApproval.Where(x => x.Id == this.ApprovaLevelId).FirstOrDefault();
            approvalLevel.NotificationType = this.FormApprovalLevel.NotificationType;
            approvalLevel.ReminderType = this.FormApprovalLevel.ReminderType;
            approvalLevel.EmailReminder = this.FormApprovalLevel.EmailReminder;
            approvalLevel.ApproveCondition = this.FormApprovalLevel.ApproveCondition;

/*            if (approvalLevel.NotificationType == "By Superior" && approvalLevel.FormApprovers.Count() == 0)
            {
                var getUserManagerId = _context.ApplicationUsers.Where(x => x.UserName == masterForm.Owner).AsNoTracking().FirstOrDefault();
                if (getUserManagerId != null)
                {
                    var userManagerId = getUserManagerId.ManagerId;
                    var getManagerInfo = _context.ApplicationUsers.Where(x => x.UserName == userManagerId).AsNoTracking().FirstOrDefault();

                    if (getManagerInfo != null)
                    {
                        var newApprover = new FormApprover() { Id = 1, ApproverName = getManagerInfo.DisplayName, ApproverEmail = getManagerInfo.Email, EmployeeId = getManagerInfo.UserName, FormApprovalLevelId = approvalLevel.Id };
                        approvalLevel.FormApprovers.Add(newApprover);
                    }
                }
            }*/

            formApproval.EditableFormApproval.Where(x => x.Id == this.ApprovaLevelId).Select(x => x = approvalLevel);

            masterForm.FormApprovalJSON = JsonConvert.SerializeObject(formApproval);
            _context.Attach(masterForm);
            _context.Entry(masterForm).Property(x => x.FormApprovalJSON).IsModified = true;
            await _context.SaveChangesAsync();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("FormApprovalLevel", new { Id = this.MasterFormId });
        }

        public IActionResult OnPostBack()
        {
            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("FormApprovalLevel", new { Id = this.MasterFormId });
        }
    }
}
