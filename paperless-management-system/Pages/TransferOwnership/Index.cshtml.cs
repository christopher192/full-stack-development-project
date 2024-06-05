using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.TransferOwnership
{
    [Authorize(Roles = "System Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public UserManager<ApplicationUser> UserManager { get; set; }

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> mgr)
        {
            _context = context;
            UserManager = mgr;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await UserManager.GetUserAsync(HttpContext.User);
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string NewOwner, params int[] FormIdCollection)
        {
            if (FormIdCollection.Length != 0 && !String.IsNullOrEmpty(NewOwner))
            {
                using (var databaseTran = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var CurrentUser = await GetCurrentUser();

                        var UserName = _context.ApplicationUsers.Where(x => x.UserName == NewOwner).FirstOrDefault();
                        var UpdateOnwer = _context.MasterFormLists.Where(x => FormIdCollection.Contains(x.Id)).ToList();
                        var OriginalOwner = new Dictionary<string, string>();

                        foreach (var owner in UpdateOnwer)
                        {
                            owner.OwnerCostCenter = UserName.CostCenterName;
                            owner.OwnerEmailAddress = UserName.Email;
                            owner.Owner = UserName.UserName;

                            OriginalOwner.Add(owner.MasterFormName, (owner.CreatedBy + " - " + owner.Owner));

                            if (owner.CreatedBy != null && owner.CreatedDate != null)
                            {
                                owner.CreatedBy = UserName.DisplayName;
                            }

                            if (owner.ModifiedBy != null && owner.ModifiedDate != null)
                            {
                                owner.ModifiedBy = UserName.DisplayName;
                            }

                            if (owner.SubmittedBy != null && owner.SubmittedDate != null)
                            {
                                owner.SubmittedBy = UserName.DisplayName;
                            }

                            if (owner.MasterFormStatus == "editing" && owner.CurrentEditor != null)
                            {
                                owner.CurrentEditor = UserName.UserName;
                            }
                        }

                        _context.MasterFormLists.UpdateRange(UpdateOnwer);

                        var publicLogTransForm = new PublicLogTransferForm();
                        publicLogTransForm.UserId = CurrentUser.UserName;
                        publicLogTransForm.UserName = CurrentUser.DisplayName;
                        publicLogTransForm.DateTime = DateTime.Now;

                        if (OriginalOwner.Count() > 1)
                        {
                            publicLogTransForm.LogDetail = String.Format("Master Forms {0} have been transferred to {1}", String.Join(", ", OriginalOwner.Select(x => x.Key + " (" + x.Value + ")").ToList()), (UserName.DisplayName + " (" + UserName.UserName + ")"));
                        }
                        else
                        {
                            publicLogTransForm.LogDetail = String.Format("Master Form {0} has been transferred to {1}", String.Join(", ", OriginalOwner.Select(x => x.Key + " (" + x.Value + ")").ToList()), (UserName.DisplayName + " (" + UserName.UserName + ")"));
                        }

                        _context.PublicLogTransferForms.Add(publicLogTransForm);

                        await _context.SaveChangesAsync();

                        databaseTran.Commit();

                        return RedirectToPage();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return RedirectToPage();
        }
    }
}
