using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Service
{
    internal class MasterFormEmailService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory scopeFactory;
        private Timer _timer;
        private readonly IConfiguration _config;

        public MasterFormEmailService(ILogger<MasterFormEmailService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            this.scopeFactory = scopeFactory;
            _config = configuration;
        }

        public System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Master Form: Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public class MasterFormApprovalLevel
        {
            public string MasterFormName { get; set; }
            public string MasterFormCreator { get; set; }
            public int? ApprovalLevelId { get; set; }
            public string EmailReminder { get; set; }
            public string ReminderType { get; set; }
            public string MasterFormDescription { get; set; }
            public DateTime? LastSend { get; set; }
            public List<MasterFormApprover> Approver { get; set; }
        }

        public class MasterFormApprover
        {
            public string ApproverEmail { get; set; }
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Master Form: Timed Background Service is working.");

            using (var scope = scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var MasterFormList = _context.MasterFormLists.Include(x => x.MasterFormDepartments).ThenInclude(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).Where(x => x.MasterFormStatus == "pending").AsNoTracking().ToList();
                var MasterFormLevel = MasterFormList.SelectMany(x => x.MasterFormDepartments.SelectMany(x => x.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "pending")), (p, c) => new MasterFormApprovalLevel
                {
                    ApprovalLevelId = c.Id,
                    EmailReminder = c.EmailReminder,
                    ReminderType = c.ReminderType,
                    MasterFormName = p.MasterFormName,
                    MasterFormDescription = p.MasterFormDescription,
                    MasterFormCreator = p.CreatedBy,
                    LastSend = c.LastSend,
                    Approver = c.MasterFormCCBApprovers.Where(x => x.ApproverStatus == "pending").Select(x => new MasterFormApprover { ApproverEmail = x.ApproverEmail }).ToList()
                }).ToList();

                string DomainName = (!String.IsNullOrEmpty(_config["Domain:ServerURL"]) ? _config["Domain:ServerURL"] + "/MasterFormCCBApproval/Index" : "No selected domain name");

                ExchangeService _service;

                _service = new ExchangeService
                {
                    Credentials = new WebCredentials("svc-e-Recording@wdc.com", "S$DeR3c0rd1ng")
                };

                _service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

                foreach (var masterFormApprover in MasterFormLevel)
                {
                    Console.WriteLine("Last Send ~ ");
                    Console.WriteLine(masterFormApprover.LastSend);

                    if (masterFormApprover.LastSend == null)
                    {
                        Console.WriteLine("Execute if last send is null");

                        var confirmSend = false;

                        var Approvers = masterFormApprover.Approver.Select(x => x.ApproverEmail).ToList();

                        if (Approvers.Count > 0)
                        {
                            try
                            {
                                EmailMessage email = new EmailMessage(_service);

                                foreach (var Approver in Approvers)
                                {
                                    email.ToRecipients.Add(Approver);
                                }

                                email.Subject = "e-Record Notification - CCB Approval";

                                StringBuilder StrMessage = new StringBuilder();
                                StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", masterFormApprover.MasterFormName, masterFormApprover.MasterFormDescription, masterFormApprover.MasterFormCreator);
                                StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);
                                StrMessage.AppendFormat("<div>This is an automated message, please do not reply.</div>");
                                email.Body = StrMessage.ToString();
                                email.Send();

                                var logEmailNotification = new PublicLogEmailNotification();
                                logEmailNotification.DateTime = DateTime.Now;
                                logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approval", String.Join(", ", Approvers), masterFormApprover.MasterFormName);

                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                _context.SaveChanges();

                                confirmSend = true;
                            }
                            catch
                            {
                                var logEmailNotification = new PublicLogEmailNotification();
                                logEmailNotification.DateTime = DateTime.Now;
                                logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approval", String.Join(", ", Approvers), masterFormApprover.MasterFormName);

                                _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                _context.SaveChanges();
                            }


                            if (confirmSend)
                            {
                                var updateApprovalLevel = _context.MasterFormCCBApprovalLevels.Where(x => x.Id == masterFormApprover.ApprovalLevelId).FirstOrDefault();

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
                    else
                    {
                        if (masterFormApprover.ReminderType == "Minutes")
                        {
                            Console.WriteLine("Execute if last send is minutes");

                            if (DateTime.Now >= masterFormApprover.LastSend.Value.AddMinutes(Convert.ToDouble(masterFormApprover.EmailReminder)))
                            {
                                var confirmSend = false;

                                var Approvers = masterFormApprover.Approver.Select(x => x.ApproverEmail).ToList();

                                if (Approvers.Count > 0)
                                {
                                    try
                                    {
                                        EmailMessage email = new EmailMessage(_service);

                                        foreach (var Approver in Approvers)
                                        {
                                            email.ToRecipients.Add(Approver);
                                        }

                                        email.Subject = "e-Record Notification - CCB Approval";

                                        StringBuilder StrMessage = new StringBuilder();
                                        StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", masterFormApprover.MasterFormName, masterFormApprover.MasterFormDescription, masterFormApprover.MasterFormCreator);
                                        StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);
                                        StrMessage.AppendFormat("<div>This is an automated message, please do not reply.</div>");
                                        email.Body = StrMessage.ToString();
                                        email.Send();

                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approval", String.Join(", ", Approvers), masterFormApprover.MasterFormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                        _context.SaveChanges();

                                        confirmSend = true;
                                    }
                                    catch
                                    {
                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approval", String.Join(", ", Approvers), masterFormApprover.MasterFormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                        _context.SaveChanges();
                                    }
                                }


                                if (confirmSend)
                                {
                                    var updateApprovalLevel = _context.MasterFormCCBApprovalLevels.Where(x => x.Id == masterFormApprover.ApprovalLevelId).FirstOrDefault();

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
                        else if (masterFormApprover.ReminderType == "Hours")
                        {
                            Console.WriteLine("Execute if last send is hours");

                            if (DateTime.Now >= masterFormApprover.LastSend.Value.AddHours(Convert.ToDouble(masterFormApprover.EmailReminder)))
                            {
                                var confirmSend = false;

                                var Approvers = masterFormApprover.Approver.Select(x => x.ApproverEmail).ToList();

                                if (Approvers.Count > 0)
                                {
                                    try
                                    {
                                        EmailMessage email = new EmailMessage(_service);

                                        foreach (var Approver in Approvers)
                                        {
                                            email.ToRecipients.Add(Approver);
                                        }

                                        email.Subject = "e-Record Notification - CCB Approval";

                                        StringBuilder StrMessage = new StringBuilder();
                                        StrMessage.AppendFormat("<p><b>{0} ({1})</b> submitted by <b>{2}</b> is required your approval</p>", masterFormApprover.MasterFormName, masterFormApprover.MasterFormDescription, masterFormApprover.MasterFormCreator);
                                        StrMessage.AppendFormat("<a href={0}><p>Click here to access to the dashboard to approve/reject.</p></a>", DomainName);
                                        StrMessage.AppendFormat("<div>This is an automated message, please do not reply.</div>");
                                        email.Body = StrMessage.ToString();
                                        email.Send();

                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} successed for master form ({1}) approval", String.Join(", ", Approvers), masterFormApprover.MasterFormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                        _context.SaveChanges();

                                        confirmSend = true;
                                    }
                                    catch
                                    {
                                        var logEmailNotification = new PublicLogEmailNotification();
                                        logEmailNotification.DateTime = DateTime.Now;
                                        logEmailNotification.LogDetail = String.Format("Email send to {0} failed for master form ({1}) approval", String.Join(", ", Approvers), masterFormApprover.MasterFormName);

                                        _context.PublicLogEmailNotifications.Add(logEmailNotification);
                                        _context.SaveChanges();
                                    }
                                }

                                if (confirmSend)
                                {
                                    var updateApprovalLevel = _context.MasterFormCCBApprovalLevels.Where(x => x.Id == masterFormApprover.ApprovalLevelId).FirstOrDefault();

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
            _logger.LogInformation("Master Form: Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
