using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.DepartmentCode
{
    public class IndexModel : DepartmentCodePageModel
    {
        private readonly ApplicationDbContext _context;
        public UserManager<ApplicationUser> UserManager { get; set; }

        [BindProperty]
        public bool ContainSystemAdmin { get; set; } = false;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> mgr)
        {
            _context = context;
            UserManager = mgr;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await UserManager.GetUserAsync(HttpContext.User);
        }

        public async Task<IActionResult> OnGet()
        {
            var User = await GetCurrentUser();
            var Roles = UserManager.GetRolesAsync(User).Result.ToArray();
            this.ContainSystemAdmin = Roles.Contains("System Admin");

            return Page();
        }
    }
}
