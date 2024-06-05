using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity {

    [Authorize(Roles = "System Admin, L1 Admin")]
    public class EmailGroupingPageModel : PageModel {

        // no methods or properties required
    }
}
