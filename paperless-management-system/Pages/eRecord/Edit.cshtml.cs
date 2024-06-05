using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.eRecord
{
    public class EditModel : eRecordPageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int? FormId { get; set; }

        public IActionResult OnGet(int? FormId)
        {
            if (FormId == null)
            {
                return NotFound();
            }
            else
            {
                this.FormId = FormId;
            }

            return Page();
        }
    }
}
