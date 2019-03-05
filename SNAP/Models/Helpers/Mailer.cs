using System;
using System.IO;
using System.Net.Mail;
using System.Web;

namespace SNAP.Models.Helpers
{
    public enum MessageTemplate
    {
        Default,
        NewHireEmployee,
        NewHireContingent,
        LastDayEmployee,
        LastDayContingent,
        Contingent,
        Evaluation,
        ServiceRequest,
        ITAASRequest,
        ErrorNotice
    }

    public class Mailer
    {
        private MailMessage Message { get; set; }

        public Mailer(MessageTemplate messageTemplate, bool attachBanner)
        {
            Message = new MailMessage();
            Message.IsBodyHtml = true;

            if (attachBanner == true)
            {
                Attachment aisBanner = new Attachment(Path.Combine(HttpRuntime.AppDomainAppPath, "Content/images/AIS_Banner.jpg"));
                aisBanner.ContentId = "AIS_Banner";
                Message.Attachments.Add(aisBanner);
            }

            switch (messageTemplate)
            {
                case MessageTemplate.NewHireEmployee:
                    AddRecipient("AM.HR.Notification.All@dimensiondata.com");
                    AddRecipient("AM.IS.NewHireNotifications@dimensiondata.com");
                    break;

                case MessageTemplate.NewHireContingent:
                    AddRecipient("AM.HR.Notification.All@dimensiondata.com");
                    AddRecipient("AM.IS.NewHireNotifications@dimensiondata.com");
                    AddRecipient("US.StaffingOpsTeam@dimensiondata.com");
                    break;

                case MessageTemplate.LastDayEmployee:
                    AddRecipient("AM.HR.Notification.All@dimensiondata.com");
                    AddRecipient("AM.IS.LastDayNotifications@dimensiondata.com");
                    break;

                case MessageTemplate.LastDayContingent:
                    AddRecipient("AM.HR.Notification.All@dimensiondata.com");
                    AddRecipient("AM.IS.LastDayNotifications@dimensiondata.com");
                    AddRecipient("US.StaffingOpsTeam@dimensiondata.com");
                    break;

                case MessageTemplate.ServiceRequest:
                    AddRecipient("AM.ServiceDesk@us.didata.com");
                    break;

                case MessageTemplate.ITAASRequest:
                    AddRecipient("ITaas.Help@dimensiondata.com");
                    break;

                case MessageTemplate.Contingent:
                    AddRecipient("AM.IS.Contingent.Notifications@dimensiondata.com");
                    break;

                case MessageTemplate.Evaluation:
                    AddRecipient("AM.EvaluationsTeam@dimensiondata.com");
                    break;

                case MessageTemplate.ErrorNotice:
                    AddRecipient("AM.SNAP.Errors@dimensiondata.com");
                    break;

                default:
                    break;
            }
        }

        internal void SetFromAddress(string address)
        {
            try
            {
                Message.From = new MailAddress(address);
            }
            catch (Exception)
            {
                //REVISIT - Failed to set From Address
            }
        }

        internal void AddRecipient(string address)
        {
            try
            {
                Message.To.Add(new MailAddress(address));
            }
            catch (Exception)
            {
                Message.Bcc.Add(new MailAddress("AM.SNAP.Errors@dimensiondata.com"));
            }
        }

        internal void AddITaaSNotificationGroup()
        {
            try
            {
                Message.To.Add(new MailAddress("employee.movement@itaas.dimensiondata.com"));
            }
            catch (Exception)
            {
                Message.Bcc.Add(new MailAddress("AM.SNAP.Errors@dimensiondata.com"));
            }
        }

        internal void AddBcc(string address)
        {
            try
            {
                Message.Bcc.Add(new MailAddress(address));
            }
            catch (Exception)
            {
                Message.Bcc.Add(new MailAddress("AM.SNAP.Errors@dimensiondata.com"));
            }
        }

        internal void SendMessage(string view, object model, string subject, bool external = false)
        {
            try
            {
                Message.Subject = subject;
                Message.Body = PowerShell.GetHTMLString("Mail", view, model);

                if (String.IsNullOrWhiteSpace(Message.Body))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Message = new MailMessage();
                Message.Subject = subject;
                Message.To.Add(new MailAddress("AM.SNAP.Errors@dimensiondata.com"));
            }

            using (SmtpClient smtp = new SmtpClient())
            {
                if (external == true)
                {
                    smtp.Host = "relay.us.didata.com";
                }

                smtp.Send(Message);
            }
        }
    }
}