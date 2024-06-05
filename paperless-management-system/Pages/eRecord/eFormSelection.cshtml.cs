using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.eRecord
{
    public class eFormSelectionModel : eRecordPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public eFormSelectionModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        public async Task<IActionResult> OnGet()
        {
            var currentUser = await GetCurrentUser();
            ViewData["CostCenter"] = (!String.IsNullOrEmpty(currentUser.CostCenterName) ? currentUser.CostCenterName : "Not Available");

            return Page();
        }
    }
}
