using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.FormHistory
{
    public class DetailsModel : FormHistoryPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public int? FormHistoryId { get; set; }

        [BindProperty]
        public string ReturnURL { get; set; }

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? FormHistoryId, string? ReturnURL)
        {
            if (FormHistoryId == null || String.IsNullOrEmpty(ReturnURL))
            {
                return NotFound();
            }
            else
            {
                this.FormHistoryId = FormHistoryId;
                this.ReturnURL = ReturnURL;
            }

            return Page();
        }
    }
}
