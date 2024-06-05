using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;

namespace WD_ERECORD_CORE.Pages.MasterForm
{
    public class FormApprovalModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public string FormMode { get; set; }

        [BindProperty]
        public int MasterFormId { get; set; }

        [BindProperty]
        public List<FormApprovalLevel> formApprovalLevel { get; set; } = new List<FormApprovalLevel>();

        [BindProperty]
        public List<FormApprover> formApprovers { get; set; } = new List<FormApprover>();

        public FormApprovalModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? Id)
        {
            if (TempData["RequestFormMode"] as string == "Create" && Id != null)
            {
                this.FormMode = "Create";
                this.MasterFormId = Id ?? -1;
            }
            else if (TempData["RequestFormMode"] as string == "Edit" && Id != null)
            {
                this.FormMode = "Edit";
                this.MasterFormId = Id ?? -1;
            }
            else if (TempData["RequestFormMode"] as string == "EditOnly" && Id != null)
            {
                this.FormMode = "EditOnly";
                this.MasterFormId = Id ?? -1;
            }

            if (String.IsNullOrEmpty(this.FormMode) || this.MasterFormId == -1) 
            {
                return NotFound();
            }

            var FormApproval = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

            // deserialize the object
            FormApproval DesFormApproval = JsonConvert.DeserializeObject<FormApproval>(FormApproval.FormApprovalJSON);
            this.formApprovalLevel = DesFormApproval.EditableFormApproval;
            this.formApprovers = formApprovalLevel.SelectMany(x => x.FormApprovers).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostCreateApprovalLevelAsync()
        {
            // deserialize
            var masterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

            // create new appproval level
            var masterFormApprovalLevel = new FormApprovalLevel();

            FormApproval desFormApproval = JsonConvert.DeserializeObject<FormApproval>(masterForm.FormApprovalJSON);
            var formApproval = desFormApproval.EditableFormApproval;

            // initialize id
            // check total form approvers
            var totalApprovalLevel = formApproval.Count();

            int approvalLevelId = 1;

            if (totalApprovalLevel > 0)
            {
                approvalLevelId = formApproval.Max(x => x.Id) + 1;

                masterFormApprovalLevel.Id = approvalLevelId;
                formApproval.Add(masterFormApprovalLevel);
            }
            else 
            {
                masterFormApprovalLevel.Id = approvalLevelId;
                formApproval.Add(masterFormApprovalLevel);
            }

            desFormApproval.EditableFormApproval = formApproval;
            masterForm.FormApprovalJSON = JsonConvert.SerializeObject(desFormApproval);

            _context.Attach(masterForm);
            _context.Entry(masterForm).Property(x => x.FormApprovalJSON).IsModified = true;
            await _context.SaveChangesAsync();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("FormApprovalApprover", new { Id = this.MasterFormId, ApprovalLevelId = approvalLevelId });
        }

        public IActionResult OnPostPrevious() 
        {
            if (this.FormMode == "EditOnly")
            {
                TempData["RequestFormMode"] = this.FormMode;
                return RedirectToPage("NumbericBuilder", new { RequestFormMode = "Previous", Id = this.MasterFormId });
            }
            else 
            {
                TempData["RequestFormMode"] = this.FormMode;
                return RedirectToPage("MasterFormBuilder", new { Id = this.MasterFormId });
            }
        }

        public IActionResult OnPostEditFormApprovalLevel(int ApprovalLevelId)
        {
            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("FormApprovalApprover", new { Id = this.MasterFormId, ApprovalLevelId = ApprovalLevelId });
        }

        public async Task<IActionResult> OnPostDeleteFormApprovalLevelAsync(int ApprovalLevelId)
        {
            var masterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

            FormApproval DesFormApproval = JsonConvert.DeserializeObject<FormApproval>(masterForm.FormApprovalJSON);
            var approvalLevelsList = DesFormApproval.EditableFormApproval;
            var deleteApprovaLevels = approvalLevelsList.Where(x => x.Id != ApprovalLevelId);

            DesFormApproval.EditableFormApproval = deleteApprovaLevels.ToList();
            masterForm.FormApprovalJSON = JsonConvert.SerializeObject(DesFormApproval);

            _context.Attach(masterForm);
            _context.Entry(masterForm).Property(x => x.FormApprovalJSON).IsModified = true;
            await _context.SaveChangesAsync();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("FormApprovalLevel", new { Id = this.MasterFormId });
        }

        public IActionResult OnPostNext()
        {
            // find empty approvers in all levels
            var foundEmptyApprover = false;

/*            foreach (var approvalLevel in this.formApprovalLevel)
            {
                var checkEmpty = formApprovers.Where(x => x.FormApprovalLevelId == approvalLevel.Id).Any();

                if (checkEmpty == false)
                {
                    foundEmptyApprover = true;
                }
            }*/

            var masterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

            FormApproval DesFormApproval = JsonConvert.DeserializeObject<FormApproval>(masterForm.FormApprovalJSON);
            var approvalLevelsList = DesFormApproval.EditableFormApproval.ToList();

            if (approvalLevelsList.Count() > 0) 
            {
                foreach (var approvalLevel in approvalLevelsList.Where(x => x.NotificationType == "By Name/ Employee"))
                {
                    if (approvalLevel.FormApprovers.ToList().Count() == 0)
                    {
                        foundEmptyApprover = true;
                        break;
                    }
                }
            }

            if (foundEmptyApprover)
            {
                ViewData["Empty Approver"] = "Found";
                return Page();
            }
            else 
            {
                TempData["RequestFormMode"] = this.FormMode;
                return RedirectToPage("Department", new { Id = this.MasterFormId });
            }
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (this.FormMode == "Create" || (this.FormMode == "Edit"))
            {
                var disableCurrentEditor = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).FirstOrDefault();

                if (disableCurrentEditor.CurrentEditor != null)
                {
                    disableCurrentEditor.CurrentEditor = null;
                    
                    _context.Attach(disableCurrentEditor);
                    _context.Entry(disableCurrentEditor).Property(x => x.CurrentEditor).IsModified = true;

                    await _context.SaveChangesAsync();
                }
            }

            TempData["Save"] = "Close Window";
            return Page();
        }
    }
}
