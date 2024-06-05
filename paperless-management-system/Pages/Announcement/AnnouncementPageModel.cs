using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity 
{
    [Authorize(Roles = "System Admin, Announcement Group")]
    public class AnnouncementPageModel : PageModel {

        // no methods or properties required
    }
}
