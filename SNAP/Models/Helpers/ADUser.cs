using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SNAP.Models.Helpers
{
    public class ADUser
    {
        [Display(Name = "GUID")]
        public string GUID { get; set; }
        
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Display(Name = "Given Name")]
        public string GivenName { get; set; }

        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Manager")]
        public string Manager { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Office")]
        public string Office { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Employee Number")]
        public string EmployeeNumber { get; set; }

        [Display(Name = "Employee Type")]
        public string EmployeeType { get; set; }

        [Display(Name = "O365 License")]
        public string O365License { get; set; }

        [Display(Name = "Locked Out")]
        public bool LockedOut { get; set; }

        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? Created { get; set; }

        [Display(Name = "Account Expiration Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? AccountExpirationDate { get; set; }

        internal static ADUser CreateFromDynamicObject(dynamic psObject)
        {
            try
            {
                ADUser adUser = new ADUser();

                adUser.GUID = psObject.ObjectGUID.ToString();
                adUser.EmailAddress = psObject.EmailAddress;
                adUser.DisplayName = psObject.DisplayName;
                adUser.GivenName = psObject.GivenName;
                adUser.Surname = psObject.Surname;
                adUser.Title = psObject.Title;
                adUser.Department = psObject.Department;
                adUser.Manager = psObject.Manager;
                adUser.Country = psObject.Country;
                adUser.Office = psObject.Office;
                adUser.State = psObject.State;
                adUser.EmployeeNumber = psObject.EmployeeNumber;
                adUser.EmployeeType = psObject.EmployeeType;
                adUser.O365License = psObject.O365License;
                adUser.LockedOut = psObject.LockedOut;
                adUser.AccountExpirationDate = psObject.AccountExpirationDate;
                adUser.Created = psObject.Created;

                if (adUser.AccountExpirationDate != null)
                {
                    adUser.AccountExpirationDate = Convert.ToDateTime(adUser.AccountExpirationDate).AddDays(-1);
                }

                return adUser;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static ADUser CreateFromIdentity(string identity)
        {
            try
            {
                dynamic psObject = PowerShell.RunScript("Get-ADUser", new { Identity = identity });

                ADUser adUser = CreateFromDynamicObject(psObject);

                return adUser;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static List<ADUser> GetList(string firstName, string lastName)
        {
            try
            {
                List<ADUser> adUsers = new List<ADUser>();

                dynamic psObjectArray = PowerShell.RunScript("Get-ADUserList", new { FirstName = firstName, LastName = lastName });

                foreach (dynamic psObject in psObjectArray)
                {
                    ADUser adUser = CreateFromDynamicObject(psObject);

                    adUsers.Add(adUser);
                }

                return adUsers;
            }
            catch (Exception)
            {
                return new List<ADUser>();
            }
        }

        internal static string GeneratePassword()
        {
            try
            {
                return PowerShell.RunScript("Generate-Password");
            }
            catch (Exception)
            {
                return null;
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

        internal bool UnlockAccount()
        {
            try
            {
                PowerShell.RunScript("Unlock-ADUser", new { Attributes = this });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal bool ResetPassword(string password, string verifiedPassword, ModelStateDictionary modelState)
        {
            try
            {
                if (password != verifiedPassword)
                {
                    modelState.AddModelError("Password", "Passwords do not match");

                    return false;
                }

                if (password.Length < 8)
                {
                    modelState.AddModelError("Password", "Password does not meet length requirement");

                    return false;
                }

                PowerShell.RunScript("Reset-ADUserPassword", new { Attributes = this, Password = password });

                return true;
            }
            catch (Exception)
            {
                modelState.AddModelError("Password", "Failed To Reset Password");

                return false;
            }
        }
    }
}