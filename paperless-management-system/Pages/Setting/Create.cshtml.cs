using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.Setting
{
    public class CreateModel : PageModel
    {
        private readonly WD_ERECORD_CORE.Data.ApplicationDbContext _context;

        [BindProperty]
        public List<SelectListItem> APIType { get; set; }

        public CreateModel(WD_ERECORD_CORE.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            this.APIType = new List<SelectListItem>() {
                new SelectListItem { Value = "", Text = "Select API Type" },
                new SelectListItem { Value = "oracle_sql_query", Text = "Oracle SQL Query" },
                new SelectListItem { Value = "mssql_query", Text = "MSSQL SQL Query" },
                new SelectListItem { Value = "fix_expression", Text = "Fix Expression" },
                new SelectListItem { Value = "web_service", Text = "Web Service"},
                new SelectListItem { Value = "json", Text = "Json" },
            };

            return Page();
        }

        [BindProperty]
        public APISetting APISetting { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

/*            _context.APISettings.Add(APISetting);
            await _context.SaveChangesAsync();*/

            return RedirectToPage("./Index");
        }
    }
}
