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
    public class DepartmentModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public string FormMode { get; set; }

        [BindProperty]
        public int MasterFormId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Cost Center is required.")]
        public string SelectDepartment { get; set; }

        [BindProperty]
        public List<MasterFormDepartment> Departments { get; set; }

        public DepartmentModel(ApplicationDbContext context)
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

            var masterForm = _context.MasterFormLists.Include(x => x.MasterFormDepartments).Where(x => x.Id == this.MasterFormId).FirstOrDefault();

            if (masterForm != null) 
            {
                this.Departments = masterForm.MasterFormDepartments.OrderBy(x => x.Id).ToList();
            }
            else
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddDepartmentAsync()
        {
            if (!String.IsNullOrEmpty(this.SelectDepartment))
            {
                // check if department exists in list
                var getMasterForm = _context.MasterFormLists.Include(x => x.MasterFormDepartments).Where(x => x.Id == this.MasterFormId).FirstOrDefault();

                if (getMasterForm == null)
                {
                    return NotFound();
                }

                var getAllDepartments = getMasterForm.MasterFormDepartments.ToList();
                
                if (getAllDepartments != null && getAllDepartments.Count() > 0)
                {
                    if(getAllDepartments.Select(x => x.DepartmentName).Contains(this.SelectDepartment))
                    {
                        ViewData["Same Department"] = "Found";
                        return Page();
                    }
                }

                var newDepartment = new MasterFormDepartment();
                newDepartment.DepartmentName = this.SelectDepartment;
                newDepartment.MasterFormListId = this.MasterFormId;

                _context.MasterFormDepartments.Add(newDepartment);
                await _context.SaveChangesAsync();

                TempData["RequestFormMode"] = this.FormMode;
                return RedirectToPage(new { Id = this.MasterFormId });          
            }

            return Page();
        }

        public IActionResult OnPostPrevious()
        {
            ModelState.Clear();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("FormApprovalLevel", new { Id = this.MasterFormId });
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

        public async Task<IActionResult> OnPostDeleteDepartmentAsync(int DepartmentId)
        {
            ModelState.Clear();

            var deleteDepartment = _context.MasterFormDepartments.Where(x => x.Id == DepartmentId).FirstOrDefault();
            _context.MasterFormDepartments.Remove(deleteDepartment);
            await _context.SaveChangesAsync();

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("Department", new { Id = this.MasterFormId });
        }

        public IActionResult OnPostNext()
        {
            ModelState.Clear();

            if (Departments.Count() == 0)
            {
                ViewData["Empty Department"] = "Found";

                return Page();
            }

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("CCBApproval", new { Id = this.MasterFormId });
        }
    }
}
