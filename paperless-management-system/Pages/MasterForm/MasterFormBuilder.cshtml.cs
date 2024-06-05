using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;

namespace WD_ERECORD_CORE.Pages.MasterForm
{
    public class MasterFormBuilderModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public string FormMode { get; set; }

        [BindProperty]
        public FormDesignViewModel formDesignViewModel { get; set; } = new FormDesignViewModel();

        public MasterFormBuilderModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
/*            if (TempData["RequestFormMode"] as string == "Create" && Id != null)
            {
                this.FormMode = "Create";
            }
            else if (TempData["RequestFormMode"] as string == "Edit" && Id != null)
            {
                this.FormMode = "Edit";
            }
            else
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(this.FormMode))
            {
                return NotFound();
            }*/

            this.FormMode = "Create";

            var GetMasterForm = _context.MasterFormLists.Where(x => x.Id == Id).FirstOrDefault();

            if (GetMasterForm != null)
            {
                var User = await GetCurrentUser();

                if (GetMasterForm.CurrentEditor != null && GetMasterForm.CurrentEditor != User.UserName)
                {
                    return NotFound();
                }

                this.formDesignViewModel.MasterFormId = GetMasterForm.Id;
                this.formDesignViewModel.MasterFormDesignData = GetMasterForm.MasterFormData;
            }
            else
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(this.FormMode)) 
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostPreviousAsync()
        {
            var UpdateMasterForm = _context.MasterFormLists.Where(x => x.Id == this.formDesignViewModel.MasterFormId).FirstOrDefault();

            if (UpdateMasterForm != null && this.formDesignViewModel.MasterFormDesignData != null)
            {
                UpdateMasterForm.MasterFormData = this.formDesignViewModel.MasterFormDesignData;

                _context.MasterFormLists.Update(UpdateMasterForm);
                await _context.SaveChangesAsync();
            }

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("NumbericBuilder", new { RequestFormMode = "Previous", Id = this.formDesignViewModel.MasterFormId });
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (this.FormMode == "Create" || this.FormMode == "Edit")
            {
                var UpdateMasterForm = _context.MasterFormLists.Where(x => x.Id == this.formDesignViewModel.MasterFormId).FirstOrDefault();

                if (UpdateMasterForm != null)
                {
                    if (this.formDesignViewModel.MasterFormDesignData != null)
                    {
                        UpdateMasterForm.MasterFormData = this.formDesignViewModel.MasterFormDesignData;
                    }

                    if (UpdateMasterForm.CurrentEditor != null)
                    {
                        UpdateMasterForm.CurrentEditor = null;
                    }

                    _context.MasterFormLists.Update(UpdateMasterForm);
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Save"] = "Close Window";
            return Page();
        }

        public async Task<IActionResult> OnPostNextAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (this.formDesignViewModel.MasterFormDesignData == String.Format("{{{0}}}", "\"display\":\"form\",\"components\":[]"))
            {
                ViewData["Empty Form Data"] = "Found";
                return Page();
            }

            var UpdateMasterForm = _context.MasterFormLists.Where(x => x.Id == this.formDesignViewModel.MasterFormId).FirstOrDefault();

            if (UpdateMasterForm != null && this.formDesignViewModel.MasterFormDesignData != null)
            {
                UpdateMasterForm.MasterFormData = this.formDesignViewModel.MasterFormDesignData;

                _context.MasterFormLists.Update(UpdateMasterForm);
                await _context.SaveChangesAsync();
            }

            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("FormApprovalLevel", new { Id = this.formDesignViewModel.MasterFormId });
        }
    }
}
