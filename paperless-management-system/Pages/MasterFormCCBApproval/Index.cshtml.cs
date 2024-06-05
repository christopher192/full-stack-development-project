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

namespace WD_ERECORD_CORE.Pages.MasterFormCCBApproval
{
    public class IndexModel : MasterFormCCBApprovalPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public bool isSuperAdmin { get; set; } = false;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        public async Task<IActionResult> OnGet(int? MasterFormId)
        {
            var user = await GetCurrentUser();
            var roles = _userManager.GetRolesAsync(user).Result;

            if (roles.Contains("System Admin"))
            {
                this.isSuperAdmin = true;
            }
            else
            {
                this.isSuperAdmin = false;
            }

            if (MasterFormId != null)
            {
                ViewData["InputMasterFormId"] = MasterFormId.ToString();
            }

            return Page();
        }
    }
}
