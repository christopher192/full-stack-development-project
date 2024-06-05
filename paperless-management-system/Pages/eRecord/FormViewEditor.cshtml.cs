using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.Function;

namespace WD_ERECORD_CORE.Pages.eRecord
{
    public class FormViewEditorModel : eRecordPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment Environment;
        private readonly IConfiguration _configuration;

        public class UserInfo
        {
            public string employeeId { get; set; }
            public DateTime? dateTime { get; set; }
            public string department { get; set; }
            public string employeeId1 { get; set; }
        }

        [BindProperty]
        public FormList FormList { get; set; }

        [BindProperty]
        public UserInfo userInfo { get; set; } = new UserInfo();

        [BindProperty]
        public string? SubmittedType { get; set; }

        public FormViewEditorModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment _environment, IConfiguration configuration)
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

        public IActionResult OnGet(int? FormId)
        {
            if (FormId == null)
            {
                return NotFound();
            }

            this.FormList = _context.FormLists.Where(x => x.Id == FormId).FirstOrDefault();

            if (this.FormList == null || (this.FormList.FormStatus != "new" && this.FormList.FormStatus != "editing"))
            {
                return NotFound();
            }

            var GetUserInfo = _userManager.GetUserAsync(HttpContext.User);

            this.userInfo.dateTime = DateTime.Now;
            this.userInfo.employeeId = GetUserInfo.Result.EmployeeId;
            this.userInfo.department = GetUserInfo.Result.CostCenterName;
            this.userInfo.employeeId1 = GetUserInfo.Result.DisplayName;              

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();

            using (var databaseTran = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!String.IsNullOrEmpty(this.SubmittedType) && this.SubmittedType == "Update")
                    {
                        var user = await GetCurrentUser();
                        var getForm = _context.FormLists.Where(x => x.Id == this.FormList.Id).FirstOrDefault();

                        if (getForm == null || (getForm.FormStatus != "new" && getForm.FormStatus != "editing"))
                        {
                            return NotFound();
                        }

                        getForm.ModifiedDate = DateTime.Now;
                        getForm.ModifiedBy = user.DisplayName;
                        getForm.FormStatus = "editing";
                        getForm.FormSubmittedData = this.FormList.FormSubmittedData;

                        if (getForm.JSON != null)
                        {
                            JObject modifiedJSON = JObject.Parse(getForm.JSON);
                            modifiedJSON["ModifiedDate"] = getForm.ModifiedDate;
                            modifiedJSON["ModifiedBy"] = getForm.ModifiedBy;
                            modifiedJSON["FormStatus"] = getForm.FormStatus;

                            getForm.JSON = modifiedJSON.ToString(Formatting.None);
                        }

                        _context.Attach(getForm);

                        _context.Entry(getForm).Property(x => x.ModifiedDate).IsModified = true;
                        _context.Entry(getForm).Property(x => x.ModifiedBy).IsModified = true;
                        _context.Entry(getForm).Property(x => x.FormStatus).IsModified = true;
                        _context.Entry(getForm).Property(x => x.JSON).IsModified = true;
                        _context.Entry(getForm).Property(x => x.FormSubmittedData).IsModified = true;

                        await _context.SaveChangesAsync();

                        databaseTran.Commit();

                        ViewData["Result"] = "submit";
                        return Page();
                    }
                    else if (!String.IsNullOrEmpty(this.SubmittedType) && this.SubmittedType == "Submit")
                    {
                        var user = await GetCurrentUser();
                        var getForm = _context.FormLists.Where(x => x.Id == this.FormList.Id).Include(x => x.FormListApprovalLevels).ThenInclude(x => x.FormListApprovers).AsNoTracking().FirstOrDefault();
                        IDictionary<int, List<string>> emailNotifiersInfo = new Dictionary<int, List<string>>();

                        if (getForm == null || (getForm.FormStatus != "new" && getForm.FormStatus != "editing"))
                        {
                            return NotFound();
                        }

                        if (getForm.FormListApprovalLevels != null && getForm.FormListApprovalLevels.Count() > 0)
                        {
                            getForm.FormStatus = "pending";
                            getForm.SubmittedDate = DateTime.Now;
                            getForm.SubmittedBy = user.DisplayName;
                            getForm.FormSubmittedData = this.FormList.FormSubmittedData;

                            var approvalLevels = getForm.FormListApprovalLevels.OrderBy(x => x.Id).FirstOrDefault();
                            approvalLevels.ApprovalStatus = "pending";
                            emailNotifiersInfo[approvalLevels.Id] = new List<string>();

                            if (approvalLevels.FormListApprovers != null && approvalLevels.FormListApprovers.Count() > 0)
                            {
                                approvalLevels.FormListApprovers.ToList().ForEach(x => { x.ApproverStatus = "pending"; emailNotifiersInfo[approvalLevels.Id].Add(x.ApproverEmail); });
                            }

                            var json = JsonConvert.SerializeObject(getForm, new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                            JObject modifiedJSON = JObject.Parse(json);
                            modifiedJSON["FormData"] = "";
                            modifiedJSON["FormSubmittedData"] = "";

                            getForm.JSON = modifiedJSON.ToString(Formatting.None);

                            _context.FormLists.Update(getForm);
                            await _context.SaveChangesAsync();

                            if (Environment.IsProduction())
                            {
                                if (emailNotifiersInfo.Count() > 0)
                                {
                                    string DomainName = _configuration["Domain:ServerURL"].ToString() + "/EFormApproval/Index";

                                    foreach (var emailNotifiers in emailNotifiersInfo)
                                    {
                                        var confirmSend = false;

                                        var Senders = emailNotifiers.Value.ToArray();

                                        if (Senders.Length > 0)
                                        {
                                            StringBuilder StrMessage = new StringBuilder();
                                            StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", getForm.FormName, getForm.FormDescription, getForm.CreatedBy);
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - EForm Approval", StrMessage.ToString(), Senders, null);

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approval", String.Join(", ", Senders), getForm.FormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                confirmSend = true;
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approval", String.Join(", ", Senders), getForm.FormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }

                                            if (confirmSend)
                                            {
                                                var updateAppLvl = _context.FormListApprovalLevels.Where(x => x.Id == emailNotifiers.Key).FirstOrDefault();

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
                                    string DomainName = _configuration["Domain:ServerURL"].ToString() + "/EFormApproval/Index";

                                    foreach (var emailNotifiers in emailNotifiersInfo)
                                    {
                                        var confirmSend = false;

                                        var Senders = emailNotifiers.Value.ToArray();

                                        if (Senders.Length > 0)
                                        {
                                            StringBuilder StrMessage = new StringBuilder();
                                            StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", getForm.FormName, getForm.FormDescription, getForm.CreatedBy);
                                            StrMessage.AppendFormat("<p>Receiver: {0}</p>", String.Join(", ", Senders));
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - EForm Approval", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approval", String.Join(", ", Senders), getForm.FormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                confirmSend = true;
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approval", String.Join(", ", Senders), getForm.FormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }

                                            if (confirmSend)
                                            {
                                                var updateAppLvl = _context.FormListApprovalLevels.Where(x => x.Id == emailNotifiers.Key).FirstOrDefault();

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
                        }
                        else
                        {
                            getForm.FormStatus = "approved";
                            getForm.SubmittedDate = DateTime.Now;
                            getForm.SubmittedBy = user.DisplayName;
                            getForm.FormSubmittedData = this.FormList.FormSubmittedData;

                            if (getForm.JSON != null)
                            {
                                JObject modifiedJSON = JObject.Parse(getForm.JSON);
                                modifiedJSON["SubmittedDate"] = getForm.SubmittedDate;
                                modifiedJSON["SubmittedBy"] = getForm.SubmittedBy;
                                modifiedJSON["FormStatus"] = getForm.FormStatus;

                                getForm.JSON = modifiedJSON.ToString(Formatting.None);
                            }

                            _context.Attach(getForm);

                            _context.Entry(getForm).Property(x => x.SubmittedDate).IsModified = true;
                            _context.Entry(getForm).Property(x => x.SubmittedBy).IsModified = true;
                            _context.Entry(getForm).Property(x => x.FormStatus).IsModified = true;
                            _context.Entry(getForm).Property(x => x.JSON).IsModified = true;
                            _context.Entry(getForm).Property(x => x.FormSubmittedData).IsModified = true;

                            var formHistory = new FormListHistory();
                            formHistory.FormId = getForm.Id;
                            formHistory.FormName = getForm.FormName;
                            formHistory.FormDescription = getForm.FormDescription;
                            formHistory.FormData = getForm.FormData;
                            formHistory.FormSubmittedData = getForm.FormSubmittedData;
                            formHistory.FormStatus = getForm.FormStatus;
                            formHistory.FormRevision = getForm.FormRevision;
                            formHistory.Owner = getForm.Owner;
                            formHistory.OwnerCostCenter = getForm.OwnerCostCenter;
                            formHistory.RunningNumber = getForm.RunningNumber;
                            formHistory.MasterFormDetail = getForm.MasterFormDetail;
                            formHistory.CreatedDate = getForm.CreatedDate;
                            formHistory.CreatedBy = getForm.CreatedBy;
                            formHistory.ModifiedDate = getForm.ModifiedDate;
                            formHistory.ModifiedBy = getForm.ModifiedBy;
                            formHistory.SubmittedDate = getForm.SubmittedDate;
                            formHistory.SubmittedBy = getForm.SubmittedBy;
                            formHistory.ArchievedDate = DateTime.Now;
                            formHistory.JSON = getForm.JSON;

                            _context.FormListHistories.Add(formHistory);
                            await _context.SaveChangesAsync();

                            if (Environment.IsProduction())
                            {
                                string DomainName = _configuration["Domain:ServerURL"].ToString() + "/eRecord/Index";
                                string Sender = getForm.OwnerEmailAddress;

                                StringBuilder StrMessage = new StringBuilder();
                                StrMessage.AppendFormat("<p>Your form <b>{0} ({1})</b> is approved</p>", getForm.FormName, getForm.FormDescription);
                                StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, Sender);

                                if (emailNotificationResult == "Success")
                                {
                                    var logEmailNotification = new PublicLogEmailNotification();
                                    logEmailNotification.DateTime = DateTime.Now;
                                    logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approve notification", Sender, getForm.FormName);

                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                }
                                else
                                {
                                    var logEmailNotification = new PublicLogEmailNotification();
                                    logEmailNotification.DateTime = DateTime.Now;
                                    logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approve notification", Sender, getForm.FormName);

                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                }

                                await _context.SaveChangesAsync();
                            }
                            else if (Environment.IsDevelopment())
                            {
                                string DomainName = _configuration["Domain:ServerURL"].ToString() + "/eRecord/Index";
                                string Sender = getForm.OwnerEmailAddress;

                                StringBuilder StrMessage = new StringBuilder();
                                StrMessage.AppendFormat("<p>Your form <b>{0} ({1})</b> is approved</p>", getForm.FormName, getForm.FormDescription);
                                StrMessage.AppendFormat("<p>Receiver: {0}</p>", Sender);
                                StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                if (emailNotificationResult == "Success")
                                {
                                    var logEmailNotification = new PublicLogEmailNotification();
                                    logEmailNotification.DateTime = DateTime.Now;
                                    logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approve notification", Sender, getForm.FormName);

                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                }
                                else
                                {
                                    var logEmailNotification = new PublicLogEmailNotification();
                                    logEmailNotification.DateTime = DateTime.Now;
                                    logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approve notification", Sender, getForm.FormName);

                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                }

                                await _context.SaveChangesAsync();
                            }
                        }

                        databaseTran.Commit();

                        ViewData["Result"] = "submit";
                        return Page();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return Page();
        }
    }
}
