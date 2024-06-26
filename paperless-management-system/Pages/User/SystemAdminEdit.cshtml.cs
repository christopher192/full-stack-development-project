using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;

namespace WD_ERECORD_CORE.Pages.User
{
    [Authorize(Roles = "System Admin")]
    public class SystemAdminEditModel : UserManagementPageModel
    {
        private readonly ApplicationDbContext _context;

        public UserManager<ApplicationUser> _userManager { get; set; }

        public RoleManager<IdentityRole> _roleManager { get; set; }

        [BindProperty]
        public ApplicationUserViewModel user { get; set; }

        [BindProperty]
        public List<SelectListItem> rolesList { get; set; } = new List<SelectListItem>();

        public SystemAdminEditModel(UserManager<ApplicationUser> mgr, RoleManager<IdentityRole> roleMgr, ApplicationDbContext context)
        {
            _userManager = mgr;
            _roleManager = roleMgr;
            _context = context; 
        }

        public async Task<IActionResult> OnGetAsync(string? Id)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                this.user = new ApplicationUserViewModel()
                {
                    Id = user.Id,
                    ProfilePicture = user.ProfilePicture,
                    UserName = user.UserName,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Title = user.Title,
                    EmployeeType = user.EmployeeType,
                    Department = user.Department,
                    CostCenterID = user.CostCenterID,
                    CostCenterName = user.CostCenterName,
                };

                this.rolesList = _roleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                }).ToList();
            }
            else
            {
                return NotFound();
            }

            return Page();
        }

        private bool ValidateFile(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            string[] allowedFileTypes = { ".gif", ".png", ".jpeg", ".jpg", ".jfif" };

            if (allowedFileTypes.Contains(fileExtension))
            {
                return true;
            }

            return false;
        }

        public async Task<IActionResult> OnPostAsync(params string[] selectedRole)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser updateUser = _context.ApplicationUsers.Include(x => x.SystemRoles).Where(x => x.Id == this.user.Id).FirstOrDefault();

            if (updateUser != null)
            {
                var userRoles = await _userManager.GetRolesAsync(updateUser);
                selectedRole = selectedRole ?? new string[] { };

                if (this.user.UploadImage != null)
                {
                    if (ValidateFile(this.user.UploadImage))
                    {
                        using (var ms = new MemoryStream())
                        {
                            this.user.UploadImage.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string s = Convert.ToBase64String(fileBytes);
                            this.user.ProfilePicture = s;

                            updateUser.ProfilePicture = s;

                            var addRoleList = selectedRole.Except(userRoles).ToArray<string>();
                            var removeRoleList = userRoles.Except(selectedRole).ToArray<string>();

                            if (addRoleList.Length != 0)
                            {
                                foreach (var role in addRoleList)
                                {
                                    updateUser.SystemRoles.Add(new SystemRole() { Name = role });
                                }
                            }

                            if (removeRoleList.Length != 0)
                            {
                                foreach (var role in removeRoleList)
                                {
                                    var deleteRole = updateUser.SystemRoles.Where(x => x.Name == role).FirstOrDefault();
                                    updateUser.SystemRoles.Remove(deleteRole);
                                }
                            }

                            _context.ApplicationUsers.Update(updateUser);
                            await _context.SaveChangesAsync();
                        }

                        await _userManager.AddToRolesAsync(updateUser, selectedRole.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(updateUser, userRoles.Except(selectedRole).ToArray<string>());

                        return RedirectToPage("./SystemAdminPage");
                    }
                    else
                    {
                        ModelState.AddModelError("user.UploadImage", "Only .gif, .png, .jpeg, .jpg, .jfif file extension is allowed.");
                        return Page();
                    }
                }
                else
                {
                    var addRoleList = selectedRole.Except(userRoles).ToArray<string>();
                    var removeRoleList = userRoles.Except(selectedRole).ToArray<string>();

                    if (addRoleList.Length != 0)
                    {
                        foreach (var role in addRoleList)
                        {
                            updateUser.SystemRoles.Add(new SystemRole() { Name = role });
                        }
                    }

                    if (removeRoleList.Length != 0)
                    {
                        foreach (var role in removeRoleList)
                        {
                            var deleteRole = updateUser.SystemRoles.Where(x => x.Name == role).FirstOrDefault();
                            updateUser.SystemRoles.Remove(deleteRole);
                        }
                    }

                    _context.ApplicationUsers.Update(updateUser);
                    await _context.SaveChangesAsync();

                    await _userManager.AddToRolesAsync(updateUser, selectedRole.Except(userRoles).ToArray<string>());
                    await _userManager.RemoveFromRolesAsync(updateUser, userRoles.Except(selectedRole).ToArray<string>());

                    return RedirectToPage("./SystemAdminPage");
                }
            }

            return Page();
        }
    }
}
