using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity {

    [Authorize(Roles = "Normal User, System Admin")]
    public class EFormApprovalPageModel : PageModel {

        // no methods or properties required
    }
}
