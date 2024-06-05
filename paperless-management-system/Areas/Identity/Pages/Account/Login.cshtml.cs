// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WD_ERECORD_CORE.Data;
using System.Runtime.Versioning;
using System.DirectoryServices.AccountManagement;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WD_ERECORD_CORE.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext content, IWebHostEnvironment _environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = content;
            Environment = _environment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Employee Id is required")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        [SupportedOSPlatform("windows")]
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        [SupportedOSPlatform("windows")]
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                if (this.Environment.IsDevelopment())
                {
                    var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");

                        var User = _userManager.FindByNameAsync(Input.UserName);

                        string LogDetail = String.Format("{0} login to system", User.Result.DisplayName);
                        PublicLogSystem pls = new PublicLogSystem() { DateTime = DateTime.Now, UserName = User.Result.DisplayName, UserId = User.Result.UserName, LogDetail = LogDetail };

                        _context.PublicLogSystems.Add(pls);
                        await _context.SaveChangesAsync();

                        return LocalRedirect(returnUrl);
                    }

                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt");
                        var user = await _userManager.FindByNameAsync(Input.UserName);
                        if (user == null)
                        {
                            ModelState.AddModelError(string.Empty, "Invalid Employee Id");
                        }
                        else if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                        {
                            ModelState.AddModelError(string.Empty, "Invalid Password");
                        }
                        return Page();
                    }
                }
                else if (this.Environment.IsProduction())
                {
                    if (Input.UserName == "admin" || Input.UserName == "approver-qa" || Input.UserName == "ccbapprover-lv1" || Input.UserName == "ccbapprover-lv2" || Input.UserName == "ccbapprover-lv3")
                    {
                        var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User logged in.");

                            var User = _userManager.FindByNameAsync(Input.UserName);

                            string LogDetail = String.Format("{0} login to system", User.Result.DisplayName);
                            PublicLogSystem pls = new PublicLogSystem() { DateTime = DateTime.Now, UserName = User.Result.DisplayName, UserId = User.Result.UserName, LogDetail = LogDetail };

                            _context.PublicLogSystems.Add(pls);
                            await _context.SaveChangesAsync();

                            return LocalRedirect(returnUrl);
                        }

                        if (result.RequiresTwoFactor)
                        {
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        }

                        if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User account locked out.");
                            return RedirectToPage("./Lockout");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                            var user = await _userManager.FindByNameAsync(Input.UserName);
                            if (user == null)
                            {
                                ModelState.AddModelError(string.Empty, "Invalid UserName");
                            }
                            else if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                            {
                                ModelState.AddModelError(string.Empty, "Invalid Password");
                            }
                            return Page();
                        }
                    }
                    else
                    {
                        // This doesn't count login failures towards account lockout
                        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "ad.shared", "OU=MPP,OU=Malaysia,OU=StdUsers,OU=UsersAndGroups,OU=Accounts,DC=ad,DC=shared"))
                        {
                            // validate the credentials
                            bool isValid = pc.ValidateCredentials(Input.UserName, Input.Password);

                            if (isValid)
                            {
                                var user = await _userManager.FindByNameAsync(Input.UserName);

                                if (user != null)
                                {
                                    _logger.LogInformation("User logged in.");

                                    var result = await _signInManager.PasswordSignInAsync(Input.UserName, "admin", Input.RememberMe, lockoutOnFailure: false);
                                    if (result.Succeeded)
                                    {
                                        var User = _userManager.FindByNameAsync(Input.UserName);

                                        string LogDetail = String.Format("{0} login to system", User.Result.DisplayName);
                                        PublicLogSystem pls = new PublicLogSystem() { DateTime = DateTime.Now, UserName = User.Result.DisplayName, UserId = User.Result.UserName, LogDetail = LogDetail };

                                        _context.PublicLogSystems.Add(pls);
                                        await _context.SaveChangesAsync();

                                        return LocalRedirect(returnUrl);
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Not such user found in e-Record system.");
                                    return Page();
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Fail authentication with AD");
                                return Page();
                            }
                        }
                    }
            }
        }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
