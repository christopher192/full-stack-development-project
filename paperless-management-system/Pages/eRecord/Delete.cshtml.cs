using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.eRecord
{
    public class DeleteModel : eRecordPageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FormList FormList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? FormId)
        {
            if (FormId == null)
            {
                return NotFound();
            }

            FormList = await _context.FormLists.FirstOrDefaultAsync(m => m.Id == FormId);

            if (FormList == null || (FormList.FormStatus != "new" && FormList.FormStatus != "editing"))
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (FormList.Id == null)
            {
                return NotFound();
            }

            var DeleteFormList = await _context.FormLists.FindAsync(FormList.Id);

            if (DeleteFormList != null)
            {
                _context.FormLists.Remove(DeleteFormList);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
