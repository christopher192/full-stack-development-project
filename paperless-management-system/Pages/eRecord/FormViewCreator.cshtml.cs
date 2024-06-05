using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;
using NuGet.Protocol.Plugins;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.Function;

namespace WD_ERECORD_CORE.Pages.eRecord
{
/*    [IgnoreAntiforgeryToken(Order = 1001)]*/
    public class FormViewCreatorModel : eRecordPageModel
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
        public MasterFormList MasterForm { get; set; }

        [BindProperty]
        public UserInfo userInfo { get; set; } = new UserInfo();

        [BindProperty]
        public string? FormSubmittedData { get; set; }

        [BindProperty]
        public string? SubmittedType { get; set; }

        [BindProperty]
        public string? ReturnURL { get; set; }

        [BindProperty]
        public int? MasterFormId { get; set; }

        public FormViewCreatorModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment _environment, IConfiguration configuration)
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

        public IActionResult OnGet(int? MasterFormId, string ReturnURL)
        {
            if (MasterFormId == null || String.IsNullOrEmpty(ReturnURL))
            {
                return NotFound();
            }

            this.MasterForm = _context.MasterFormLists.Where(x => x.Id == MasterFormId).FirstOrDefault();
            this.MasterFormId = MasterForm.Id;

            if (this.MasterForm == null)
            {
                return NotFound();
            }

            this.ReturnURL = ReturnURL;

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

            var user = await GetCurrentUser();
            var x = this.MasterFormId;

            var getMasterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterForm.Id).FirstOrDefault();
            IDictionary<int, List<string>> emailNotifiersInfo = new Dictionary<int, List<string>>();

            if (getMasterForm == null || getMasterForm.MasterFormStatus != "active")
            {
                return NotFound();
            }

            using (var databaseTran = _context.Database.BeginTransaction()) 
            {
                try
                {
                    if (!String.IsNullOrEmpty(this.SubmittedType) && this.SubmittedType == "Update")
                    {
                        FormList formList = new FormList();
                        formList.MasterFormId = getMasterForm.Id;
                        formList.FormName = getMasterForm.MasterFormName;
                        formList.FormStatus = "new";
                        formList.FormDescription = getMasterForm.MasterFormDescription;
                        formList.GuidelineFile = getMasterForm.GuidelineFile;
                        formList.UniqueGuidelineFile = getMasterForm.UniqueGuidelineFile;
                        formList.FormRevision = getMasterForm.MasterFormRevision;
                        formList.FormData = getMasterForm.MasterFormData;
                        formList.FormSubmittedData = this.FormSubmittedData;
                        formList.Owner = user.UserName;
                        formList.OwnerEmailAddress = user.Email;
                        formList.CreatedDate = DateTime.Now;
                        formList.CreatedBy = user.DisplayName;
                        formList.OwnerCostCenter = !string.IsNullOrEmpty(user.CostCenterName) ? user.CostCenterName : "unavailable";
                        formList.RunningNumber = getMasterForm.RunningNumber;
                        formList.MasterFormDetail = JsonConvert.SerializeObject(new MasterFormDetail() { MasterFormOwner = getMasterForm.Owner, MasterFormOwnerDepartment = getMasterForm.OwnerCostCenter });

                        var desFormListApproval = JsonConvert.DeserializeObject<FormApproval>(getMasterForm.FormApprovalJSON);
                        // var formApproval = desFormListApproval.FixFormApproval;
                        var formApproval = desFormListApproval.EditableFormApproval;

                        if (formApproval != null && formApproval.Count() > 0)
                        {
                            var formListApprovalLevelsList = new List<FormListApprovalLevel>();

                            foreach (var formApprovaLevel in formApproval.OrderBy(x => x.Id))
                            {
                                var formListApprovalLevel = new FormListApprovalLevel();
                                formListApprovalLevel.FormListApprovers = new List<FormListApprover>();
                                formListApprovalLevel.EmailReminder = formApprovaLevel.EmailReminder;
                                formListApprovalLevel.ReminderType = formApprovaLevel.ReminderType;
                                formListApprovalLevel.NotificationType = formApprovaLevel.NotificationType;
                                formListApprovalLevel.ApproveCondition = formApprovaLevel.ApproveCondition;
                                formListApprovalLevel.LastSend = null;

                                if (formApprovaLevel.FormApprovers.Count() == 0 && formApprovaLevel.NotificationType == "By Superior")
                                {
                                    // get manager id from user
                                    var getSuperiorInfo = _context.Users.Where(x => x.UserName == user.ManagerId).FirstOrDefault();

                                    if (getSuperiorInfo != null)
                                    {
                                        var FormListApprover = new FormListApprover();
                                        FormListApprover.ApproverEmail = getSuperiorInfo.Email;
                                        FormListApprover.ApproverName = getSuperiorInfo.DisplayName;
                                        FormListApprover.EmployeeId = getSuperiorInfo.UserName;

                                        formListApprovalLevel.FormListApprovers.Add(FormListApprover);
                                    }
                                }
                                else
                                {
                                    foreach (var formApprover in formApprovaLevel.FormApprovers.OrderBy(x => x.Id))
                                    {
                                        var formListApprover = new FormListApprover();
                                        formListApprover.ApproverEmail = formApprover.ApproverEmail;
                                        formListApprover.ApproverName = formApprover.ApproverName;
                                        formListApprover.EmployeeId = formApprover.EmployeeId;

                                        formListApprovalLevel.FormListApprovers.Add(formListApprover);
                                    }
                                }

                                formListApprovalLevelsList.Add(formListApprovalLevel);
                            }

                            formList.FormListApprovalLevels = formListApprovalLevelsList;
                        }

                        var json = JsonConvert.SerializeObject(formList, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        JObject modifiedJSON = JObject.Parse(json);
                        modifiedJSON["FormData"] = "";
                        modifiedJSON["FormSubmittedData"] = "";

                        formList.JSON = modifiedJSON.ToString(Formatting.None);

                        _context.FormLists.Add(formList);

                        getMasterForm.RunningNumber = getMasterForm.RunningNumber + 1;

                        _context.Attach(getMasterForm);
                        _context.Entry(getMasterForm).Property(r => r.RunningNumber).IsModified = true;

                        await _context.SaveChangesAsync();

                        databaseTran.Commit();

                        ViewData["Result"] = "submit";
                        return Page();
                    }
                    else if (!String.IsNullOrEmpty(this.SubmittedType) && this.SubmittedType == "Submit")
                    {
                        FormList formList = new FormList();
                        formList.MasterFormId = getMasterForm.Id;
                        formList.FormName = getMasterForm.MasterFormName;
                        formList.FormDescription = getMasterForm.MasterFormDescription;
                        formList.GuidelineFile = getMasterForm.GuidelineFile;
                        formList.UniqueGuidelineFile = getMasterForm.UniqueGuidelineFile;
                        formList.FormRevision = getMasterForm.MasterFormRevision;
                        formList.FormData = getMasterForm.MasterFormData;
                        formList.FormSubmittedData = this.FormSubmittedData;
                        formList.Owner = user.UserName;
                        formList.OwnerEmailAddress = user.Email;
                        formList.CreatedDate = DateTime.Now;
                        formList.CreatedBy = user.DisplayName;
                        formList.SubmittedDate = formList.CreatedDate;
                        formList.SubmittedBy = user.DisplayName;
                        formList.OwnerCostCenter = !string.IsNullOrEmpty(user.CostCenterName) ? user.CostCenterName : "unavailable";
                        formList.RunningNumber = getMasterForm.RunningNumber;
                        formList.MasterFormDetail = JsonConvert.SerializeObject(new MasterFormDetail() { MasterFormOwner = getMasterForm.Owner, MasterFormOwnerDepartment = getMasterForm.OwnerCostCenter });

                        var desFormListApproval = JsonConvert.DeserializeObject<FormApproval>(getMasterForm.FormApprovalJSON);
                        // var formApproval = desFormListApproval.FixFormApproval;
                        var formApproval = desFormListApproval.EditableFormApproval;

                        if (formApproval.Count() > 0)
                        {
                            var formListApprovalLevelsList = new List<FormListApprovalLevel>();

                            foreach (var formApprovaLevel in formApproval.OrderBy(x => x.Id))
                            {
                                var formListApprovalLevel = new FormListApprovalLevel();
                                formListApprovalLevel.FormListApprovers = new List<FormListApprover>();
                                formListApprovalLevel.EmailReminder = formApprovaLevel.EmailReminder;
                                formListApprovalLevel.ReminderType = formApprovaLevel.ReminderType;
                                formListApprovalLevel.NotificationType = formApprovaLevel.NotificationType;
                                formListApprovalLevel.ApproveCondition = formApprovaLevel.ApproveCondition;
                                formListApprovalLevel.LastSend = null;

                                if (formApprovaLevel.FormApprovers.Count() == 0 && formApprovaLevel.NotificationType == "By Superior")
                                {
                                    // get manager id from user
                                    var getSuperiorInfo = _context.Users.Where(x => x.UserName == user.ManagerId).FirstOrDefault();

                                    if (getSuperiorInfo != null)
                                    {
                                        var FormListApprover = new FormListApprover();
                                        FormListApprover.ApproverEmail = getSuperiorInfo.Email;
                                        FormListApprover.ApproverName = getSuperiorInfo.DisplayName;
                                        FormListApprover.EmployeeId = getSuperiorInfo.UserName;

                                        formListApprovalLevel.FormListApprovers.Add(FormListApprover);
                                    }
                                }
                                else
                                {
                                    foreach (var formApprover in formApprovaLevel.FormApprovers.OrderBy(x => x.Id))
                                    {
                                        var formListApprover = new FormListApprover();
                                        formListApprover.ApproverEmail = formApprover.ApproverEmail;
                                        formListApprover.ApproverName = formApprover.ApproverName;
                                        formListApprover.EmployeeId = formApprover.EmployeeId;

                                        formListApprovalLevel.FormListApprovers.Add(formListApprover);
                                    }
                                }

                                formListApprovalLevelsList.Add(formListApprovalLevel);
                            }

                            formListApprovalLevelsList.FirstOrDefault().ApprovalStatus = "pending";
                            formListApprovalLevelsList.FirstOrDefault().FormListApprovers.ToList().ForEach(x => { x.ApproverStatus = "pending"; });

                            formList.FormListApprovalLevels = formListApprovalLevelsList;
                            formList.FormStatus = "pending";
                        }
                        else
                        {
                            formList.FormStatus = "approved";
                        }

                        var json = JsonConvert.SerializeObject(formList, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        JObject modifiedJSON = JObject.Parse(json);
                        modifiedJSON["FormData"] = "";
                        modifiedJSON["FormSubmittedData"] = "";

                        formList.JSON = modifiedJSON.ToString(Formatting.None);

                        _context.FormLists.Add(formList);

                        getMasterForm.RunningNumber = getMasterForm.RunningNumber + 1;

                        _context.Attach(getMasterForm);
                        _context.Entry(getMasterForm).Property(r => r.RunningNumber).IsModified = true;

                        await _context.SaveChangesAsync();

                        if (formList.FormStatus == "approved")
                        {
                            var formHistory = new FormListHistory();
                            formHistory.FormId = formList.Id;
                            formHistory.FormName = formList.FormName;
                            formHistory.FormDescription = formList.FormDescription;
                            formHistory.FormData = formList.FormData;
                            formHistory.FormSubmittedData = formList.FormSubmittedData;
                            formHistory.FormStatus = formList.FormStatus;
                            formHistory.FormRevision = formList.FormRevision;
                            formHistory.Owner = formList.Owner;
                            formHistory.OwnerCostCenter = formList.OwnerCostCenter;
                            formHistory.RunningNumber = formList.RunningNumber;
                            formHistory.MasterFormDetail = formList.MasterFormDetail;
                            formHistory.CreatedDate = formList.CreatedDate;
                            formHistory.CreatedBy = formList.CreatedBy;
                            formHistory.ModifiedDate = formList.ModifiedDate;
                            formHistory.ModifiedBy = formList.ModifiedBy;
                            formHistory.SubmittedDate = formList.SubmittedDate;
                            formHistory.SubmittedBy = formList.SubmittedBy;
                            formHistory.ArchievedDate = DateTime.Now;
                            formHistory.JSON = formList.JSON;

                            _context.FormListHistories.Add(formHistory);

                            if (Environment.IsProduction())
                            {
                                string DomainName = _configuration["Domain:ServerURL"].ToString() + "/eRecord/Index";
                                string Sender = formList.OwnerEmailAddress;

                                StringBuilder StrMessage = new StringBuilder();
                                StrMessage.AppendFormat("<p>Your form <b>{0} ({1})</b> is approved</p>", formList.FormName, formList.FormDescription);
                                StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, Sender);

                                if (emailNotificationResult == "Success")
                                {
                                    var logEmailNotification = new PublicLogEmailNotification();
                                    logEmailNotification.DateTime = DateTime.Now;
                                    logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approve notification", Sender, formList.FormName);

                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                }
                                else
                                {
                                    var logEmailNotification = new PublicLogEmailNotification();
                                    logEmailNotification.DateTime = DateTime.Now;
                                    logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approve notification", Sender, formList.FormName);

                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                }
                            }
                            else if (Environment.IsDevelopment())
                            {
                                string DomainName = _configuration["Domain:ServerURL"].ToString() + "/eRecord/Index";
                                string Sender = formList.OwnerEmailAddress;

                                StringBuilder StrMessage = new StringBuilder();
                                StrMessage.AppendFormat("<p>Your form <b>{0} ({1})</b> is approved</p>", formList.FormName, formList.FormDescription);
                                StrMessage.AppendFormat("<p>Receiver: {0}</p>", Sender);
                                StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                if (emailNotificationResult == "Success")
                                {
                                    var logEmailNotification = new PublicLogEmailNotification();
                                    logEmailNotification.DateTime = DateTime.Now;
                                    logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approve notification", Sender, formList.FormName);

                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                }
                                else
                                {
                                    var logEmailNotification = new PublicLogEmailNotification();
                                    logEmailNotification.DateTime = DateTime.Now;
                                    logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approve notification", Sender, formList.FormName);

                                    _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                }
                            }

                            await _context.SaveChangesAsync();
                        }
                        else if (formList.FormStatus == "pending")
                        {
                            if (Environment.IsProduction())
                            {
                                var updateApprovalLevel = formList.FormListApprovalLevels.Where(x => x.ApprovalStatus == "pending").FirstOrDefault();

                                if (updateApprovalLevel != null)
                                {
                                    emailNotifiersInfo[updateApprovalLevel.Id] = new List<string>();

                                    if (updateApprovalLevel.FormListApprovers.ToList().Count() > 0)
                                    {
                                        foreach (var approver in updateApprovalLevel.FormListApprovers.ToList())
                                        {
                                            emailNotifiersInfo[updateApprovalLevel.Id].Add(approver.ApproverEmail);
                                        }
                                    }
                                    // emailNotifiersInfo[updateApprovalLevel.Id].Add("christopher@sophicautomation.com");
                                }

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
                                            StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", formList.FormName, formList.FormDescription, formList.CreatedBy);
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - EForm Approval", StrMessage.ToString(), Senders, null);

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approval", String.Join(", ", Senders), formList.FormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                confirmSend = true;
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approval", String.Join(", ", Senders), formList.FormName);

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
                                var updateApprovalLevel = formList.FormListApprovalLevels.Where(x => x.ApprovalStatus == "pending").FirstOrDefault();

                                if (updateApprovalLevel != null)
                                {
                                    emailNotifiersInfo[updateApprovalLevel.Id] = new List<string>();

                                    if (updateApprovalLevel.FormListApprovers.ToList().Count() > 0)
                                    {
                                        foreach (var approver in updateApprovalLevel.FormListApprovers.ToList())
                                        {
                                            emailNotifiersInfo[updateApprovalLevel.Id].Add(approver.ApproverEmail);
                                        }
                                    }

                                }

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
                                            StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", formList.FormName, formList.FormDescription, formList.CreatedBy);
                                            StrMessage.AppendFormat("<p>Receiver: {0}</p>", String.Join(", ", Senders));
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - EForm Approval", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approval", String.Join(", ", Senders), formList.FormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                confirmSend = true;
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approval", String.Join(", ", Senders), formList.FormName);

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
