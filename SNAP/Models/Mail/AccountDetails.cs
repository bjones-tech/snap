using System.ComponentModel.DataAnnotations;
using SNAP.Models.Persistent;
using System;

namespace SNAP.Models.Mail
{
    public class AccountDetails
    {
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "NAME")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "EMAIL ADDRESS")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "LOGON NAME")]
        public string LogonName { get; set; }

        [Required]
        [Display(Name = "PASSWORD")]
        public string Password { get; set; }

        internal static AccountDetails NewHire(NewHire newHire)
        {
            try
            {
                AccountDetails accountDetails = new AccountDetails();

                accountDetails.Name = String.Format("{0} {1}", newHire.FirstName, newHire.LastName);
                accountDetails.Subject = String.Format("{0} Update | {1} | {2}", newHire.HireType, accountDetails.Name, newHire.StartDate.ToString("MMMM dd, yyyy"));
                accountDetails.EmailAddress = newHire.EmailAddress;
                accountDetails.LogonName = newHire.LogonName;
                accountDetails.Password = newHire.Password;

                return accountDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}