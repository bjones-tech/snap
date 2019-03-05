using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web.Mvc;

namespace SNAP.Models.Helpers
{
    public class DistributionGroup
    {
        [Required]
        [Display(Name = "Group Format")]
        public int Format { get; set; }

        [Display(Name = "Alias")]
        public string Alias { get; set; }

        [Required]
        [Display(Name = "Alias Prefix")]
        public string AliasPrefix { get; set; }

        [Display(Name = "Alias 1")]
        public string Alias1 { get; set; }

        [Display(Name = "Alias 2")]
        public string Alias2 { get; set; }

        [Display(Name = "Preview")]
        public string Preview { get; set; }

        [Display(Name = "Manager")]
        public string Manager { get; set; }

        [Required]
        [Display(Name = "Manager's Email")]
        public string ManagersEmail { get; set; }

        [Display(Name = "Creator")]
        public string Creator { get; set; }

        [Display(Name = "GUID")]
        public string GUID { get; set; }

        internal static Dictionary<int, string> Formats()
        {
            Dictionary<int, string> purposes = new Dictionary<int, string>();

            purposes.Add(1, "Function for a Business Unit");
            purposes.Add(2, "Function for a Customer");
            purposes.Add(3, "Function only");

            return purposes;
        }

        internal static List<string> Prefixes()
        {
            List<string> prefixes = new List<string>();

            prefixes.AddRange(new List<string>(){
                "AM",
                "US",
                "CA",
                "MX",
                "BR",
                "CL",
                "Group"
            });

            return prefixes;
        }

        internal void Validate(ModelStateDictionary modelState, IPrincipal user)
        {
            Creator = user.Identity.Name;

            ValidateAlias(modelState);
            ValidateManager(modelState);
        }

        private void ValidateAlias(ModelStateDictionary modelState)
        {
            bool requiresSecondAlias = (Format == 1 || Format == 2);

            if (requiresSecondAlias == true && String.IsNullOrWhiteSpace(Alias2))
            {
                modelState.AddModelError("Alias", "Invalid Alias");
            }

            if (String.IsNullOrWhiteSpace(Alias1))
            {
                modelState.AddModelError("Alias", "Invalid Alias");
            }
        }

        internal void ValidateManager(ModelStateDictionary modelState)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = ManagersEmail });

                Manager = String.Format("NA\\{0}", exchangeRecipient.SamAccountName);
            }
            catch (Exception)
            {
                modelState.AddModelError("ManagersEmail", "Invalid Manager");
            }
        }

        internal bool CreateDistributionGroup()
        {
            try
            {
                PowerShell.RunScript("Create-DistributionGroup", new { DistributionGroup = this });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}