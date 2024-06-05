using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.EFormApproval
{
    public class FormViewModel : EFormApprovalPageModel
    {
        private readonly ApplicationDbContext _context;

        public FormList FormList = new FormList();

        public FormViewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? FormId)
        {
            this.FormList = _context.FormLists.Where(x => x.Id == FormId).FirstOrDefault();

            return Page();
        }
    }
}
