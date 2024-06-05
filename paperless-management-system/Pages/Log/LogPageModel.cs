using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity {

    [Authorize(Roles = "System Admin, Auditor Group")]
    public class LogPageModel : PageModel {

        // no methods or properties required
    }
}
