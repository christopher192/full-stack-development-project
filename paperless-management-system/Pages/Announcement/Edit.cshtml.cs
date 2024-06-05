using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;

namespace WD_ERECORD_CORE.Pages.Announcement
{
    public class EditModel : AnnouncementPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public AnnouncementListEditViewModel AnnouncementListViewModel { get; set; } = new AnnouncementListEditViewModel();

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var AnnouncementList = await _context.AnnouncementLists.FirstOrDefaultAsync(m => m.Id == Id);

            if (AnnouncementList == null)
            {
                return NotFound();
            }

            this.AnnouncementListViewModel.Id = AnnouncementList.Id;
            this.AnnouncementListViewModel.Image = AnnouncementList.Image;
            this.AnnouncementListViewModel.Label1 = AnnouncementList.Label1;
            this.AnnouncementListViewModel.Label2 = AnnouncementList.Label2;
            this.AnnouncementListViewModel.Status = AnnouncementList.Status;


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

            if (this.AnnouncementListViewModel.UploadImage != null)
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
                var editnnouncementList = _context.AnnouncementLists.Where(x => x.Id == this.AnnouncementListViewModel.Id).FirstOrDefault();

                if (editnnouncementList == null)
                {
                    return NotFound();
                }

                editnnouncementList.Label1 = this.AnnouncementListViewModel.Label1;
                editnnouncementList.Label2 = this.AnnouncementListViewModel.Label2;
                editnnouncementList.Status = this.AnnouncementListViewModel.Status;

                if (!String.IsNullOrEmpty(this.AnnouncementListViewModel.Image))
                {
                    editnnouncementList.Image = this.AnnouncementListViewModel.Image;
                }

                _context.AnnouncementLists.Update(editnnouncementList);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
