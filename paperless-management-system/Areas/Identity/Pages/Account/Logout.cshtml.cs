// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment Environment;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, UserManager<ApplicationUser> userManager,
            ApplicationDbContext context, IWebHostEnvironment _environment)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _context = context;
            Environment = _environment;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            if (this.Environment.IsProduction()) {
                var User = await _userManager.GetUserAsync(HttpContext.User);
                string LogDetail = String.Format("{0} logout from system", User.DisplayName);
                PublicLogSystem pls = new PublicLogSystem() { DateTime = DateTime.Now, UserName = User.DisplayName, UserId = User.UserName, LogDetail = LogDetail };

                _context.PublicLogSystems.Add(pls);
                await _context.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
