using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Server.IIS.Core;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;

namespace WD_ERECORD_CORE.Pages.Announcement
{
    public class CreateModel : AnnouncementPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public AnnouncementListViewModel AnnouncementListViewModel { get; set; } = new AnnouncementListViewModel();

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            string[] status = { "Active", "Disactive" };
            ViewData["SelectionStatus"] = status.Select((r, index) => new SelectListItem { Text = r, Value = r });

            return Page();
        }

        private bool ValidateFile(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            string[] allowedFileTypes = { ".gif", ".png", ".jpeg", ".jpg" };
            if (allowedFileTypes.Contains(fileExtension))
            {
                return true;
            }
            return false;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string[] status2 = { "Active", "Disactive" };
            ViewData["SelectionStatus"] = status2.Select((r, index) => new SelectListItem { Text = r, Value = r });

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (this.AnnouncementListViewModel != null)
            {
                if (ValidateFile(this.AnnouncementListViewModel.UploadImage))
                {
                    using (var ms = new MemoryStream())
                    {
                        this.AnnouncementListViewModel.UploadImage.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        this.AnnouncementListViewModel.Image = s;
                    }
                }
                else
                {
                    ModelState.AddModelError("AnnouncementListViewModel.UploadImage", "Only .gif, .png, .jpeg, .jpg file extension is allowed.");
                    
                    return Page();
                }
            }

            if (ModelState.IsValid)
            {
                var newAnnouncementList = new AnnouncementList();
                newAnnouncementList.Label1 = this.AnnouncementListViewModel.Label1;
                newAnnouncementList.Label2 = this.AnnouncementListViewModel.Label2;
                newAnnouncementList.Status = this.AnnouncementListViewModel.Status;
                newAnnouncementList.Image = this.AnnouncementListViewModel.Image;
                newAnnouncementList.UploadDate = DateTime.Now;

                _context.AnnouncementLists.Add(newAnnouncementList);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
