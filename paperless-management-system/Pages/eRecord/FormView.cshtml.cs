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
    public class FormViewModel : eRecordPageModel
    {
        private readonly ApplicationDbContext _context;

        public FormViewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FormList FormList { get; set; }

        public IActionResult OnGetAsync(int? FormId)
        {
            if (FormId == null)
            {
                return NotFound();
            }

            FormList = _context.FormLists.Where(m => m.Id == FormId).FirstOrDefault();

            if (FormList == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
