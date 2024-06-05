using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.FormHistory
{
    [Authorize(Roles = "System Admin, Auditor Group")]
    public class AdminPageModel : FormHistoryPageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminPageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost([FromBody] List<int> request) {
            var data = _context.FormListHistories.Where(x => request.Contains(x.Id)).Select(x => new { Id = x.Id, FormData = x.FormData, FormSubmittedData = x.FormSubmittedData }).ToList();

            return new JsonResult(data);
        }
    }
}
