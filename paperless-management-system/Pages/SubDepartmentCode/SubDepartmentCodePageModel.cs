using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity 
{
    [Authorize(Roles = "System Admin, L1 Admin")]
    public class SubDepartmentCodePageModel : PageModel {

        // no methods or properties required
    }
}
