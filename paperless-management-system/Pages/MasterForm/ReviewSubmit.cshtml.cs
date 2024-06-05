using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Pages.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using WD_ERECORD_CORE.ViewModels;
using Newtonsoft.Json.Linq;
using System.Text;
using WD_ERECORD_CORE.Function;
using NuGet.Protocol.Plugins;

namespace WD_ERECORD_CORE.Pages.MasterForm
{
    public class ReviewSubmitModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment Environment;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string? FormMode { get; set; }

        [BindProperty]
        public ReviewSubmitViewModel ReviewSubmitViewModel { get; set; } = new ReviewSubmitViewModel();

        [BindProperty]
        [Display(Name = "New Change Log")]
        public string? NewChangeLog { get; set; }

        public ReviewSubmitModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment _environment, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            Environment = _environment;
            _configuration = configuration;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        public IActionResult OnGet(int? Id)
        {
            if (TempData["RequestFormMode"] as string == "Create" && Id != null)
            {
                this.FormMode = "Create";
            }
            else if (TempData["RequestFormMode"] as string == "Edit" && Id != null)
            {
                this.FormMode = "Edit";
            }
            else if (TempData["RequestFormMode"] as string == "EditOnly" && Id != null)
            {
                this.FormMode = "EditOnly";
            }

            if (String.IsNullOrEmpty(this.FormMode))
            {
                return NotFound();
            }

            var masterFormList = _context.MasterFormLists.Where(x => x.Id == Id).FirstOrDefault();

            if (masterFormList != null)
            {
                this.ReviewSubmitViewModel.MasterFormId = masterFormList.Id;
                this.ReviewSubmitViewModel.MasterFormName = masterFormList.MasterFormName;
                this.ReviewSubmitViewModel.MasterFormDescription = masterFormList.MasterFormDescription;
                this.ReviewSubmitViewModel.MasterFormData = masterFormList.MasterFormData;
                this.ReviewSubmitViewModel.ChangeLog = masterFormList.ChangeLog;
                this.ReviewSubmitViewModel.GuidelineFile = masterFormList.GuidelineFile;
                this.ReviewSubmitViewModel.UniqueGuidelineFile = masterFormList.UniqueGuidelineFile;
                this.ReviewSubmitViewModel.MasterFormCreatedBy = masterFormList.CreatedBy;
                this.ReviewSubmitViewModel.Owner = masterFormList.Owner;
            }
            else
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPostPrevious() 
        {
            TempData["RequestFormMode"] = this.FormMode;
            return RedirectToPage("CCBApproval", new { Id = this.ReviewSubmitViewModel.MasterFormId });
        }
            
        public async Task<IActionResult> OnPostSubmitAsync()    
        {
            if (this.FormMode == "Create" || this.FormMode == "Edit")
            {
                var masterForm = _context.MasterFormLists.Where(x => x.Id == this.ReviewSubmitViewModel.MasterFormId).Include(x => x.MasterFormDepartments)
                    .ThenInclude(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).FirstOrDefault();

                if (masterForm != null)
                {
                    // get authorized user info
                    var user = await GetCurrentUser();

                    // get department with approval level and approvers
                    var getDepartments = masterForm.MasterFormDepartments;
                    IDictionary<int, List<string>> emailNotifiersInfo = new Dictionary<int, List<string>>();

                    using (var databaseTran = _context.Database.BeginTransaction()) 
                    {
                        try
                        {
                            // if all departments have ccb approval levels
                            if (getDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).Count() > 0)
                            {
                                foreach (var department in getDepartments)
                                {
                                    // if department has approval level, change department status To 'pending'
                                    if (department.MasterFormCCBApprovalLevels.Count() > 0)
                                    {
                                        // set 'pending' to department status
                                        department.Status = "pending";

                                        var ccbApprovalLevel = department.MasterFormCCBApprovalLevels.OrderBy(x => x.Id).FirstOrDefault();

                                        // set 'pending' to first ccb approval level
                                        if (ccbApprovalLevel != null)
                                        {
                                            ccbApprovalLevel.ApprovalStatus = "pending";
                                            emailNotifiersInfo[ccbApprovalLevel.Id] = new List<string>();

                                            // set 'pending' to all approvers in first ccb approval level
                                            if (ccbApprovalLevel.MasterFormCCBApprovers.ToList().Count > 0)
                                            {
                                                ccbApprovalLevel.MasterFormCCBApprovers.ToList().ForEach(x => { x.ApproverStatus = "pending"; emailNotifiersInfo[ccbApprovalLevel.Id].Add(x.ApproverEmail); });
                                            }
                                        }
                                    }
                                }

                                // update master form`s status to 'pending' and save relevant data
                                masterForm.MasterFormStatus = "pending";
                                masterForm.SubmittedDate = DateTime.Now;
                                masterForm.SubmittedBy = user.DisplayName;
                                masterForm.CurrentEditor = null;

                                if (!String.IsNullOrEmpty(this.NewChangeLog))
                                {
                                    if (masterForm.ChangeLog == null)
                                    {
                                        masterForm.ChangeLog = DateTime.Now.ToString("dd MMM yy, hh:mm:ss") + " - " + this.NewChangeLog;
                                    }
                                    else
                                    {
                                        masterForm.ChangeLog += ("\r\n" + DateTime.Now.ToString("dd MMM yy, hh:mm:ss") + " - " + this.NewChangeLog);
                                    }
                                }

                                // populate json data
                                var json = JsonConvert.SerializeObject(masterForm, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                                JObject modifiedJSON = JObject.Parse(json);
                                modifiedJSON["MasterFormData"] = "";
                                modifiedJSON["JSON"] = "";
                                masterForm.JSON = modifiedJSON.ToString(Formatting.None);

                                _context.MasterFormLists.Update(masterForm);
                                await _context.SaveChangesAsync();

                                if (Environment.IsProduction())
                                {
                                    if (emailNotifiersInfo.Count() > 0)
                                    {
                                        string DomainName = _configuration["Domain:ServerURL"].ToString() + "/MasterFormCCBApproval/Index";

                                        foreach (var emailNotifiers in emailNotifiersInfo)
                                        {
                                            var confirmSend = false;

                                            var Senders = emailNotifiers.Value.ToArray();

                                            if (Senders.Length > 0)
                                            {
                                                StringBuilder StrMessage = new StringBuilder();
                                                StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", this.ReviewSubmitViewModel.MasterFormName, this.ReviewSubmitViewModel.MasterFormDescription, this.ReviewSubmitViewModel.MasterFormCreatedBy);
                                                StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                                var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - CCB Approval", StrMessage.ToString(), Senders, null);

                                                if (emailNotificationResult == "Success")
                                                {
                                                    var logEmailNotification = new PublicLogEmailNotification();
                                                    logEmailNotification.DateTime = DateTime.Now;
                                                    logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approval", String.Join(", ", Senders), this.ReviewSubmitViewModel.MasterFormName);

                                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                    confirmSend = true;
                                                }
                                                else
                                                {
                                                    var logEmailNotification = new PublicLogEmailNotification();
                                                    logEmailNotification.DateTime = DateTime.Now;
                                                    logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approval", String.Join(", ", Senders), this.ReviewSubmitViewModel.MasterFormName);

                                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                }

                                                if (confirmSend)
                                                {
                                                    var updateAppLvl = _context.MasterFormCCBApprovalLevels.Where(x => x.Id == emailNotifiers.Key).FirstOrDefault();

                                                    if (updateAppLvl != null)
                                                    {
                                                        updateAppLvl.LastSend = DateTime.Now;

                                                        _context.Attach(updateAppLvl);
                                                        _context.Entry(updateAppLvl).Property(x => x.LastSend).IsModified = true;
                                                    }
                                                }

                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                    }
                                }
                                else if (Environment.IsDevelopment())
                                {
                                    if (emailNotifiersInfo.Count() > 0)
                                    {
                                        string DomainName = _configuration["Domain:ServerURL"].ToString() + "/MasterFormCCBApproval/Index";

                                        foreach (var emailNotifiers in emailNotifiersInfo)
                                        {
                                            var confirmSend = false;

                                            var Senders = emailNotifiers.Value.ToArray();

                                            if (Senders.Length > 0)
                                            {
                                                StringBuilder StrMessage = new StringBuilder();
                                                StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", this.ReviewSubmitViewModel.MasterFormName, this.ReviewSubmitViewModel.MasterFormDescription, this.ReviewSubmitViewModel.MasterFormCreatedBy);
                                                StrMessage.AppendFormat("<p>Receiver: {0}</p>", String.Join(", ", Senders));
                                                StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                                var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - CCB Approval", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                                if (emailNotificationResult == "Success")
                                                {
                                                    var logEmailNotification = new PublicLogEmailNotification();
                                                    logEmailNotification.DateTime = DateTime.Now;
                                                    logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approval", String.Join(", ", Senders), this.ReviewSubmitViewModel.MasterFormName);

                                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                    confirmSend = true;
                                                }
                                                else
                                                {
                                                    var logEmailNotification = new PublicLogEmailNotification();
                                                    logEmailNotification.DateTime = DateTime.Now;
                                                    logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approval", String.Join(", ", Senders), this.ReviewSubmitViewModel.MasterFormName);

                                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                }

                                                if (confirmSend)
                                                {
                                                    var updateAppLvl = _context.MasterFormCCBApprovalLevels.Where(x => x.Id == emailNotifiers.Key).FirstOrDefault();

                                                    if (updateAppLvl != null)
                                                    {
                                                        updateAppLvl.LastSend = DateTime.Now;

                                                        _context.Attach(updateAppLvl);
                                                        _context.Entry(updateAppLvl).Property(x => x.LastSend).IsModified = true;
                                                    }
                                                }

                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                    }
                                }

                                databaseTran.Commit();

                                TempData["Save"] = "Close Window";
                                return Page();
                            }
                            // if all departments don`t have ccb approval levels
                            else
                            {
                                // update master form` status to 'active' and save relevant data
                                masterForm.MasterFormStatus = "active";
                                masterForm.SubmittedDate = DateTime.Now;
                                masterForm.SubmittedBy = user.DisplayName;
                                masterForm.AllowUpRevision = true;
                                masterForm.CurrentEditor = null;

                                if (!String.IsNullOrEmpty(this.NewChangeLog))
                                {
                                    if (masterForm.ChangeLog == null)
                                    {
                                        masterForm.ChangeLog = DateTime.Now.ToString("dd MMM yy, hh:mm:ss") + " - " + this.NewChangeLog;
                                    }
                                    else
                                    {
                                        masterForm.ChangeLog += ("\r\n" + DateTime.Now.ToString("dd MMM yy, hh:mm:ss") + " - " + this.NewChangeLog);
                                    }
                                }

                                // populate json data
                                var json = JsonConvert.SerializeObject(masterForm, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                                JObject modifiedJSON = JObject.Parse(json);
                                modifiedJSON["MasterFormData"] = "";
                                modifiedJSON["JSON"] = "";
                                masterForm.JSON = modifiedJSON.ToString(Formatting.None);

                                var permittedDepartmentJson = JsonConvert.SerializeObject(masterForm.MasterFormDepartments.Select(x => x.DepartmentName).ToList(), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                                masterForm.PermittedDepartments = permittedDepartmentJson;

                                // assign these two so can populate later
                                var historyJSON = modifiedJSON.ToString(Formatting.None);
                                var historyCCBApprovalJSON = JsonConvert.SerializeObject(masterForm.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).SelectMany(x => x.MasterFormCCBApprovers).Where(x => x.ApproverStatus == "approved" || x.ApproverStatus == "rejected").Select(x => x.EmployeeId).ToList(), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                                // disable master form parent
                                // if master form parent id is found
                                if (masterForm.MasterFormParentId > 0)
                                {
                                    var mfparent = _context.MasterFormLists.Where(x => x.Id == masterForm.MasterFormParentId).FirstOrDefault();

                                    if (mfparent != null)
                                    {
                                        _context.Attach(mfparent);
                                        _context.Entry(mfparent).Property(x => x.AllowUpRevision).IsModified = true;
                                        _context.Entry(mfparent).Property(x => x.MasterFormStatus).IsModified = true;

                                        mfparent.AllowUpRevision = false;
                                        mfparent.MasterFormStatus = "inactive";
                                    }
                                }

                                _context.MasterFormLists.Update(masterForm);

                                // add master form history
                                var formMasterListHistory = new MasterFormListHistory();
                                formMasterListHistory.MasterFormId = masterForm.Id;
                                formMasterListHistory.MasterFormName = masterForm.MasterFormName;
                                formMasterListHistory.MasterFormData = masterForm.MasterFormData;
                                formMasterListHistory.MasterFormDescription = masterForm.MasterFormDescription;
                                formMasterListHistory.MasterFormStatus = masterForm.MasterFormStatus;
                                formMasterListHistory.MasterFormRevision = masterForm.MasterFormRevision;
                                formMasterListHistory.Owner = masterForm.Owner;
                                formMasterListHistory.CreatedDate = masterForm.CreatedDate;
                                formMasterListHistory.CreatedBy = masterForm.CreatedBy;
                                formMasterListHistory.ModifiedDate = masterForm.ModifiedDate;
                                formMasterListHistory.ModifiedBy = masterForm.ModifiedBy;
                                formMasterListHistory.SubmittedDate = masterForm.SubmittedDate;
                                formMasterListHistory.SubmittedBy = masterForm.SubmittedBy;
                                formMasterListHistory.ArchievedDate = DateTime.Now;
                                formMasterListHistory.ChangeLog = masterForm.ChangeLog;
                                formMasterListHistory.CCBApprovalJSON = historyCCBApprovalJSON;
                                formMasterListHistory.JSON = historyJSON;

                                _context.MasterFormListHistories.Add(formMasterListHistory);
                                await _context.SaveChangesAsync();

                                if (Environment.IsProduction())
                                {
                                    string DomainName = _configuration["Domain:ServerURL"].ToString() + "/MasterForm/Index";
                                    string Sender = masterForm.OwnerEmailAddress;

                                    StringBuilder StrMessage = new StringBuilder();
                                    StrMessage.AppendFormat("<p>Your master form <b>{0} ({1})</b> is approved</p>", this.ReviewSubmitViewModel.MasterFormName, this.ReviewSubmitViewModel.MasterFormDescription);
                                    StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                    var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, Sender);

                                    if (emailNotificationResult == "Success")
                                    {
                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approve notification", Sender, this.ReviewSubmitViewModel.MasterFormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                    }
                                    else
                                    {
                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approve notification", Sender, this.ReviewSubmitViewModel.MasterFormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                    }

                                    await _context.SaveChangesAsync();
                                }
                                else if (Environment.IsDevelopment())
                                {
                                    string DomainName = _configuration["Domain:ServerURL"].ToString() + "/MasterForm/Index";
                                    string Sender = masterForm.OwnerEmailAddress;

                                    StringBuilder StrMessage = new StringBuilder();
                                    StrMessage.AppendFormat("<p>Your master form <b>{0} ({1})</b> is approved</p>", this.ReviewSubmitViewModel.MasterFormName, this.ReviewSubmitViewModel.MasterFormDescription);
                                    StrMessage.AppendFormat("<p>Receiver: {0}</p>", Sender);
                                    StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                    var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                    if (emailNotificationResult == "Success")
                                    {
                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approve notification", Sender, this.ReviewSubmitViewModel.MasterFormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                    }
                                    else
                                    {
                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approve notification", Sender, this.ReviewSubmitViewModel.MasterFormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                    }

                                    await _context.SaveChangesAsync();
                                }

                                databaseTran.Commit();

                                TempData["Save"] = "Close Window";
                                return Page();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }                    
                    }
                }
            }
            else {
                TempData["Save"] = "Close Window";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (this.FormMode == "Create" || (this.FormMode == "Edit"))
            {
                var disableCurrentEditor = _context.MasterFormLists.Where(x => x.Id == this.ReviewSubmitViewModel.MasterFormId).FirstOrDefault();

                if (disableCurrentEditor.CurrentEditor != null)
                {
                    disableCurrentEditor.CurrentEditor = null;

                    _context.Attach(disableCurrentEditor);
                    _context.Entry(disableCurrentEditor).Property(x => x.CurrentEditor).IsModified = true;

                    await _context.SaveChangesAsync();
                }
            }

            TempData["Save"] = "Close Window";
            return Page();
        }
    }
}
