using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.UserManual
{
    [Authorize(Roles = "System Admin")]
    public class EditModel : UserManualPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        [BindProperty]
        [Display(Name = "Upload File")]
        public IFormFile? UploadFile { get; set; }

        [BindProperty]
        public UserManualList UserManualList { get; set; }

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        private bool ValidateFile(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            string[] allowedFileTypes = { ".pdf", ".mkv", ".xlsx", ".mp4", ".docx" };
            if (allowedFileTypes.Contains(fileExtension))
            {
                return true;
            }
            return false;
        }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            UserManualList = await _context.UserManualLists.FirstOrDefaultAsync(m => m.Id == Id);

            if (UserManualList == null)
            {
                return NotFound();
            }

            string[] status = { "Active", "Disactive" };
            ViewData["SelectionStatus"] = status.Select((r, index) => new SelectListItem { Text = r, Value = r });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                string[] status = { "Active", "Disactive" };
                ViewData["SelectionStatus"] = status.Select((r, index) => new SelectListItem { Text = r, Value = r });

                return Page();
            }

            string contextRootPath = _env.ContentRootPath;

            if (this.UploadFile != null)
            {
                if (ValidateFile(this.UploadFile))
                {
                    string deletePath = Path.Combine(contextRootPath + @"\UserManualFiles\", this.UserManualList.UserManualFilePath);

                    if (System.IO.File.Exists(deletePath))
                    {
                        System.IO.File.Delete(deletePath);
                    }

                    var fileExtension = Path.GetExtension(UploadFile.FileName);

                    if (fileExtension == ".mkv")
                    {
                        fileExtension = ".mp4";
                    }

                    var uniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), fileExtension);
                    var filePath = Path.Combine(contextRootPath + @"\UserManualFiles\", uniqueFileName);
                    this.UserManualList.UserManualFilePath = uniqueFileName;

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await UploadFile.CopyToAsync(stream);
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(this.UploadFile), "Only .pdf, .xlsx, .docx, .mkv, .mp4 file extension is allowed.");

                    string[] status = { "Active", "Disactive" };
                    ViewData["SelectionStatus"] = status.Select((r, index) => new SelectListItem { Text = r, Value = r });

                    return Page();

                }
            }

            if (ModelState.IsValid)
            {
                var User = await GetCurrentUser();

                this.UserManualList.LatestUpdatedDate = DateTime.Now;
                this.UserManualList.LatestUpdatedBy = User.DisplayName;

                _context.Update(this.UserManualList);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
