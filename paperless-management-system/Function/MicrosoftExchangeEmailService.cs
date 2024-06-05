using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Function
{
    public static class MicrosoftExchangeEmailService
    {
        public static string SendEmail(string subject, string emailBody, string[] receivers, string receiver)
        {
            try
            {
                ExchangeService _service;

                _service = new ExchangeService
                {
                    Credentials = new WebCredentials("svc-e-Recording@wdc.com", "S$DeR3c0rd1ng")
                };

                _service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

                EmailMessage email = new EmailMessage(_service);

                if (receivers != null) {
                    if (receivers.Length > 0) {
                        foreach (var ireceiver in receivers)
                        {
                            email.ToRecipients.Add(ireceiver);
                        }
                    }
                }

                if (receiver != null)
                {
                    email.ToRecipients.Add(receiver);
                }

                /*                email.CcRecipients.Add("elwyntoh@sophicautomation.com");*/
                email.Subject = subject;
                emailBody += "<div> This is an automated message, please do not reply.</div>";
                email.Body = new MessageBody(emailBody);
                email.Send();

                return "Success";
            }
            catch
            {
                return "Fail";
            }
        }
    }
}
