using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.MasterFormHistory
{
    public class DetailsModel : MasterFormHistoryPageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public int? Id { get; set; }

        [BindProperty]
        public string ReturlURL { get; set; }

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public MasterFormList MasterFormList { get; set; }

        public ActionResult OnGet(int? Id, string ReturlURL)
        {
            if (Id == null || String.IsNullOrEmpty(ReturlURL))
            {
                return NotFound();
            }
            else
            {
                this.Id = Id;
                this.ReturlURL = ReturlURL;
            }

            return Page();
        }
    }
}
