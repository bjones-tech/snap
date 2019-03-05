using SNAP.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Principal;
using System.Web.Mvc;

namespace SNAP.Models.Persistent
{
    public class Shipment
    {
        public int ID { get; set; }

        [Display(Name = "Recipient")]
        public string RecipientsName { get; set; }

        [Required]
        [Display(Name = "Recipient's Email")]
        public string RecipientsEmail { get; set; }

        [Display(Name = "Manager")]
        public string ManagersName { get; set; }

        [Display(Name = "Manager's Email")]
        public string ManagersEmail { get; set; }

        [Display(Name = "Shipper")]
        public string ShippersName { get; set; }

        [Display(Name = "Shipper's Email")]
        public string ShippersEmail { get; set; }

        [Display(Name = "Shipped On")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime ShippedOn { get; set; }

        [Required]
        [Display(Name = "Item")]
        public string Item { get; set; }

        [Required]
        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        [Display(Name = "Tracking URL")]
        public string TrackingURL { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        internal static List<string> Items()
        {
            List<string> items = new List<string>();

            items.AddRange(new List<string>(){
                "Laptop",
                "Laptop w/ Accessories",
                "Monitor",
                "Docking Station"
            });

            return items;
        }

        internal void Validate(ModelStateDictionary modelState, IPrincipal user)
        {
            ShippedOn = DateTime.Today;

            ValidateShipper(modelState, user);
            ValidateRecipient(modelState);

            if (String.IsNullOrWhiteSpace(ManagersName))
            {
                ValidateManager(modelState, ManagersEmail);
            }

            if (!String.IsNullOrWhiteSpace(TrackingNumber))
            {
                TrackingNumber = TrackingNumber.ToUpper();
                TrackingURL = String.Format("http://www.fedex.com/Tracking?action=track&tracknumbers={0}", TrackingNumber);
            }
        }

        private void ValidateShipper(ModelStateDictionary modelState, IPrincipal user)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = user.Identity.Name });

                ShippersName = exchangeRecipient.DisplayName;
                ShippersEmail = exchangeRecipient.PrimarySmtpAddress;
            }
            catch (Exception)
            {
                modelState.AddModelError("ShippersEmail", "Invalid Shipper");
            }
        }

        private void ValidateRecipient(ModelStateDictionary modelState)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = RecipientsEmail });

                RecipientsName = exchangeRecipient.DisplayName;
                RecipientsEmail = exchangeRecipient.PrimarySmtpAddress;

                if (String.IsNullOrWhiteSpace(ManagersEmail))
                {
                    ValidateManager(modelState, exchangeRecipient.Manager);
                }
            }
            catch (Exception)
            {
                modelState.AddModelError("RecipientsEmail", "Invalid Recipient");
            }
        }

        private void ValidateManager(ModelStateDictionary modelState, string managersEmail)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = managersEmail });

                ManagersName = exchangeRecipient.DisplayName;
                ManagersEmail = exchangeRecipient.PrimarySmtpAddress;

                modelState.SetModelValue("ManagersEmail", new ValueProviderResult(ManagersEmail, "", CultureInfo.InvariantCulture));
            }
            catch (Exception)
            {
                modelState.AddModelError("ManagersEmail", "Invalid Manager");
            }
        }
    }
}