using SNAP.DAL;
using SNAP.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace SNAP.Models.Persistent
{
    public class NewHire
    {
        public int ID { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // Employee Only
        [Display(Name = "Worker ID")]
        public string WorkerID { get; set; }

        [Required]
        [Display(Name = "Worker Type")]
        public string WorkerType { get; set; }

        [Required]
        [Display(Name = "Hire Type")]
        public string HireType { get; set; }

        [Display(Name = "Is Contingent")]
        public bool IsContingent { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime StartDate { get; set; }

        // Contingent Only
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? EndDate { get; set; }

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
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Office")]
        public string Office { get; set; }

        // Contingent Only
        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "License Profile")]
        public string O365License { get; set; }

        [Display(Name = "Computer Required")]
        public bool ComputerRequired { get; set; }

        [Display(Name = "Desk Phone")]
        public bool PhoneRequired { get; set; }

        [Display(Name = "Oracle Time & Expense")]
        public bool OracleRequired { get; set; }

        [Display(Name = "Rehire")]
        public bool Rehire { get; set; }

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

        [Display(Name = "Service Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime ServiceDate { get; set; }

        [Display(Name = "GUID")]
        public string GUID { get; set; }

        [Display(Name = "Service Request Sent")]
        public bool ServiceRequest { get; set; }

        [Display(Name = "Account Details Sent")]
        public bool AccountDetails { get; set; }

        [Display(Name = "Complete")]
        public bool Complete { get; set; }

        [Display(Name = "Error Log")]
        [DataType(DataType.MultilineText)]
        public string ErrorLog { get; set; }

        [NotMapped]
        [Display(Name = "Is Conversion")]
        public bool IsConversion { get; set; }

        [NotMapped]
        [Display(Name = "Logon Name")]
        public string LogonName { get; set; }

        [NotMapped]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name = "Suppress Notification")]
        public bool Suppress { get; set; }
        
        [Display(Name = "ITaaS")]
        public bool ITaaS { get; set; }

        internal static List<string> EmployeeHireTypes()
        {
            return new List<string>
            {
                "New Employee",
                "Employee Transfer",
                "Employee Conversion"
            };
        }

        internal static NewHire Contingent()
        {
            try
            {
                NewHire contingent = new NewHire();

                contingent.WorkerType = "Contingent Worker-StaffAug";
                contingent.HireType = "New Contractor";
                contingent.IsContingent = true;
                contingent.StartDate = DateTime.Now;
                contingent.Department = "Contingents";
                contingent.Country = "United States of America";

                contingent.SetServiceDate();

                return contingent;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static NewHire EmployeeLookup(string workerId, string hireType, SNAPContext db)
        {
            try
            {
                NewHire employee = new NewHire();

                dynamic workdayEmployee = PowerShell.RunScript("Get-WorkdayEmployee", new { WorkerID = workerId });

                employee.WorkerID = workerId;
                employee.HireType = hireType;

                if (employee.HireType == "Employee Conversion")
                {
                    employee.IsConversion = true;
                }

                switch (workdayEmployee.Worker_Type as string)
                {
                    case "Contractor":
                        employee.IsContingent = true;
                        employee.WorkerType = workdayEmployee.Worker_Type;
                        break;

                    case "Regular":
                        employee.WorkerType = "Employee-Regular";
                        employee.O365License = "E3, EMS";
                        employee.OracleRequired = true;
                        break;

                    default:
                        employee.WorkerType = workdayEmployee.Worker_Type;
                        employee.O365License = "E3, EMS";
                        employee.OracleRequired = true;
                        break;
                }

                employee.FirstName = workdayEmployee.First_Name;
                employee.LastName = workdayEmployee.Last_Name;
                employee.EmailAddress = workdayEmployee.Email;
                employee.StartDate = Convert.ToDateTime(workdayEmployee.Hire_Date);
                employee.Title = workdayEmployee.Title;
                employee.Department = workdayEmployee.Department;
                employee.ManagersEmail = workdayEmployee.Manager_Email;
                employee.Office = workdayEmployee.Office_Location;
                employee.Country = workdayEmployee.Country;

                if (db.Countries.Any(x => x.Name == employee.Country) == false)
                {
                    employee.Country = "United States of America";
                }

                employee.SetServiceDate();

                return employee;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal void ValidateContingent(ModelStateDictionary modelState, IPrincipal user)
        {
            if (EndDate == null)
            {
                modelState.AddModelError("EndDate", "End Date Required");
            }

            if (String.IsNullOrWhiteSpace(State))
            {
                modelState.AddModelError("State", "State Required");
            }

            Requester = user.Identity.Name;
            RequestedOn = DateTime.Now;

            ValidateManager(modelState);
            SetRequestersEmail(user);
            SetServiceDate();
        }

        internal void ValidateEmployee(ModelStateDictionary modelState, IPrincipal user)
        {
            if (String.IsNullOrWhiteSpace(WorkerID))
            {
                modelState.AddModelError("WorkerID", "Worker ID Required");
            }

            Requester = user.Identity.Name;
            RequestedOn = DateTime.Now;

            ValidateManager(modelState);
            SetRequestersEmail(user);
            SetServiceDate();
        }

        internal void ValidateManager(ModelStateDictionary modelState)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = ManagersEmail });

                Manager = exchangeRecipient.DisplayName;

                if (IsContingent == true && !String.IsNullOrWhiteSpace(exchangeRecipient.Department))
                {
                    Department = exchangeRecipient.Department;
                }
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
                //REVISIT - Failed to set New Hire Requester's Email
            }
        }

        internal void SetServiceDate()
        {
            ServiceDate = StartDate.AddDays(-7);

            if (DateTime.Today >= ServiceDate || HireType == "Employee Conversion")
            {
                ServiceDate = DateTime.Today;
            }
        }

        internal void SetEmailAddress()
        {
            Regex regex = new Regex("[^a-zA-Z]");

            string firstName = regex.Replace(FirstName, "").ToLower();
            string lastName = regex.Replace(LastName, "").ToLower();

            EmailAddress = String.Format("{0}.{1}@dimensiondata.com", firstName, lastName);
        }
    }
}