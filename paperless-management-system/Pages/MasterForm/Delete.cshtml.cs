using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.MasterForm
{
    public class DeleteModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        [BindProperty]
        public MasterFormList MasterFormList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            MasterFormList = await _context.MasterFormLists.FirstOrDefaultAsync(m => m.Id == Id);

            if (MasterFormList != null)
            {
                var currentUser = await GetCurrentUser();
                var roles = await _userManager.GetRolesAsync(currentUser);
                var haveSystemAdmin = roles.Contains("System Admin");

                // allow delete when master form is rejected or editing status 
                // super admin able to delete while the master form status is editing
                if (MasterFormList.MasterFormStatus == "editing")
                {
                    if (haveSystemAdmin) 
                    {
                        return Page();
                    }
                    else
                    {
                        return NotFound();
                    }
                    
                }
                else if (MasterFormList.MasterFormStatus == "rejected")
                {
                    return Page();
                }
            }
            else 
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            MasterFormList = await _context.MasterFormLists.FindAsync(Id);

            if (MasterFormList != null)
            {
                _context.MasterFormLists.Remove(MasterFormList);

                if (MasterFormList.MasterFormParentId > 0)
                {
                    // enable master form parent up revision
                    var masterFormParent = _context.MasterFormLists.Where(x => x.Id == MasterFormList.MasterFormParentId).FirstOrDefault();

                    if (masterFormParent != null)
                    {
                        masterFormParent.AllowUpRevision = true;

                        _context.Attach(masterFormParent);
                        _context.Entry(masterFormParent).Property(x => x.AllowUpRevision).IsModified = true;
                    }
                }

                // delete exsiting files
                // check if file exisis, otherwise do nothing
                if (!String.IsNullOrEmpty(MasterFormList.UniqueGuidelineFile) && !String.IsNullOrEmpty(MasterFormList.GuidelineFile))
                {
                    string ContentRootPath = _env.ContentRootPath + "/GuidelineFiles/";

                    // check if file exisis in server, otherwise do nothing
                    var DeleteFilePath = Path.Combine(ContentRootPath, MasterFormList.UniqueGuidelineFile);

                    if (System.IO.File.Exists(DeleteFilePath))
                    {
                        System.IO.File.Delete(DeleteFilePath);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
