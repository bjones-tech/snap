using System.ComponentModel.DataAnnotations;
using SNAP.Models.Persistent;
using System;

namespace SNAP.Models.Mail
{
    public class ShippingNotice
    {
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Heading")]
        public string Heading { get; set; }

        [Required]
        [Display(Name = "RECIPIENT")]
        public string Recipient { get; set; }

        [Required]
        [Display(Name = "ITEM")]
        public string Item { get; set; }

        [Required]
        [Display(Name = "TRACK SHIPMENT")]
        public string TrackingURL { get; set; }

        [Display(Name = "NOTES")]
        public string Notes { get; set; }

        public ShippingNotice()
        {

        }

        public ShippingNotice(Shipment shipment)
        {
            Recipient = shipment.RecipientsName;
            Item = shipment.Item;
            TrackingURL = shipment.TrackingURL;
            Notes = shipment.Notes;

            Heading = "Shipping Notice";
            Subject = String.Format("{0} | {1} shipped to {2}", Heading, Item, Recipient);
        }
    }
}