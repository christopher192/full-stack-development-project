using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.eRecord
{
    public class CreateModel : eRecordPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public int? MasterFormId { get; set; }

        [BindProperty]
        public string? ReturnURL { get; set; }

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? MasterFormId, string? ReturnURL)
        {
            if (MasterFormId == null || String.IsNullOrEmpty(ReturnURL))
            {
                return NotFound();
            }
            else
            {
                this.MasterFormId = MasterFormId;
                this.ReturnURL = ReturnURL;
            }

            return Page();
        }
    }
}
