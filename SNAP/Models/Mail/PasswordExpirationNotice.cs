using System;
using System.ComponentModel.DataAnnotations;

namespace SNAP.Models.Mail
{
    public class PasswordExpirationNotice
    {
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Heading")]
        public string Heading { get; set; }

        [Required]
        [Display(Name = "COUNTRY")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "FIRST NAME")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "DAYS REMAINING")]
        public string DaysRemaining { get; set; }

        internal static PasswordExpirationNotice CreateFromDynamicObject(dynamic psObject)
        {
            try
            {
                PasswordExpirationNotice passwordExpirationNotice = new PasswordExpirationNotice();

                passwordExpirationNotice.Subject = "Password Expiration Notice | Please Read";
                passwordExpirationNotice.Heading = "Password Expiration Notice";
                passwordExpirationNotice.Country = psObject.Country;
                passwordExpirationNotice.FirstName = psObject.FirstName;
                passwordExpirationNotice.DaysRemaining = psObject.DaysRemaining;

                return passwordExpirationNotice;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}