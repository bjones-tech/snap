using System;
using System.ComponentModel.DataAnnotations;
using SNAP.DAL;
using SNAP.Models.Helpers;
using System.Security.Principal;

namespace SNAP.Models.Persistent
{
    public class ChangeLog
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Timestamp")]
        public DateTime Timestamp { get; set; }

        [Required]
        [Display(Name = "Event")]
        public string Event { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Submitted By")]
        public string SubmittedBy { get; set; }

        internal static string DistributionGroup(DistributionGroup distributionGroup, SNAPContext db)
        {
            try
            {
                ChangeLog changeLog = new ChangeLog();

                changeLog.Timestamp = DateTime.Now;
                changeLog.Event = "Distribution Group Created";
                changeLog.Description = String.Format("@{0} created", distributionGroup.Alias);
                changeLog.SubmittedBy = distributionGroup.Creator;

                db.ChangeLogs.Add(changeLog);
                db.SaveChanges();

                return changeLog.Description;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static string ContactAccount(ContactAccount contactAccount, SNAPContext db)
        {
            try
            {
                ChangeLog changeLog = new ChangeLog();

                changeLog.Timestamp = DateTime.Now;
                changeLog.Event = "Contact Account Created";
                changeLog.Description = String.Format("{0} created", contactAccount.EmailAddress);
                changeLog.SubmittedBy = contactAccount.Creator;

                db.ChangeLogs.Add(changeLog);
                db.SaveChanges();

                return changeLog.Description;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static string PasswordReset(ADUser adUser, IPrincipal user, SNAPContext db)
        {
            try
            {
                ChangeLog changeLog = new ChangeLog();

                changeLog.Timestamp = DateTime.Now;
                changeLog.Event = "Password Reset";
                changeLog.Description = String.Format("Password reset for {0}", adUser.EmailAddress);
                changeLog.SubmittedBy = user.Identity.Name;

                db.ChangeLogs.Add(changeLog);
                db.SaveChanges();

                return changeLog.Description;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}