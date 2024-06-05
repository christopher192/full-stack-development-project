using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.EmailGrouping
{
    public class EditModel : EmailGroupingPageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EmailGroupList EmailGroupList { get; set; }

        [BindProperty]
        public List<EmailGroupUserList> EmailGroupUserLists { get; set; } = new List<EmailGroupUserList>();

        [BindProperty]
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address/ Distribution List is required")]
        public string InputEmail { get; set; }

        [BindProperty]
        [Display(Name = "Distribution List")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email Address/ Distribution List is required")]
        public string DistributionList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.EmailGroupList = await _context.EmailGroupLists.Include(x => x.EmailGroupUserLists).FirstOrDefaultAsync(m => m.Id == id);
            this.EmailGroupUserLists = this.EmailGroupList.EmailGroupUserLists.ToList();

            if (this.EmailGroupList == null)
            {
                return NotFound();
            }

            return Page();
        }

        public JsonResult OnGetUserList(string searchBy)
        {
            var selectedData = _context.ApplicationUsers.Select(x => new
            {
                id = x.Id,
                value = x.DisplayName + " - " + x.Email,
                text = x.DisplayName + " - " + x.Email,
                email = x.Email,
                displayname = x.DisplayName
            });

            if (!String.IsNullOrEmpty(searchBy))
            {
                selectedData = selectedData.Where(x => x.email.ToLower().Contains(searchBy.ToLower()) || (x.displayname != null && x.displayname.ToLower().Contains(searchBy.ToLower())));
            }

            return new JsonResult(selectedData);
        }

        public async Task<IActionResult> OnPostUpdateGroupNameAsync()
        {
            ModelState.Remove("InputEmail");
            ModelState.Remove("DistributionList");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(EmailGroupList).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            ModelState.Clear();

            return RedirectToPage(new { Id = this.EmailGroupList.Id});
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int Id)
        {
            var deleteUser = _context.EmailGroupUserLists.Where(x => x.Id == Id).FirstOrDefault();

            if (deleteUser != null)
            {
                _context.EmailGroupUserLists.Remove(deleteUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { Id = this.EmailGroupList.Id });
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            var emailAddress = "";
            var userName = "";

            if (!String.IsNullOrEmpty(this.InputEmail))
            {
                ModelState.Remove("DistributionList");
                var user = _context.ApplicationUsers.Where(x => x.Id == this.InputEmail).FirstOrDefault();

                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    userName = user.DisplayName;
                    emailAddress = user.Email;
                }
            }
            else if (!String.IsNullOrEmpty(this.DistributionList)) 
            {
                ModelState.Remove("InputEmail");
                userName = "Not Available";
                emailAddress = this.DistributionList;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!String.IsNullOrEmpty(emailAddress) && !String.IsNullOrEmpty(userName))
            {
                var findEmaiList = _context.EmailGroupLists.Include(x => x.EmailGroupUserLists).Where(x => x.Id == this.EmailGroupList.Id).FirstOrDefault();

                if (findEmaiList.EmailGroupUserLists.Select(x => x.Email).Contains(emailAddress))
                {
                    ModelState.Clear();

                    TempData["Same Email"] = "Kindly choose different email address!";
                    return RedirectToPage(new { Id = this.EmailGroupList.Id });
                }
                else
                {
                    var newEmailGroupUserLists = new EmailGroupUserList();
                    newEmailGroupUserLists.EmailGroupListId = this.EmailGroupList.Id;
                    newEmailGroupUserLists.Username = userName;
                    newEmailGroupUserLists.Email = emailAddress;

                    _context.EmailGroupUserLists.Add(newEmailGroupUserLists);
                    await _context.SaveChangesAsync();
                }
            }

            ModelState.Clear();

            return RedirectToPage(new { Id = this.EmailGroupList.Id });
        }
    }
}
