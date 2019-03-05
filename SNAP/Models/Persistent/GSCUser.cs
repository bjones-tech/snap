using SNAP.DAL;
using SNAP.Models.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web.Mvc;

namespace SNAP.Models.Persistent
{
    public class GSCUser
    {
        public int ID { get; set; }
        public int GSCClientID { get; set; }

        public virtual GSCClient GSCClient { get; set; }

        [Required]
        [Display(Name = "GUID")]
        public string GUID { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Manager")]
        public string Manager { get; set; }

        [Required]
        [Display(Name = "Manager's Email")]
        public string ManagersEmail { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; } = true;
        
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        internal void Validate(ModelStateDictionary modelState, string emailAddress, GSCClient gscClient)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = emailAddress });

                string distinguishedName = exchangeRecipient.DistinguishedName;

                ADUser adUser = ADUser.CreateFromIdentity(distinguishedName);

                if (adUser == null)
                {
                    throw new Exception();
                }

                GUID = adUser.GUID.ToString();
                EmailAddress = adUser.EmailAddress;
                Name = adUser.DisplayName;
                Title = adUser.Title;
                Department = adUser.Department;
                GSCClientID = gscClient.ID;

                if (!String.IsNullOrWhiteSpace(adUser.Manager))
                {
                    SetManagersEmail(adUser.Manager);
                }
            }
            catch (Exception)
            {
                modelState.AddModelError("EmailAddress", "Invalid User");
            }
        }

        private void SetManagersEmail(string manager)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = manager });

                Manager = exchangeRecipient.DisplayName;
                ManagersEmail = exchangeRecipient.PrimarySmtpAddress;
            }
            catch (Exception)
            {
                return;
            }
        }

        internal void MailExternalCompany()
        {
            Mailer mailer = new Mailer(MessageTemplate.Default, false);

            mailer.SetFromAddress("AM.SNAP.Notifications@dimensiondata.com");
            mailer.AddRecipient(GSCClient.EmailAddress);
            mailer.AddRecipient(GSCClient.InternalEmailAddress);
            mailer.SendMessage("GSCUserNotice", this, "Dimension Data Resource Notification", true);
        }

        internal void Decommission(SNAPContext db)
        {
            Active = false;
            EndDate = DateTime.Today;

            MailExternalCompany();

            db.Entry(this).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}