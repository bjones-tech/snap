using SNAP.Models.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace SNAP.Models.Persistent
{
    public class Contingent
    {
        public int ID { get; set; }

        [Display(Name = "GUID")]
        public string GUID { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Manager's Name")]
        public string ManagersName { get; set; }

        [Display(Name = "Manager's Email")]
        public string ManagersEmail { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Office")]
        public string Office { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Account Created")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? AccountCreated { get; set; }

        [Display(Name = "Account Expiration Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? AccountExpirationDate { get; set; }

        [Display(Name = "Code Of Ethics")]
        public bool CodeOfEthics { get; set; }

        [Display(Name = "Info Security")]
        public bool InfoSecurity { get; set; }

        [Display(Name = "People & Culture Business Partner")]
        public string PCBusinessPartner { get; set; }

        [NotMapped]
        [Display(Name = "Manager")]
        public string Manager { get; set; }

        [NotMapped]
        [Display(Name = "Status Style")]
        public string StatusStyle { get; set; }

        internal static Contingent Create(ADUser adUser)
        {
            try
            {
                Contingent contingent = new Contingent();

                contingent.GUID = adUser.GUID;
                contingent.EmailAddress = adUser.EmailAddress;
                contingent.Name = adUser.DisplayName;
                contingent.FirstName = adUser.GivenName;
                contingent.LastName = adUser.Surname;
                contingent.Title = adUser.Title;
                contingent.Department = adUser.Department;
                contingent.Country = adUser.Country;
                contingent.Office = adUser.Office;
                contingent.State = adUser.State;
                contingent.AccountCreated = adUser.Created;
                contingent.AccountExpirationDate = adUser.AccountExpirationDate;

                if (String.IsNullOrWhiteSpace(contingent.GUID) || String.IsNullOrWhiteSpace(contingent.FirstName) || String.IsNullOrWhiteSpace(contingent.LastName))
                {
                    throw new Exception();
                }

                if (!String.IsNullOrWhiteSpace(adUser.Manager))
                {
                    contingent.SetManagersEmail(adUser.Manager);
                }

                return contingent;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal void Validate(ModelStateDictionary modelState)
        {
            if (AccountExpirationDate == null)
            {
                modelState.AddModelError("AccountExpirationDate", "Account Expiration Date Required");
            }

            if (String.IsNullOrWhiteSpace(Title))
            {
                modelState.AddModelError("Title", "Title Required");
            }

            if (String.IsNullOrWhiteSpace(ManagersEmail))
            {
                modelState.AddModelError("ManagersEmail", "Manager's Email Required");
            }

            ValidateManager(modelState);
        }

        private void SetManagersEmail(string manager)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = manager });

                ManagersName = exchangeRecipient.DisplayName;
                ManagersEmail = exchangeRecipient.PrimarySmtpAddress;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void ValidateManager(ModelStateDictionary modelState)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = ManagersEmail });

                Manager = exchangeRecipient.DistinguishedName;
                ManagersName = exchangeRecipient.DisplayName;
                ManagersEmail = exchangeRecipient.PrimarySmtpAddress;
            }
            catch (Exception)
            {
                modelState.AddModelError("ManagersEmail", "Invalid Manager");
            }
        }

        internal bool UpdateAccount()
        {
            try
            {
                PowerShell.RunScript("Update-ADUser", new { Attributes = this });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal void SetStatusStyle()
        {
            if (AccountExpirationDate == null || String.IsNullOrWhiteSpace(ManagersEmail) || CodeOfEthics == false || InfoSecurity == false)
            {
                StatusStyle = "btn btn-warning";
            }
        }
    }
}