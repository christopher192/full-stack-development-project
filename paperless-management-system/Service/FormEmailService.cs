using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.Service;

namespace WD_ERECORD_CORE.Service
{
    internal class FormEmailService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory scopeFactory;
        private Timer _timer;
        private readonly IConfiguration _config;

        public FormEmailService(ILogger<FormEmailService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            this.scopeFactory = scopeFactory;
            _config = configuration;
        }

        public System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Form: Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public class FormApprovalLevel
        {
            public string FormName { get; set; }
            public string FormCreator { get; set; }
            public int? ApprovalLevelId { get; set; }
            public string EmailReminder { get; set; }
            public string ReminderType { get; set; }
            public string FormDescription { get; set; }
            public DateTime? LastSend { get; set; }
            public List<FormApprover> Approver { get; set; }
        }

        public class FormApprover
        {
            public string ApproverEmail { get; set; }
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Form: Timed Background Service is working.");

            using (var scope = scopeFactory.CreateScope()) {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var FormList = _context.FormLists.Include(x => x.FormListApprovalLevels).ThenInclude(x => x.FormListApprovers).Where(x => x.FormStatus == "pending").AsNoTracking().ToList();
                var FormLevel = FormList.SelectMany(x => x.FormListApprovalLevels.Where(x => x.ApprovalStatus == "pending"), (p, c) => new FormApprovalLevel
                {
                    ApprovalLevelId = c.Id,
                    EmailReminder = c.EmailReminder,
                    FormName = p.FormName,
                    FormDescription = p.FormDescription,
                    ReminderType = c.ReminderType,
                    FormCreator = p.CreatedBy,
                    LastSend = c.LastSend,
                    Approver = c.FormListApprovers.Where(x => x.ApproverStatus == "pending").Select(x => new FormApprover { ApproverEmail = x.ApproverEmail }).ToList()
                }).ToList();

                string DomainName = (!String.IsNullOrEmpty(_config["Domain:ServerURL"]) ? _config["Domain:ServerURL"] + "/EFormApproval/Index" : "No selected domain name");

                ExchangeService _service;

                _service = new ExchangeService
                {
                    Credentials = new WebCredentials("svc-e-Recording@wdc.com", "S$DeR3c0rd1ng")
                };

                _service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

                foreach (var FormApprover in FormLevel)
                {
                    Console.WriteLine("Approval Level Id: " + FormApprover.ApprovalLevelId.ToString());

                    if (FormApprover.LastSend == null)
                    {
                        Console.WriteLine("Execute if last send is null");

                        var confirmSend = false;

                        var Approvers = FormApprover.Approver.Select(x => x.ApproverEmail).ToList();

                        if (Approvers.Count > 0) {
                            try
                            {
                                EmailMessage email = new EmailMessage(_service);

                                foreach (var Approver in Approvers)
                                {
                                    email.ToRecipients.Add(Approver);
                                }

                                email.Subject = "e-Record Notification - EForm Approval";

                                StringBuilder StrMessage = new StringBuilder();
                                StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", FormApprover.FormName, FormApprover.FormDescription, FormApprover.FormCreator);
                                StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);
                                StrMessage.AppendFormat("<div>This is an automated message, please do not reply.</div>");
                                email.Body = StrMessage.ToString();
                                email.Send();

                                var logEmailNotification = new PublicLogEmailNotification();
                                logEmailNotification.DateTime = DateTime.Now;
                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for user form ({1}) approval", String.Join(", ", Approvers), FormApprover.FormName);

                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                _context.SaveChanges();

                                confirmSend = true;
                            }
                            catch
                            {
                                var logEmailNotification = new PublicLogEmailNotification();
                                logEmailNotification.DateTime = DateTime.Now;
                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for user form ({1}) approval", String.Join(", ", Approvers), FormApprover.FormName);

                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                _context.SaveChanges();
                            }
                        }

                        if (confirmSend)
                        {
                            var updateApprovalLevel = _context.FormListApprovalLevels.Where(x => x.Id == FormApprover.ApprovalLevelId).FirstOrDefault();

                            if (updateApprovalLevel != null)
                            {
                                updateApprovalLevel.LastSend = DateTime.Now;
                                _context.Attach(updateApprovalLevel);
                                _context.Entry(updateApprovalLevel).Property(x => x.LastSend).IsModified = true;
                                _context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        if (FormApprover.ReminderType == "Minutes")
                        {
                            Console.WriteLine("Execute if last send is minutes");

                            if (DateTime.Now >= FormApprover.LastSend.Value.AddMinutes(Convert.ToDouble(FormApprover.EmailReminder)))
                            {
                                var confirmSend = false;

                                var Approvers = FormApprover.Approver.Select(x => x.ApproverEmail).ToList();

                                if (Approvers.Count > 0) {
                                    try
                                    {
                                        EmailMessage email = new EmailMessage(_service);

                                        foreach (var Approver in Approvers)
                                        {
                                            email.ToRecipients.Add(Approver);
                                        }

                                        email.Subject = "e-Record Notification - EForm Approval";

                                        StringBuilder StrMessage = new StringBuilder();
                                        StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", FormApprover.FormName, FormApprover.FormDescription, FormApprover.FormCreator);
                                        StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);
                                        StrMessage.AppendFormat("<div>This is an automated message, please do not reply.</div>");
                                        email.Body = StrMessage.ToString();

                                        email.Send();

                                         var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for user form ({1}) approval", String.Join(", ", Approvers), FormApprover.FormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                        _context.SaveChanges();

                                        confirmSend = true;
                                    }
                                    catch
                                    {
                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for user form ({1}) approval", String.Join(", ", Approvers), FormApprover.FormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                        _context.SaveChanges();
                                    }
                                }

                                if (confirmSend)
                                {
                                    var updateApprovalLevel = _context.FormListApprovalLevels.Where(x => x.Id == FormApprover.ApprovalLevelId).FirstOrDefault();

                                    if (updateApprovalLevel != null)
                                    {
                                        updateApprovalLevel.LastSend = DateTime.Now;
                                        _context.Attach(updateApprovalLevel);
                                        _context.Entry(updateApprovalLevel).Property(x => x.LastSend).IsModified = true;
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        else if (FormApprover.ReminderType == "Hours")
                        {
                            Console.WriteLine("Execute if last send is hours");

                            if (DateTime.Now >= FormApprover.LastSend.Value.AddHours(Convert.ToDouble(FormApprover.EmailReminder)))
                            {
                                var confirmSend = false;

                                var Approvers = FormApprover.Approver.Select(x => x.ApproverEmail).ToList();

                                if (Approvers.Count > 0) {
                                    try
                                    {
                                        EmailMessage email = new EmailMessage(_service);

                                        foreach (var Approver in Approvers)
                                        {
                                            email.ToRecipients.Add(Approver);
                                        }

                                        email.Subject = "e-Record Notification - EForm Approval";

                                        StringBuilder StrMessage = new StringBuilder();
                                        StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", FormApprover.FormName, FormApprover.FormDescription, FormApprover.FormCreator);
                                        StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);
                                        StrMessage.AppendFormat("<div>This is an automated message, please do not reply.</div>");
                                        email.Body = StrMessage.ToString();

                                        email.Send();

                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for user form ({1}) approval", String.Join(", ", Approvers), FormApprover.FormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                        _context.SaveChanges();

                                        confirmSend = true;
                                    }
                                    catch
                                    {
                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for user form ({1}) approval", String.Join(", ", Approvers), FormApprover.FormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                        _context.SaveChanges();
                                    }
                                }

                                if (confirmSend)
                                {
                                    var updateApprovalLevel = _context.FormListApprovalLevels.Where(x => x.Id == FormApprover.ApprovalLevelId).FirstOrDefault();

                                    if (updateApprovalLevel != null)
                                    {
                                        updateApprovalLevel.LastSend = DateTime.Now;
                                        _context.Attach(updateApprovalLevel);
                                        _context.Entry(updateApprovalLevel).Property(x => x.LastSend).IsModified = true;
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Form: Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
