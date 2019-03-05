using System.ComponentModel.DataAnnotations;
using SNAP.Models.Persistent;
using System;

namespace SNAP.Models.Mail
{
    public class NewHireNotice
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

        [Required]
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

        public NewHireNotice()
        {

        }

        public NewHireNotice(NewHire newHire, string noticeType)
        {
            if (newHire.HireType == "New Employee" && newHire.WorkerType != "Employee-Regular")
            {
                if (newHire.WorkerType.Contains("Contractor"))
                {
                    newHire.HireType = "New Contractor";
                }
                else if (newHire.WorkerType.Contains("Intern") && !newHire.WorkerType.Contains("Temporary"))
                {
                    newHire.HireType = "New Intern";
                }
            }

            switch (noticeType)
            {
                case "Update":
                    Heading = String.Format("{0} Notice Update", newHire.HireType);
                    break;

                case "Cancel":
                    Heading = String.Format("{0} Cancellation", newHire.HireType);
                    break;

                default:
                case "New":
                    Heading = String.Format("{0} Notice", newHire.HireType);
                    break;
            }
            
            Name = String.Format("{0} {1}", newHire.FirstName, newHire.LastName);
            Type = newHire.WorkerType;
            StartDate = newHire.StartDate.ToString("MMMM dd, yyyy");
            Title = newHire.Title;
            Department = newHire.Department;
            Manager = newHire.Manager;
            Office = newHire.Office;
            Notes = newHire.PublicNotes;

            Subject = String.Format("{0} | {1} | {2}", Heading, Name, StartDate);
        }
    }
}