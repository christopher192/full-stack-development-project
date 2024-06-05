using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.Data.Datatable;
using WD_ERECORD_CORE.Extensions;

namespace WD_ERECORD_CORE.Pages.EmailGrouping
{
    public class IndexModel : EmailGroupingPageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostLoadTable([FromBody] DtParameters dtParameters)
        {
            var data = _context.EmailGroupLists;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = data.AsQueryable();

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await data.CountAsync();

            return new JsonResult(new DtResult<EmailGroupList>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }
    }
}
