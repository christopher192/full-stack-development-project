using System;
using System.Collections.Generic;
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
    public class CCBApprovalModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public string FormMode { get; set; }

        [BindProperty]
        public int MasterFormId { get; set; }

        [BindProperty]
        public List<MasterFormDepartment> Departments { get; set; } = new List<MasterFormDepartment>();

        [BindProperty]
        public List<MasterFormCCBApprovalLevel> CCBApprovalLevels { get; set; } = new List<MasterFormCCBApprovalLevel>();

        [BindProperty]
        public List<MasterFormCCBApprover> CCBApprovers { get; set; } = new List<MasterFormCCBApprover>();

        public CCBApprovalModel(ApplicationDbContext context)
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

            var departmentList = _context.MasterFormLists.Include(x => x.MasterFormDepartments).ThenInclude(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).Where(x => x.Id == Id).FirstOrDefault();

            if (departmentList != null)
            {
                this.Departments = departmentList.MasterFormDepartments.ToList();
                this.CCBApprovalLevels = departmentList.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).OrderBy(x => x.Id).ToList();
                this.CCBApprovers = departmentList.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).SelectMany(x => x.MasterFormCCBApprovers).ToList();
            }
            else 
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPostSet(int DepartmentId)
        {
            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("CCBApprovalLevel", new { DepartmentId = DepartmentId });
        }

        public async Task<IActionResult> OnPostSetAll(int DepartmentId)
        {
            var getDepartmentApprovalLevels = this.CCBApprovalLevels.Where(x => x.MasterFormDepartmentId == DepartmentId).ToList();

            // find empty approvers in all levels
            var foundEmptyApprover = false;

            if (getDepartmentApprovalLevels.Count() > 0) 
            {
                foreach (var approvalLevel in getDepartmentApprovalLevels)
                {
                    var checkEmpty = this.CCBApprovers.Where(x => x.MasterFormCCBApprovalLevelId == approvalLevel.Id).Any();

                    if (checkEmpty == false)
                    {
                        foundEmptyApprover = true;
                    }
                }

                if (foundEmptyApprover)
                {
                    ViewData["Set All"] = "Unable to proceess, empty approver is found!";
                    return Page();
                }
            }
            else
            {
                ViewData["Set All"] = "Unable to proceess, no approval level is found!";
                return Page();
            }

            var getMasterForm = _context.MasterFormLists.Include(x => x.MasterFormDepartments).ThenInclude(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).Where(x => x.Id == this.MasterFormId).AsNoTracking().FirstOrDefault();

            if (getMasterForm != null)
            {
                var getSelectedDepartment = getMasterForm.MasterFormDepartments.Where(x => x.Id == DepartmentId).FirstOrDefault();

                if (getSelectedDepartment != null)
                {
                    if ((getSelectedDepartment.MasterFormCCBApprovalLevels.Count() != 0 && getSelectedDepartment.MasterFormCCBApprovalLevels != null) && (getSelectedDepartment.MasterFormCCBApprovalLevels.SelectMany(x => x.MasterFormCCBApprovers).Count() != 0 && getSelectedDepartment.MasterFormCCBApprovalLevels.SelectMany(x => x.MasterFormCCBApprovers) != null)) 
                    {
                        var getAllApprovalLevels = getSelectedDepartment.MasterFormCCBApprovalLevels.ToList();

                        if (getAllApprovalLevels.Count() > 0)
                        {
                            var otherDepartments = getMasterForm.MasterFormDepartments.Where(x => x.Id != DepartmentId).ToList();

                            if (otherDepartments.Count() > 0)
                            {
                                // remove existing approval levels
                                _context.MasterFormCCBApprovalLevels.RemoveRange(otherDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).ToList());

                                var duplicatedExistingApproval = new List<MasterFormCCBApprovalLevel>();

                                // duplicate selected department approval level to other department
                                foreach (var department in otherDepartments)
                                {
                                    foreach (var approvalLevel in getAllApprovalLevels)
                                    {
                                        var newApprovalLevel = new MasterFormCCBApprovalLevel() { MasterFormDepartmentId = department.Id, EmailReminder = approvalLevel.EmailReminder, ApprovalStatus = "none", ApproveCondition = approvalLevel.ApproveCondition, NotificationType = approvalLevel.NotificationType, MasterFormCCBApprovers = new List<MasterFormCCBApprover>() };

                                        foreach (var existingApprover in approvalLevel.MasterFormCCBApprovers)
                                        {
                                            var NewApprover = new MasterFormCCBApprover() { ApproverEmail = existingApprover.ApproverEmail, ApproverName = existingApprover.ApproverName, ApproverStatus = "none", EmployeeId = existingApprover.EmployeeId };
                                            newApprovalLevel.MasterFormCCBApprovers.Add(NewApprover);
                                        }

                                        duplicatedExistingApproval.Add(newApprovalLevel);
                                    }
                                }

                                _context.MasterFormCCBApprovalLevels.AddRange(duplicatedExistingApproval);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            else 
            {
                return NotFound();
            }

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage(new { Id = this.MasterFormId });
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            ModelState.Clear();

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

        public IActionResult OnPostPrevious()
        {
            ModelState.Clear();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("Department", new { Id = this.MasterFormId });
        }

        public IActionResult OnPostNext()
        {
            ModelState.Clear();

            // find empty approvers in all levels
            var foundEmptyApprover = false;

            /*            foreach (var approvalLevel in this.CCBApprovalLevels)
                        {
                            var checkEmpty = CCBApprovers.Where(x => x.MasterFormCCBApprovalLevelId == approvalLevel.Id).Any();

                            if (checkEmpty == false)
                            {
                                foundEmptyApprover = true;
                            }
                        }*/

            var getMasterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).Include(x => x.MasterFormDepartments).ThenInclude(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).FirstOrDefault();
            var departments = getMasterForm.MasterFormDepartments.ToList();

            if (departments.Count() > 0)
            {
                foreach (var department in departments)
                {
                    if (department.MasterFormCCBApprovalLevels.ToList().Count() > 0)
                    {
                        foreach (var ccbApprovalLevels in department.MasterFormCCBApprovalLevels.ToList())
                        {
                            if (ccbApprovalLevels.MasterFormCCBApprovers.ToList() != null)
                            {
                                if (ccbApprovalLevels.MasterFormCCBApprovers.ToList().Count() == 0)
                                {
                                    foundEmptyApprover = true;
                                    break;
                                }
                            }
                        }
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
                return RedirectToPage("ReviewSubmit", new { Id = this.MasterFormId });
            }
        }
    }
}
