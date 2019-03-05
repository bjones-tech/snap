using System.ComponentModel.DataAnnotations;
using SNAP.Models.Persistent;
using System;

namespace SNAP.Models.Mail
{
    public class ErrorNotice
    {
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Error Message")]
        public string ErrorMessage { get; set; }

        internal static ErrorNotice CreateFromDynamicObject(dynamic psObject)
        {
            try
            {
                ErrorNotice errorNotice = new ErrorNotice();

                errorNotice.Subject = psObject.Subject;
                errorNotice.ErrorMessage = psObject.ErrorMessage;

                if (String.IsNullOrWhiteSpace(errorNotice.Subject) || String.IsNullOrWhiteSpace(errorNotice.ErrorMessage))
                {
                    throw new Exception();
                }

                return errorNotice;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}