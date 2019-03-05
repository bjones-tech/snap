using SNAP.Models.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using System.Web.Mvc;

namespace SNAP.Models.Persistent
{
    public class LastDay
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        // Employee Only
        [Display(Name = "Worker ID")]
        public string WorkerID { get; set; }

        [Required]
        [Display(Name = "Worker Type")]
        public string WorkerType { get; set; }

        [Display(Name = "Contingent")]
        public bool IsContingent { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? EndDate { get; set; }

        // Employee Only
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? StartDate { get; set; }

        // Employee Only
        [Display(Name = "Code")]
        public string Code { get; set; }

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

        [Required]
        [Display(Name = "Office")]
        public string Office { get; set; }

        // Service Request
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Immediately Disable Account")]
        public bool Immediate { get; set; }

        [Display(Name = "Public Notes")]
        [DataType(DataType.MultilineText)]
        public string PublicNotes { get; set; }

        [Display(Name = "Private Notes")]
        [DataType(DataType.MultilineText)]
        public string PrivateNotes { get; set; }

        [Display(Name = "Requester")]
        public string Requester { get; set; }

        [Display(Name = "Requester's Email")]
        public string RequestersEmail { get; set; }

        [Display(Name = "Requested On")]
        public DateTime RequestedOn { get; set; }

        [Required]
        [Display(Name = "GUID")]
        public string GUID { get; set; }

        [Display(Name = "Decommissioned")]
        public bool Decommissioned { get; set; }

        [Display(Name = "Service Request Sent")]
        public bool ServiceRequest { get; set; }

        [Display(Name = "Complete")]
        public bool Complete { get; set; }

        [Display(Name = "Error Log")]
        [DataType(DataType.MultilineText)]
        public string ErrorLog { get; set; }

        [NotMapped]
        [Display(Name = "Suppress Notification")]
        public bool Suppress { get; set; }
        
        [Display(Name = "ITaaS")]
        public bool ITaaS { get; set; }

        internal static LastDay Create(string emailAddress)
        {
            try
            {
                LastDay lastDay = new LastDay();

                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = emailAddress });

                string distinguishedName = exchangeRecipient.DistinguishedName;

                ADUser adUser = ADUser.CreateFromIdentity(distinguishedName);

                if (adUser == null)
                {
                    throw new Exception();
                }

                lastDay.GUID = adUser.GUID.ToString();
                lastDay.EmailAddress = adUser.EmailAddress;
                lastDay.Name = adUser.DisplayName;
                lastDay.WorkerID = adUser.EmployeeNumber;
                lastDay.WorkerType = adUser.EmployeeType;
                lastDay.Title = adUser.Title;
                lastDay.Department = adUser.Department;
                lastDay.Office = adUser.Office;
                lastDay.Country = adUser.Country;

                if (!String.IsNullOrWhiteSpace(adUser.Manager))
                {
                    lastDay.SetManagersEmail(adUser.Manager);
                }

                if (lastDay.WorkerType == "Contingent Worker-StaffAug")
                {
                    lastDay.IsContingent = true;
                }

                if (!String.IsNullOrWhiteSpace(lastDay.WorkerID))
                {
                    lastDay.SetStartDate(lastDay.WorkerID);
                }

                lastDay.EndDate = null;

                return lastDay;
            }
            catch (Exception)
            {
                return null;
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

        private void SetStartDate(string workerID)
        {
            try
            {
                dynamic workdayEmployee = PowerShell.RunScript("Get-WorkdayEmployee", new { WorkerID = workerID });

                StartDate = Convert.ToDateTime(workdayEmployee.Hire_Date);
            }
            catch (Exception)
            {
                return;
            }
        }

        internal void Validate(ModelStateDictionary modelState, IPrincipal user)
        {
            if (!String.IsNullOrWhiteSpace(WorkerID) && String.IsNullOrWhiteSpace(Code))
            {
                modelState.AddModelError("Code", "Code Required");
            }

            Requester = user.Identity.Name;
            RequestedOn = DateTime.Now;

            ValidateManager(modelState);
            SetRequestersEmail(user);
        }

        internal void ValidateManager(ModelStateDictionary modelState)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = ManagersEmail });

                Manager = exchangeRecipient.DisplayName;
            }
            catch (Exception)
            {
                modelState.AddModelError("ManagersEmail", "Invalid Manager");
            }
        }

        internal void SetRequestersEmail(IPrincipal user)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = user.Identity.Name });

                RequestersEmail = exchangeRecipient.PrimarySmtpAddress;
            }
            catch (Exception)
            {
                //REVISIT - Failed to set Last Day Requester's Email
            }
        }

        internal void Decommission()
        {
            try
            {
                PowerShell.RunScript("Decommission-LastDay", new { LastDay = this });
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}