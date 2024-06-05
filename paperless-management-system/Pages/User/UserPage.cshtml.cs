using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.User
{
    public class UserPageModel : UserManagementPageModel
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager { get; set; }
        private RoleManager<IdentityRole> _roleManager { get; set; }

        public UserPageModel(ApplicationDbContext context, UserManager<ApplicationUser> mgr, RoleManager<IdentityRole> roleMgr)
        {
            _context = context;
            _userManager = mgr;
            _roleManager = roleMgr;
        }

        public List<SelectListItem> CostCenterList { get; set; }
        public List<SelectListItem> DepartmentList { get; set; }
        public List<SelectListItem> RoleList { get; set; }

        public IActionResult OnGet()
        {
            CostCenterList = _context.CostCenterLists.Where(x => x.Name != "All Department").Select(x => new {
                CostCenterName = x.Name
            }).Distinct().Select(x => new SelectListItem
            {
                Text = x.CostCenterName,
                Value = x.CostCenterName,
            }).ToList();
            ViewData["CostCenterSelectList"] = CostCenterList;

            DepartmentList = _context.Users.Where(x => x.CostCenterName != "Admin").Select(x => new {
                Department = x.Department
            }).Distinct().Where(x => x.Department != "Not Available").Select(x => new SelectListItem
            {
                Text = x.Department,
                Value = x.Department,
            }).ToList();
            ViewData["DepartmentList"] = DepartmentList;

            RoleList = _roleManager.Roles.Select(x => new SelectListItem { Text = x.Name, Value = x.Name }).ToList();
            ViewData["RoleList"] = RoleList;

            return Page();
        }
    }
}
