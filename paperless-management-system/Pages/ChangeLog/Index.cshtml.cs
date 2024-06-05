using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.EntityFrameworkCore.Design.Internal;

namespace WD_ERECORD_CORE.Pages.ChangeLog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
