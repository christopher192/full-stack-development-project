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
using Newtonsoft.Json.Linq;
using IdentityApp.Pages.Identity;
using WD_ERECORD_CORE.Function;
using System.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace WD_ERECORD_CORE.Pages.MasterFormCCBApproval
{
    public class TakeActionModel : MasterFormCCBApprovalPageModel
    {
        private readonly ApplicationDbContext _context;

        private IWebHostEnvironment Environment;

        private readonly IConfiguration _configuration;

        [BindProperty]
        public MasterFormCCBApprover MasterFormCCBApprover { get; set; }

        [BindProperty]
        public int MasterFormId { get; set; }

        [BindProperty]
        public string MasterFormName { get; set; }

        [BindProperty]
        public string MasterFormDescription { get; set; }

        [BindProperty]
        public string MasterFormCreatedBy { get; set; }

        public TakeActionModel(ApplicationDbContext context, IWebHostEnvironment _environment, IConfiguration configuration)
        {
            _context = context;
            Environment = _environment;
            _configuration = configuration;
        }

        public IActionResult OnGet(int? ApproverId)
        {
            if (ApproverId == null)
            {
                return NotFound();
            }

            this.MasterFormCCBApprover = _context.MasterFormCCBApprovers.Where(x => x.Id == ApproverId)
                .Include(x => x.MasterFormCCBApprovalLevel).ThenInclude(x => x.MasterFormDepartment).ThenInclude(x => x.MasterFormList).FirstOrDefault();

            // if this approval level is approved/ rejected redriect to....
            if (MasterFormCCBApprover.MasterFormCCBApprovalLevel.ApprovalStatus != "pending")
            {
                return RedirectToPage("ConfirmationPage", new { ApprovalLevelId = this.MasterFormCCBApprover.MasterFormCCBApprovalLevelId });
            }

            this.MasterFormId = this.MasterFormCCBApprover.MasterFormCCBApprovalLevel.MasterFormDepartment.MasterFormListId ?? -1;
            this.MasterFormName = this.MasterFormCCBApprover.MasterFormCCBApprovalLevel.MasterFormDepartment.MasterFormList.MasterFormName;
            this.MasterFormDescription = this.MasterFormCCBApprover.MasterFormCCBApprovalLevel.MasterFormDepartment.MasterFormList.MasterFormDescription;
            this.MasterFormCreatedBy = this.MasterFormCCBApprover.MasterFormCCBApprovalLevel.MasterFormDepartment.MasterFormList.CreatedBy;

            return Page();
        }

        public async Task<IActionResult> OnPostAction(string TakeAction)
        {
            if (!String.IsNullOrEmpty(TakeAction))
            {
                var updateApprover = _context.MasterFormCCBApprovers.Where(x => x.Id == this.MasterFormCCBApprover.Id).Include(x => x.MasterFormCCBApprovalLevel).ThenInclude(x => x.MasterFormDepartment).FirstOrDefault();
                IDictionary<int, List<string>> emailNotifiersInfo = new Dictionary<int, List<string>>();

                if (updateApprover == null)
                {
                    return NotFound();
                }

                // get department id and approval level id
                var departmentId = updateApprover.MasterFormCCBApprovalLevel.MasterFormDepartmentId;
                var approvalLevelId = updateApprover.MasterFormCCBApprovalLevelId;

                // if this approval level is approved/ rejected redriect to....
                if (updateApprover.MasterFormCCBApprovalLevel.ApprovalStatus != "pending")
                {
                    return RedirectToPage("ConfirmationPage", new { ApprovalLevelId = this.MasterFormCCBApprover.MasterFormCCBApprovalLevelId });
                }
                else 
                {
                    using (var databaseTran = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            // update approver`s detail
                            updateApprover.ApproverStatus = (TakeAction == "Approve" ? "approved" : "rejected");

                            if (!String.IsNullOrEmpty(this.MasterFormCCBApprover.Remark))
                            {
                                updateApprover.Remark = this.MasterFormCCBApprover.Remark;
                            }

                            updateApprover.ApproveDate = DateTime.Now;

                            _context.MasterFormCCBApprovers.Update(updateApprover);

                            // save approver`s approve/ reject data to history
                            if (TakeAction == "Approve")
                            {
                                PublicLogApproval PA = new PublicLogApproval() { DateTime = DateTime.Now, UserId = updateApprover.EmployeeId, UserName = updateApprover.ApproverName, LogDetail = String.Format("Master Form {0} approved by {1}", this.MasterFormName, updateApprover.ApproverName.ToString()) };
                                _context.PublicFormApprovals.Add(PA);
                            }
                            else if (TakeAction == "Reject")
                            {
                                PublicLogApproval PA = new PublicLogApproval() { DateTime = DateTime.Now, UserId = updateApprover.EmployeeId, UserName = updateApprover.ApproverName, LogDetail = String.Format("Master Form {0} rejected by {1}", this.MasterFormName, updateApprover.ApproverName.ToString()) };
                                _context.PublicFormApprovals.Add(PA);
                            }

                            await _context.SaveChangesAsync();

                            _context.Entry(updateApprover).State = EntityState.Detached;
                            _context.Entry(updateApprover.MasterFormCCBApprovalLevel).State = EntityState.Detached;
                            _context.Entry(updateApprover.MasterFormCCBApprovalLevel.MasterFormDepartment).State = EntityState.Detached;

                            var approvalLevel = _context.MasterFormCCBApprovalLevels.Where(x => x.Id == approvalLevelId).Include(x => x.MasterFormCCBApprovers).FirstOrDefault();

                            if (approvalLevel != null)
                            {
                                if (approvalLevel.ApprovalStatus == "pending")
                                {
                                    // update approval status based on approval conditon
                                    // execute if approval condition is 'single'
                                    if (approvalLevel.ApproveCondition == "single")
                                    {
                                        // get all approvers in this approval level
                                        var getAllApprovers = approvalLevel.MasterFormCCBApprovers.Select(x => x.ApproverStatus).ToList();

                                        // if at least one 'approved' is found in this level approvers
                                        if (getAllApprovers.Contains("approved"))
                                        {
                                            // update this approval level`s status to 'approved'
                                            approvalLevel.ApprovalStatus = "approved";
                                            _context.MasterFormCCBApprovalLevels.Update(approvalLevel);

                                            // check next approval level
                                            // update to 'pending' if exist
                                            var approvalLevelDepartment = _context.MasterFormDepartments.Where(x => x.Id == departmentId).Include(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).FirstOrDefault();
                                            var nextApprovalLevel = approvalLevelDepartment.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "none" && x.Id != approvalLevelId).OrderBy(x => x.Id).FirstOrDefault();

                                            if (nextApprovalLevel != null)
                                            {
                                                nextApprovalLevel.ApprovalStatus = "pending";
                                                emailNotifiersInfo[nextApprovalLevel.Id] = new List<string>();

                                                if (nextApprovalLevel.MasterFormCCBApprovers != null && nextApprovalLevel.MasterFormCCBApprovers.Count() > 0)
                                                {
                                                    nextApprovalLevel.MasterFormCCBApprovers.ToList().ForEach(x => { x.ApproverStatus = "pending"; emailNotifiersInfo[nextApprovalLevel.Id].Add(x.ApproverEmail); });
                                                }

                                                _context.MasterFormCCBApprovalLevels.Update(nextApprovalLevel);
                                            }

                                            await _context.SaveChangesAsync();

                                            _context.Entry(approvalLevel).State = EntityState.Detached;

                                            foreach (var child in approvalLevel.MasterFormCCBApprovers)
                                            {
                                                _context.Entry(child).State = EntityState.Detached;
                                            }

                                            if (approvalLevelDepartment != null)
                                            {
                                                _context.Entry(approvalLevelDepartment).State = EntityState.Detached;

                                                if (nextApprovalLevel != null)
                                                {
                                                    _context.Entry(nextApprovalLevel).State = EntityState.Detached;

                                                    if (nextApprovalLevel.MasterFormCCBApprovers != null && nextApprovalLevel.MasterFormCCBApprovers.Count() > 0)
                                                    {
                                                        foreach (var child in nextApprovalLevel.MasterFormCCBApprovers)
                                                        {
                                                            _context.Entry(child).State = EntityState.Detached;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        // if at least one 'rejected' is found in this level approvers
                                        else if (getAllApprovers.Contains("rejected"))
                                        {
                                            // update this approval level`s status to 'rejected'
                                            approvalLevel.ApprovalStatus = "rejected";
                                            _context.MasterFormCCBApprovalLevels.Update(approvalLevel);

                                            // get the rest approval level
                                            // update to 'rejected' if exist
                                            var approvalLevelDepartment = _context.MasterFormDepartments.Where(x => x.Id == departmentId).Include(x => x.MasterFormCCBApprovalLevels).FirstOrDefault();
                                            var restApprovalLevels = approvalLevelDepartment.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "none" && x.Id != approvalLevelId).ToList();

                                            if (restApprovalLevels != null && restApprovalLevels.Count() > 0)
                                            {
                                                restApprovalLevels.ForEach(x => x.ApprovalStatus = "rejected");

                                                _context.MasterFormCCBApprovalLevels.UpdateRange(restApprovalLevels);
                                            }

                                            await _context.SaveChangesAsync();

                                            _context.Entry(approvalLevel).State = EntityState.Detached;

                                            foreach (var child in approvalLevel.MasterFormCCBApprovers)
                                            {
                                                _context.Entry(child).State = EntityState.Detached;
                                            }

                                            if (approvalLevelDepartment != null)
                                            {
                                                _context.Entry(approvalLevelDepartment).State = EntityState.Detached;

                                                if (restApprovalLevels != null && restApprovalLevels.Count() > 0)
                                                {
                                                    foreach (var child in restApprovalLevels)
                                                    {
                                                        _context.Entry(child).State = EntityState.Detached;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // execute when approval condition is 'all'
                                    else if (approvalLevel.ApproveCondition == "all")
                                    {
                                        // get all approvers in this approval level
                                        var getAllApprovers = approvalLevel.MasterFormCCBApprovers.Select(x => x.ApproverStatus).ToList();

                                        static bool EqualToApprove(string Status)
                                        {
                                            return Status == "approved";
                                        }

                                        if (getAllApprovers.Contains("rejected"))
                                        {
                                            // update this approval level`s status to 'rejected'
                                            approvalLevel.ApprovalStatus = "rejected";
                                            _context.MasterFormCCBApprovalLevels.Update(approvalLevel);
                                            // get the rest approval level
                                            // update to 'rejected' if exist
                                            var approvalLevelDepartment = _context.MasterFormDepartments.Where(x => x.Id == departmentId).Include(x => x.MasterFormCCBApprovalLevels).FirstOrDefault();
                                            var restApprovalLevels = approvalLevelDepartment.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "none" && x.Id != approvalLevelId).ToList();

                                            if (restApprovalLevels != null && restApprovalLevels.Count() > 0)
                                            {
                                                restApprovalLevels.ForEach(x => x.ApprovalStatus = "rejected");

                                                _context.MasterFormCCBApprovalLevels.UpdateRange(restApprovalLevels);
                                            }

                                            await _context.SaveChangesAsync();

                                            _context.Entry(approvalLevel).State = EntityState.Detached;
                                            foreach (var child in approvalLevel.MasterFormCCBApprovers)
                                            {
                                                _context.Entry(child).State = EntityState.Detached;
                                            }

                                            if (approvalLevelDepartment != null)
                                            {
                                                _context.Entry(approvalLevelDepartment).State = EntityState.Detached;

                                                if (restApprovalLevels != null && restApprovalLevels.Count() > 0)
                                                {
                                                    foreach (var child in restApprovalLevels)
                                                    {
                                                        _context.Entry(child).State = EntityState.Detached;
                                                    }
                                                }
                                            }
                                        }
                                        else if (getAllApprovers.TrueForAll(EqualToApprove))
                                        {
                                            // update this approval level`s status to 'approved'
                                            approvalLevel.ApprovalStatus = "approved";

                                            _context.MasterFormCCBApprovalLevels.Update(approvalLevel);

                                            // check next approval level
                                            // update to 'pending' if exist
                                            var approvalLevelDepartment = _context.MasterFormDepartments.Where(x => x.Id == departmentId).Include(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).FirstOrDefault();
                                            var nextApprovalLevel = approvalLevelDepartment.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "none" && x.Id != approvalLevelId).OrderBy(x => x.Id).FirstOrDefault();

                                            if (nextApprovalLevel != null)
                                            {
                                                nextApprovalLevel.ApprovalStatus = "pending";
                                                emailNotifiersInfo[nextApprovalLevel.Id] = new List<string>();

                                                if (nextApprovalLevel.MasterFormCCBApprovers != null && nextApprovalLevel.MasterFormCCBApprovers.Count() > 0)
                                                {
                                                    nextApprovalLevel.MasterFormCCBApprovers.ToList().ForEach(x => { x.ApproverStatus = "pending"; emailNotifiersInfo[nextApprovalLevel.Id].Add(x.ApproverEmail); });
                                                }

                                                _context.MasterFormCCBApprovalLevels.Update(nextApprovalLevel);
                                            }

                                            await _context.SaveChangesAsync();

                                            _context.Entry(approvalLevel).State = EntityState.Detached;
                                            foreach (var child in approvalLevel.MasterFormCCBApprovers)
                                            {
                                                _context.Entry(child).State = EntityState.Detached;
                                            }

                                            if (approvalLevelDepartment != null)
                                            {
                                                _context.Entry(approvalLevelDepartment).State = EntityState.Detached;

                                                if (nextApprovalLevel != null)
                                                {
                                                    _context.Entry(nextApprovalLevel).State = EntityState.Detached;

                                                    if (nextApprovalLevel.MasterFormCCBApprovers != null && nextApprovalLevel.MasterFormCCBApprovers.Count() > 0)
                                                    {
                                                        foreach (var child in nextApprovalLevel.MasterFormCCBApprovers)
                                                        {
                                                            _context.Entry(child).State = EntityState.Detached;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            // update department status if all approval level contain zero 'none'
                            var getDepartment = _context.MasterFormDepartments.Where(x => x.Id == departmentId).Include(x => x.MasterFormCCBApprovalLevels).FirstOrDefault();

                            if (getDepartment != null)
                            {
                                // get all approval level in this department
                                var getAllApprovalLevel = getDepartment.MasterFormCCBApprovalLevels.Select(x => x.ApprovalStatus).ToList();

                                if (getAllApprovalLevel != null && getAllApprovalLevel.Count() > 0)
                                {
                                    // execute if 'none' is not found in every level of department
                                    if (!getAllApprovalLevel.Contains("none") && !getAllApprovalLevel.Contains("pending"))
                                    {
                                        static bool EqualToApprove(string Status)
                                        {
                                            return Status == "approved";
                                        }

                                        if (getAllApprovalLevel.TrueForAll(EqualToApprove))
                                        {
                                            getDepartment.Status = "approved";

                                            _context.MasterFormDepartments.Update(getDepartment);
                                            await _context.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            getDepartment.Status = "rejected";

                                            _context.MasterFormDepartments.Update(getDepartment);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                }

                                _context.Entry(getDepartment).State = EntityState.Detached;

                                if (getDepartment.MasterFormCCBApprovalLevels != null && getDepartment.MasterFormCCBApprovalLevels.Count() > 0)
                                {
                                    foreach (var child in getDepartment.MasterFormCCBApprovalLevels)
                                    {
                                        _context.Entry(child).State = EntityState.Detached;
                                    }
                                }
                            }

                            // update master form status
                            var masterForm = _context.MasterFormLists.Where(x => x.Id == this.MasterFormId).Include(x => x.MasterFormDepartments).ThenInclude(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).FirstOrDefault();

                            if (masterForm != null)
                            {
                                var getAllDepartmentStatus = masterForm.MasterFormDepartments.Select(x => x.Status).ToList();

                                if (!getAllDepartmentStatus.Contains("pending"))
                                {
                                    static bool EqualToApproveOrNoAction(string Status)
                                    {
                                        return Status == "approved" || Status == "no action";
                                    }

                                    // change master form status to 'active'
                                    // if all department status match only 'approved' and 'no action'
                                    if (getAllDepartmentStatus.TrueForAll(EqualToApproveOrNoAction))
                                    {
                                        masterForm.MasterFormStatus = "active";
                                        masterForm.AllowUpRevision = true;

                                        // populate json data
                                        var json = JsonConvert.SerializeObject(masterForm, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                                        JObject modifiedJSON = JObject.Parse(json);
                                        modifiedJSON["MasterFormData"] = "";
                                        modifiedJSON["JSON"] = "";
                                        masterForm.JSON = modifiedJSON.ToString(Formatting.None);

                                        var permittedDepartments = JsonConvert.SerializeObject(masterForm.MasterFormDepartments.Select(x => x.DepartmentName).ToList(), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                                        masterForm.PermittedDepartments = permittedDepartments;

                                        // assign these two so can populate later
                                        var historyJSON = modifiedJSON.ToString(Formatting.None);
                                        var historyCCBApprovalJSON = JsonConvert.SerializeObject(masterForm.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).SelectMany(x => x.MasterFormCCBApprovers).Where(x => x.ApproverStatus == "approved" || x.ApproverStatus == "rejected").Select(x => x.EmployeeId).ToList(), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                                        // reset department, approval levels and approvers
                                        masterForm.MasterFormDepartments.ToList().ForEach(x => x.Status = "no action");
                                        masterForm.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).ToList().ForEach(x => { x.ApprovalStatus = "none"; x.LastSend = null; });
                                        masterForm.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).SelectMany(x => x.MasterFormCCBApprovers).ToList().ForEach(x => { x.ApproverStatus = "none"; x.ApproveDate = null; });

                                        // add editable form approval data to fix
                                        var switchFormApproval = JsonConvert.DeserializeObject<FormApproval>(masterForm.FormApprovalJSON);
                                        switchFormApproval.FixFormApproval = switchFormApproval.EditableFormApproval;

                                        masterForm.FormApprovalJSON = JsonConvert.SerializeObject(switchFormApproval);

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

                                        if (Environment.IsProduction())
                                        {
                                            string DomainName = _configuration["Domain:ServerURL"].ToString() + "/MasterForm/Index";
                                            string Sender = masterForm.OwnerEmailAddress;

                                            StringBuilder StrMessage = new StringBuilder();
                                            StrMessage.AppendFormat("<p>Your master form <b>{0} ({1})</b> is approved</p>", this.MasterFormName, this.MasterFormDescription);
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, Sender);

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approve notification", Sender, this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approve notification", Sender, this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }
                                        }
                                        else if (Environment.IsDevelopment())
                                        {
                                            string DomainName = _configuration["Domain:ServerURL"].ToString() + "/MasterForm/Index";
                                            string Sender = masterForm.OwnerEmailAddress;

                                            StringBuilder StrMessage = new StringBuilder();
                                            StrMessage.AppendFormat("<p>Your master form <b>{0} ({1})</b> is approved</p>", this.MasterFormName, this.MasterFormDescription);
                                            StrMessage.AppendFormat("<p>Receiver: {0}</p>", Sender);
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approve notification", Sender, this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approve notification", Sender, this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }
                                        }

                                        await _context.SaveChangesAsync();
                                    }
                                    // change master form status to 'rejected'
                                    // if all department status do not meet the criteria
                                    else
                                    {
                                        masterForm.MasterFormStatus = "rejected";

                                        // populate json data
                                        var json = JsonConvert.SerializeObject(masterForm, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                                        JObject modifiedJSON = JObject.Parse(json);
                                        modifiedJSON["MasterFormData"] = "";
                                        modifiedJSON["JSON"] = "";
                                        masterForm.JSON = modifiedJSON.ToString(Formatting.None);

                                        var permittedDepartments = JsonConvert.SerializeObject(masterForm.MasterFormDepartments.Select(x => x.DepartmentName).ToList(), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                                        masterForm.PermittedDepartments = permittedDepartments;

                                        // assign these two so can populate later
                                        var historyJSON = modifiedJSON.ToString(Formatting.None);
                                        var historyCCBApprovalJSON = JsonConvert.SerializeObject(masterForm.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).SelectMany(x => x.MasterFormCCBApprovers).Where(x => x.ApproverStatus == "approved" || x.ApproverStatus == "rejected").Select(x => x.EmployeeId).ToList(), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                                        // reset department, approval levels and approvers
                                        masterForm.MasterFormDepartments.ToList().ForEach(x => x.Status = "no action");
                                        masterForm.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).ToList().ForEach(x => { x.ApprovalStatus = "none"; x.LastSend = null; });
                                        masterForm.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels).SelectMany(x => x.MasterFormCCBApprovers).ToList().ForEach(x => { x.ApproverStatus = "none"; x.ApproveDate = null; x.Remark = null; });

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

                                        if (Environment.IsProduction())
                                        {
                                            string DomainName = _configuration["Domain:ServerURL"].ToString() + "/MasterForm/Index";
                                            string Sender = masterForm.OwnerEmailAddress;

                                            StringBuilder StrMessage = new StringBuilder();
                                            StrMessage.AppendFormat("<p>Your master form <b>{0} ({1})</b> is rejected</p>", this.MasterFormName, this.MasterFormDescription);
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, Sender);

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approve notification", Sender, this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approve notification", Sender, this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }
                                        }
                                        else if (Environment.IsDevelopment())
                                        {
                                            string DomainName = _configuration["Domain:ServerURL"].ToString() + "/MasterForm/Index";
                                            string Sender = masterForm.OwnerEmailAddress;

                                            StringBuilder StrMessage = new StringBuilder();
                                            StrMessage.AppendFormat("<p>Your master form <b>{0} ({1})</b> is rejected</p>", this.MasterFormName, this.MasterFormDescription);
                                            StrMessage.AppendFormat("<p>Receiver: {0}</p>", Sender);
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to view.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approve notification", Sender, this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approve notification", Sender, this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                            }
                                        }

                                        await _context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    // populate json data
                                    var json = JsonConvert.SerializeObject(masterForm, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                                    JObject modifiedJSON = JObject.Parse(json);
                                    modifiedJSON["MasterFormData"] = "";
                                    modifiedJSON["JSON"] = "";
                                    masterForm.JSON = modifiedJSON.ToString(Formatting.None);

                                    _context.Attach(masterForm);
                                    _context.Entry(masterForm).Property(x => x.JSON).IsModified = true;

                                    await _context.SaveChangesAsync();
                                }
                            }

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
                                            StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", this.MasterFormName, this.MasterFormDescription, this.MasterFormCreatedBy);
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - CCB Approval", StrMessage.ToString(), Senders, null);

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approval", String.Join(", ", Senders), this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                confirmSend = true;
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approval", String.Join(", ", Senders), this.MasterFormName);

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
                                            StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", this.MasterFormName, this.MasterFormDescription, this.MasterFormCreatedBy);
                                            StrMessage.AppendFormat("<p>Receiver: {0}</p>", String.Join(", ", Senders));
                                            StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);

                                            var emailNotificationResult = MicrosoftExchangeEmailService.SendEmail("e-Record Notification - CCB Approval", StrMessage.ToString(), null, "christopher@sophicautomation.com");

                                            if (emailNotificationResult == "Success")
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approval", String.Join(", ", Senders), this.MasterFormName);

                                                _context.PublicLogEmailNotifications.Add(logEmailNotification);

                                                confirmSend = true;
                                            }
                                            else
                                            {
                                                var logEmailNotification = new PublicLogEmailNotification();
                                                logEmailNotification.DateTime = DateTime.Now;
                                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approval", String.Join(", ", Senders), this.MasterFormName);

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
