using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.MasterFormCCBApproval
{
    public class MasterFormViewModel : MasterFormCCBApprovalPageModel
    {
        private readonly ApplicationDbContext _context;

        public MasterFormList MasterFormList = new MasterFormList();

        public MasterFormViewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? MasterFormId)
        {
            this.MasterFormList = _context.MasterFormLists.Where(x => x.Id == MasterFormId).FirstOrDefault();

            return Page();
        }
    }
}
