using System.ComponentModel.DataAnnotations;
using SNAP.Models.Persistent;
using System;

namespace SNAP.Models.Mail
{
    public class LastDayNotice
    {
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Heading")]
        public string Heading { get; set; }

        [Required]
        [Display(Name = "NAME")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "TYPE")]
        public string Type { get; set; }
        
        [Display(Name = "CODE")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "END DATE")]
        public string EndDate { get; set; }

        [Display(Name = "START DATE")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "TITLE")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "DEPARTMENT")]
        public string Department { get; set; }

        [Required]
        [Display(Name = "MANAGER")]
        public string Manager { get; set; }

        [Required]
        [Display(Name = "OFFICE")]
        public string Office { get; set; }

        [Display(Name = "NOTES")]
        public string Notes { get; set; }

        public LastDayNotice()
        {

        }

        public LastDayNotice(LastDay lastDay, string noticeType)
        {
            string prefix = "Last Day";

            if (lastDay.IsContingent == true)
            {
                prefix = "Assignment End";
            }

            switch (noticeType)
            {
                case "Update":
                    Heading = String.Format("{0} Notice Update", prefix);
                    break;

                case "Cancel":
                    Heading = String.Format("{0} Cancellation", prefix);
                    break;

                default:
                case "New":
                    Heading = String.Format("{0} Notice", prefix);
                    break;
            }
            
            Name = lastDay.Name;
            Type = lastDay.WorkerType;
            Code = lastDay.Code;
            EndDate = Convert.ToDateTime(lastDay.EndDate).ToString("MMMM dd, yyyy");
            Title = lastDay.Title;
            Department = lastDay.Department;
            Manager = lastDay.Manager;
            Office = lastDay.Office;
            Notes = lastDay.PublicNotes;

            if (lastDay.StartDate != null)
            {
                StartDate = Convert.ToDateTime(lastDay.StartDate).ToString("MMMM dd, yyyy");
            }

            Subject = String.Format("{0} | {1} | {2}", Heading, Name, EndDate);
        }
    }
}