using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web.Mvc;

namespace SNAP.Models.Helpers
{
    public class ContactAccount
    {
        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string Company { get; set; }

        [Display(Name = "Distribution Group (Optional)")]
        public string DistributionGroup { get; set; }

        [Display(Name = "Creator")]
        public string Creator { get; set; }

        internal void Validate(ModelStateDictionary modelState, IPrincipal user)
        {
            Creator = user.Identity.Name;

            if (!String.IsNullOrWhiteSpace(DistributionGroup))
            {
                try
                {
                    dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = DistributionGroup });
                }
                catch (Exception)
                {
                    modelState.AddModelError("DistributionGroup", "Invalid Distribution Group");
                }
            }
        }

        internal bool CreateContactAccount()
        {
            try
            {
                PowerShell.RunScript("Create-ContactAccount", new { ContactAccount = this });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}