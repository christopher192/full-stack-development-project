using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.UserManual
{
    [Authorize(Roles = "System Admin")]
    public class DeleteModel : UserManualPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DeleteModel(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public UserManualList UserManualList { get; set; }

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
            if (this.UserManualList.Id == null)
            {
                return NotFound();
            }

            UserManualList = await _context.UserManualLists.FindAsync(this.UserManualList.Id);

            if (UserManualList != null)
            {
                string contextRootPath = _env.ContentRootPath;
                string deletePath = Path.Combine(contextRootPath + @"\UserManualFiles\", UserManualList.UserManualFilePath);

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }

                _context.UserManualLists.Remove(UserManualList);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
