using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity {

    [Authorize(Roles = "System Admin, CCB Approver")]
    public class MasterFormCCBApprovalPageModel : PageModel {

        // no methods or properties required
    }
}
