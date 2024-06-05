using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using IdentityApp.Pages.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;
using System.Text;
using WD_ERECORD_CORE.Function;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore.Storage;

namespace WD_ERECORD_CORE.Pages.EFormApproval
{
    public class TakeActionModel : EFormApprovalPageModel
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public FormListApprover FormListApprover { get; set; }

        [BindProperty]
        public int? FormId { get; set; }

        [BindProperty]
        public string FormName { get; set; }

        [BindProperty]
        public string FormDescription { get; set; }

        [BindProperty]
        public string CreatedBy { get; set; }

        public TakeActionModel(ApplicationDbContext context, IWebHostEnvironment _environment, IConfiguration configuration)
        {
            _context = context;
            Environment = _environment;
            _configuration = configuration;
        }

        public IActionResult OnGet(int? ApproverId)
        {
            this.FormListApprover = _context.FormListApprovers.Where(x => x.Id == ApproverId)
                .Include(x => x.FormListApprovalLevel).ThenInclude(x => x.FormList).FirstOrDefault();

            if (this.FormListApprover == null)
            {
                return NotFound();
            }

            // if this approval level is approved/ rejected redriect to....
            if (this.FormListApprover.FormListApprovalLevel.ApprovalStatus != "pending")
            {
                return RedirectToPage("ConfirmationPage", new { ApprovalLevelId = this.FormListApprover.FormListApprovalLevelId });
            }

            this.FormId = this.FormListApprover.FormListApprovalLevel.FormListId;
            this.FormName = this.FormListApprover.FormListApprovalLevel.FormList.FormName;
            this.FormDescription = this.FormListApprover.FormListApprovalLevel.FormList.FormDescription;
            this.CreatedBy = this.FormListApprover.FormListApprovalLevel.FormList.CreatedBy;

            return Page();
        }

        public async Task<IActionResult> OnPostActionAsync(string TakeAction) 
        {
            if (!String.IsNullOrEmpty(TakeAction)) 
            {
                var updateApprover = _context.FormListApprovers.Where(x => x.Id == this.FormListApprover.Id).Include(x => x.FormListApprovalLevel).ThenInclude(x => x.FormList).FirstOrDefault();
                IDictionary<int, List<string>> emailNotifiersInfo = new Dictionary<int, List<string>>();

                if (updateApprover == null)
                {
                    return NotFound();
                }

                if (updateApprover.FormListApprovalLevel.ApprovalStatus != "pending")
                {
                    return RedirectToPage("ConfirmationPage", new { ApprovalLevelId = this.FormListApprover.FormListApprovalLevelId });
                }
                else
                {
                    using (var databaseTran = _context.Database.BeginTransaction()) 
                    {
                        try
                        {
                            // update approver`s detail
                            updateApprover.ApproverStatus = (TakeAction == "Approve" ? "approved" : "rejected");

                            if (!String.IsNullOrEmpty(this.FormListApprover.Remark))
                            {
                                updateApprover.Remark = this.FormListApprover.Remark;
                            }

                            updateApprover.ApproveDate = DateTime.Now;

                            if (TakeAction == "Approve")
                            {
                                PublicLogApproval PA = new PublicLogApproval() { DateTime = DateTime.Now, UserId = updateApprover.EmployeeId, UserName = updateApprover.ApproverName, LogDetail = String.Format("User Form {0} approved by {1}", updateApprover.FormListApprovalLevel.FormList.FormName, updateApprover.ApproverName.ToString()) };
                                _context.PublicFormApprovals.Add(PA);
                            }
                            else if (TakeAction == "Reject")
                            {
                                PublicLogApproval PA = new PublicLogApproval() { DateTime = DateTime.Now, UserId = updateApprover.EmployeeId, UserName = updateApprover.ApproverName, LogDetail = String.Format("User Form {0} rejected by {1}", updateApprover.FormListApprovalLevel.FormList.FormName, updateApprover.ApproverName.ToString()) };
                                _context.PublicFormApprovals.Add(PA);
                            }

                            await _context.SaveChangesAsync();

                            _context.Entry(updateApprover).State = EntityState.Detached;
                            _context.Entry(updateApprover.FormListApprovalLevel).State = EntityState.Detached;
                            _context.Entry(updateApprover.FormListApprovalLevel.FormList).State = EntityState.Detached;

                            var approvalLevel = _context.FormListApprovalLevels.Where(x => x.Id == this.FormListApprover.FormListApprovalLevelId).Include(x => x.FormListApprovers).FirstOrDefault();

                            if (approvalLevel != null)
                            {
                                if (approvalLevel.ApprovalStatus == "pending")
                                {
                                    if (approvalLevel.ApproveCondition == "single")
                                    {
                                        var getAllApprovers = approvalLevel.FormListApprovers.Select(x => x.ApproverStatus).ToList();

                                        if (getAllApprovers.Contains("approved"))
                                        {
                                            approvalLevel.ApprovalStatus = "approved";

                                            _context.Attach(approvalLevel);
                                            _context.Entry(approvalLevel).Property(x => x.ApprovalStatus).IsModified = true;

                                            var nextApprovalLevel = _context.FormListApprovalLevels.Where(x => x.FormListId == this.FormId && x.ApprovalStatus == "none" && x.Id != approvalLevel.Id).Include(x => x.FormListApprovers).OrderBy(x => x.Id).FirstOrDefault();

                                            if (nextApprovalLevel != null)
                                            {
                                                nextApprovalLevel.ApprovalStatus = "pending";
                                                emailNotifiersInfo[nextApprovalLevel.Id] = new List<string>();

                                                if (nextApprovalLevel.FormListApprovers != null && nextApprovalLevel.FormListApprovers.Count() > 0)
                                                {
                                                    nextApprovalLevel.FormListApprovers.ToList().ForEach(x => { x.ApproverStatus = "pending"; emailNotifiersInfo[nextApprovalLevel.Id].Add(x.ApproverEmail); });
                                                }

                                                _context.FormListApprovalLevels.Update(nextApprovalLevel);
                                            }

                                            await _context.SaveChangesAsync();

                                            _context.Entry(approvalLevel).State = EntityState.Detached;
                                            foreach (var child in approvalLevel.FormListApprovers)
                                            {
                                                _context.Entry(child).State = EntityState.Detached;
                                            }

                                            if (nextApprovalLevel != null)
                                            {
                                                _context.Entry(nextApprovalLevel).State = EntityState.Detached;

                                                if (nextApprovalLevel.FormListApprovers != null && nextApprovalLevel.FormListApprovers.Count() > 0)
                                                {
                                                    foreach (var child in nextApprovalLevel.FormListApprovers)
                                                    {
                                                        _context.Entry(child).State = EntityState.Detached;
                                                    }
                                                }
                                            }
                                        }
                                        else if (getAllApprovers.Contains("rejected"))
                                        {
                                            approvalLevel.ApprovalStatus = "rejected";

                                            _context.Attach(approvalLevel);
                                            _context.Entry(approvalLevel).Property(x => x.ApprovalStatus).IsModified = true;

                                            var restApprovalLevels = _context.FormListApprovalLevels.Where(x => x.FormListId == this.FormId && x.ApprovalStatus == "none" && x.Id != approvalLevel.Id).ToList();

                                            if (restApprovalLevels != null && restApprovalLevels.Count() > 0)
                                            {
                                                restApprovalLevels.ForEach(x => x.ApprovalStatus = "rejected");

                                                _context.FormListApprovalLevels.UpdateRange(restApprovalLevels);
                                            }

                                            await _context.SaveChangesAsync();

                                            _context.Entry(approvalLevel).State = EntityState.Detached;
                                            foreach (var child in approvalLevel.FormListApprovers)
                                            {
                                                _context.Entry(child).State = EntityState.Detached;
                                            }

                                            if (restApprovalLevels != null && restApprovalLevels.Count() > 0)
                                            {
                                                foreach (var child in restApprovalLevels)
                                                {
                                                    _context.Entry(child).State = EntityState.Detached;
                                                }
                                            }
                                        }
                                    }
                                    else if (approvalLevel.ApproveCondition == "all")
                                    {
                                        var getAllApprovers = approvalLevel.FormListApprovers.Select(x => x.ApproverStatus).ToList();

                                        static bool EqualToApprove(string Status)
                                        {
                                            return Status == "approved";
                                        }

                                        if (getAllApprovers.Contains("rejected"))
                                        {
                                            approvalLevel.ApprovalStatus = "rejected";

                                            _context.Attach(approvalLevel);
                                            _context.Entry(approvalLevel).Property(x => x.ApprovalStatus).IsModified = true;

                                            var restApprovalLevels = _context.FormListApprovalLevels.Where(x => x.FormListId == this.FormId && x.ApprovalStatus == "none" && x.Id != approvalLevel.Id).ToList();

                                            if (restApprovalLevels != null && restApprovalLevels.Count() > 0)
                                            {
                                                restApprovalLevels.ForEach(x => x.ApprovalStatus = "rejected");

                                                _context.FormListApprovalLevels.UpdateRange(restApprovalLevels);
                                            }

                                            await _context.SaveChangesAsync();

                                            _context.Entry(approvalLevel).State = EntityState.Detached;
                                            foreach (var child in approvalLevel.FormListApprovers)
                                            {
                                                _context.Entry(child).State = EntityState.Detached;
                                            }

                                            if (restApprovalLevels != null && restApprovalLevels.Count() > 0)
                                            {
                                                foreach (var child in restApprovalLevels)
                                                {
                                                    _context.Entry(child).State = EntityState.Detached;
                                                }
                                            }
                                        }
                                        else if (getAllApprovers.TrueForAll(EqualToApprove))
                                        {
                                            approvalLevel.ApprovalStatus = "approved";

                                            _context.Attach(approvalLevel);
                                            _context.Entry(approvalLevel).Property(x => x.ApprovalStatus).IsModified = true;

                                            var nextApprovalLevel = _context.FormListApprovalLevels.Where(x => x.FormListId == this.FormId && x.ApprovalStatus == "none" && x.Id != approvalLevel.Id).Include(x => x.FormListApprovers).OrderBy(x => x.Id).FirstOrDefault();

                                            if (nextApprovalLevel != null)
                                            {
                                                nextApprovalLevel.ApprovalStatus = "pending";
                                                emailNotifiersInfo[nextApprovalLevel.Id] = new List<string>();

                                                if (nextApprovalLevel.FormListApprovers != null && nextApprovalLevel.FormListApprovers.Count() > 0)
                                                {
                                                    nextApprovalLevel.FormListApprovers.ToList().ForEach(x => { x.ApproverStatus = "pending"; emailNotifiersInfo[nextApprovalLevel.Id].Add(x.ApproverEmail); });
                                                }

                                                _context.FormListApprovalLevels.Update(nextApprovalLevel);
                                            }

                                            await _context.SaveChangesAsync();

                                            _context.Entry(approvalLevel).State = EntityState.Detached;
                                            foreach (var child in approvalLevel.FormListApprovers)
                                            {
                                                _context.Entry(child).State = EntityState.Detached;
                                            }

                                            if (nextApprovalLevel != null)
                                            {
                                                _context.Entry(nextApprovalLevel).State = EntityState.Detached;

                                                if (nextApprovalLevel.FormListApprovers != null && nextApprovalLevel.FormListApprovers.Count() > 0)
                                                {
                                                    foreach (var child in nextApprovalLevel.FormListApprovers)
                                                    {
                                                        _context.Entry(child).State = EntityState.Detached;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            var validateForm = _context.FormLists.Include(x => x.FormListApprovalLevels).ThenInclude(x => x.FormListApprovers).Where(x => x.Id == this.FormId).FirstOrDefault();

                            if (validateForm != null)
                            {
                                if (validateForm.FormStatus == "pending")
                                {
                                    if (validateForm.FormListApprovalLevels != null && validateForm.FormListApprovalLevels.Count() > 0)
                                    {
                                        var allApprovalLevels = validateForm.FormListApprovalLevels.Select(x => x.ApprovalStatus).ToList();

                                        if (!allApprovalLevels.Contains("none") && !allApprovalLevels.Contains("pending"))
                                        {
                                            static bool EqualToApprove(string Status)
                                            {
                                                return Status == "approved";
                                            }

                                            if (allApprovalLevels.TrueForAll(EqualToApprove))
                                            {
                                                validateForm.FormStatus = "approved";
                                                validateForm.ExpiredDate = DateTime.Now.AddDays(30);

                                                var json = JsonConvert.SerializeObject(validateForm, new JsonSerializerSettings
                                                {
                                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                });

                                                JObject modifiedJSON = JObject.Parse(json);
                                                modifiedJSON["FormData"] = "";
                                                modifiedJSON["FormSubmittedData"] = "";
                                                modifiedJSON["JSON"] = "";

                                                validateForm.JSON = modifiedJSON.ToString(Formatting.None);

                                                _context.Attach(validateForm);
                                                _context.Entry(validateForm).Property(x => x.FormStatus).IsModified = true;
                                                _context.Entry(validateForm).Property(x => x.ExpiredDate).IsModified = true;
                                                _context.Entry(validateForm).Property(x => x.JSON).IsModified = true;

                                                // add form history
                                                var formHistory = new FormListHistory();
                                                formHistory.FormId = validateForm.Id;
                                                formHistory.FormName = validateForm.FormName;
                                                formHistory.FormDescription = validateForm.FormDescription;
                                                formHistory.FormData = validateForm.FormData;
                                                formHistory.FormSubmittedData = validateForm.FormSubmittedData;
                                                formHistory.FormStatus = validateForm.FormStatus;
                                                formHistory.FormRevision = validateForm.FormRevision;
                                                formHistory.Owner = validateForm.Owner;
                                                formHistory.OwnerCostCenter = validateForm.OwnerCostCenter;
                                                formHistory.RunningNumber = validateForm.RunningNumber;
                                                formHistory.MasterFormDetail = validateForm.MasterFormDetail;
                                                formHistory.CreatedDate = validateForm.CreatedDate;
                                                formHistory.CreatedBy = validateForm.CreatedBy;
                                                formHistory.ModifiedDate = validateForm.ModifiedDate;
                                                formHistory.ModifiedBy = validateForm.ModifiedBy;
                                                formHistory.SubmittedDate = validateForm.SubmittedDate;
                                                formHistory.SubmittedBy = validateForm.SubmittedBy;
                                                formHistory.ArchievedDate = DateTime.Now;
                                                formHistory.JSON = modifiedJSON.ToString(Formatting.None);

                                                _context.FormListHistories.Add(formHistory);


                                                if (Environment.IsProduction())
                                                {
                                                    string DomainName = _configuration["Domain:ServerURL"].ToString() + "/eRecord/Index";
                                                    string Sender = validateForm.OwnerEmailAddress;

                                                    StringBuilder StrMessage = new StringBuilder();
                                                    StrMessage.AppendFormat("<p>Your form <b>{0} ({1})</b> is approved</p>", validateForm.FormName, validateForm.FormDescription);
                                                    StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                                    var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, Sender);

                                                    if (emailNotificationResult == "Success")
                                                    {
                                                        var logEmailNotification = new PublicLogEmailNotification();
                                                        logEmailNotification.DateTime = DateTime.Now;
                                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approve notification", Sender, validateForm.FormName);

                                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                    }
                                                    else
                                                    {
                                                        var logEmailNotification = new PublicLogEmailNotification();
                                                        logEmailNotification.DateTime = DateTime.Now;
                                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approve notification", Sender, validateForm.FormName);

                                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                    }
                                                }
                                                else if (Environment.IsDevelopment())
                                                {
                                                    string DomainName = _configuration["Domain:ServerURL"].ToString() + "/eRecord/Index";
                                                    string Sender = validateForm.OwnerEmailAddress;

                                                    StringBuilder StrMessage = new StringBuilder();
                                                    StrMessage.AppendFormat("<p>Your form <b>{0} ({1})</b> is approved</p>", validateForm.FormName, validateForm.FormDescription);
                                                    StrMessage.AppendFormat("<p>Receiver: {0}</p>", Sender);
                                                    StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                                    var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                                    if (emailNotificationResult == "Success")
                                                    {
                                                        var logEmailNotification = new PublicLogEmailNotification();
                                                        logEmailNotification.DateTime = DateTime.Now;
                                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approve notification", Sender, validateForm.FormName);

                                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                    }
                                                    else
                                                    {
                                                        var logEmailNotification = new PublicLogEmailNotification();
                                                        logEmailNotification.DateTime = DateTime.Now;
                                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approve notification", Sender, validateForm.FormName);

                                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                    }
                                                }

                                                await _context.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                validateForm.FormStatus = "rejected";
                                                validateForm.ExpiredDate = DateTime.Now.AddDays(30);

                                                var json = JsonConvert.SerializeObject(validateForm, new JsonSerializerSettings
                                                {
                                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                });

                                                JObject modifiedJSON = JObject.Parse(json);
                                                modifiedJSON["FormData"] = "";
                                                modifiedJSON["FormSubmittedData"] = "";
                                                modifiedJSON["JSON"] = "";

                                                validateForm.JSON = modifiedJSON.ToString(Formatting.None);

                                                _context.Attach(validateForm);
                                                _context.Entry(validateForm).Property(x => x.FormStatus).IsModified = true;
                                                _context.Entry(validateForm).Property(x => x.ExpiredDate).IsModified = true;
                                                _context.Entry(validateForm).Property(x => x.JSON).IsModified = true;

                                                // add form history
                                                var formHistory = new FormListHistory();
                                                formHistory.FormId = validateForm.Id;
                                                formHistory.FormName = validateForm.FormName;
                                                formHistory.FormDescription = validateForm.FormDescription;
                                                formHistory.FormData = validateForm.FormData;
                                                formHistory.FormSubmittedData = validateForm.FormSubmittedData;
                                                formHistory.FormStatus = validateForm.FormStatus;
                                                formHistory.FormRevision = validateForm.FormRevision;
                                                formHistory.Owner = validateForm.Owner;
                                                formHistory.OwnerCostCenter = validateForm.OwnerCostCenter;
                                                formHistory.RunningNumber = validateForm.RunningNumber;
                                                formHistory.MasterFormDetail = validateForm.MasterFormDetail;
                                                formHistory.CreatedDate = validateForm.CreatedDate;
                                                formHistory.CreatedBy = validateForm.CreatedBy;
                                                formHistory.ModifiedDate = validateForm.ModifiedDate;
                                                formHistory.ModifiedBy = validateForm.ModifiedBy;
                                                formHistory.SubmittedDate = validateForm.SubmittedDate;
                                                formHistory.SubmittedBy = validateForm.SubmittedBy;
                                                formHistory.ArchievedDate = DateTime.Now;
                                                formHistory.JSON = modifiedJSON.ToString(Formatting.None);

                                                _context.FormListHistories.Add(formHistory);

                                                if (Environment.IsProduction())
                                                {
                                                    string DomainName = _configuration["Domain:ServerURL"].ToString() + "/eRecord/Index";
                                                    string Sender = validateForm.OwnerEmailAddress;

                                                    StringBuilder StrMessage = new StringBuilder();
                                                    StrMessage.AppendFormat("<p>Your form <b>{0} ({1})</b> is rejected</p>", validateForm.FormName, validateForm.FormDescription);
                                                    StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                                    var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, Sender);

                                                    if (emailNotificationResult == "Success")
                                                    {
                                                        var logEmailNotification = new PublicLogEmailNotification();
                                                        logEmailNotification.DateTime = DateTime.Now;
                                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) reject notification", Sender, validateForm.FormName);

                                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                    }
                                                    else
                                                    {
                                                        var logEmailNotification = new PublicLogEmailNotification();
                                                        logEmailNotification.DateTime = DateTime.Now;
                                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) reject notification", Sender, validateForm.FormName);

                                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                    }
                                                }
                                                else if (Environment.IsDevelopment())
                                                {
                                                    string DomainName = _configuration["Domain:ServerURL"].ToString() + "/eRecord/Index";
                                                    string Sender = validateForm.OwnerEmailAddress;

                                                    StringBuilder StrMessage = new StringBuilder();
                                                    StrMessage.AppendFormat("<p>Your form <b>{0} ({1})</b> is rejected</p>", validateForm.FormName, validateForm.FormDescription);
                                                    StrMessage.AppendFormat("<p>Receiver: {0}</p>", Sender);
                                                    StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                                    var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                                    if (emailNotificationResult == "Success")
                                                    {
                                                        var logEmailNotification = new PublicLogEmailNotification();
                                                        logEmailNotification.DateTime = DateTime.Now;
                                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) reject notification", Sender, validateForm.FormName);

                                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                    }
                                                    else
                                                    {
                                                        var logEmailNotification = new PublicLogEmailNotification();
                                                        logEmailNotification.DateTime = DateTime.Now;
                                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) reject notification", Sender, validateForm.FormName);

                                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                                    }
                                                }

                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            var json = JsonConvert.SerializeObject(validateForm, new JsonSerializerSettings
                                            {
                                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                            });

                                            JObject modifiedJSON = JObject.Parse(json);
                                            modifiedJSON["FormData"] = "";
                                            modifiedJSON["FormSubmittedData"] = "";
                                            modifiedJSON["JSON"] = "";

                                            validateForm.JSON = modifiedJSON.ToString(Formatting.None);

                                            _context.Attach(validateForm);
                                            _context.Entry(validateForm).Property(x => x.JSON).IsModified = true;

                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                }
                            }

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
                                            StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", this.FormName, this.FormDescription, this.CreatedBy);
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - EForm Approval", StrMessage.ToString(), Senders, null);

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approval", String.Join(", ", Senders), this.FormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                confirmSend = true;
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approval", String.Join(", ", Senders), this.FormName);

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
                                            StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", this.FormName, this.FormDescription, this.CreatedBy);
                                            StrMessage.AppendFormat("<p>Receiver: {0}</p>", String.Join(", ", Senders));
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - EForm Approval", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for form ({1}) approval", String.Join(", ", Senders), this.FormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                confirmSend = true;
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for form ({1}) approval", String.Join(", ", Senders), this.FormName);

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

                            databaseTran.Commit();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
