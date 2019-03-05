using SNAP.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SNAP.Models.Persistent
{
    public class Candidate
    {
        public int ID { get; set; }

        public virtual IList<CandidateInterview> Interviews { get; set; }

        [Display(Name = "Candidate", AutoGenerateFilter = true)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Requisition Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Requisition Number")]
        public string Number { get; set; }

        [Display(Name = "Hiring Manager")]
        public string Manager { get; set; }

        [Display(Name = "Hiring Manager")]
        public string ManagersName { get; set; }

        [Required]
        [Display(Name = "Hiring Manager's Email")]
        public string ManagersEmail { get; set; }

        [Display(Name = "Recruiter")]
        public string Recruiter { get; set; }

        [Display(Name = "Recruiter")]
        public string RecruitersName { get; set; }

        [Display(Name = "Recruiter's Email")]
        public string RecruitersEmail { get; set; }

        [Display(Name = "Requested On")]
        public DateTime RequestedOn { get; set; }

        [Display(Name = "Resume File Path")]
        public string ResumeFilePath { get; set; }

        [Display(Name = "Resume Extension")]
        public string ResumeFileName { get; set; }

        [Display(Name = "Candidate Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(Name = "Complete")]
        public bool Complete { get; set; }

        internal static Candidate CreateWithInterviewList()
        {
            try
            {
                Candidate candidate = new Candidate();

                candidate.Interviews = new List<CandidateInterview>();

                for (var i = 0; i < 10; i++)
                {
                    candidate.Interviews.Add(new CandidateInterview());
                }

                return candidate;
            }
            catch (Exception)
            {
                return new Candidate();
            }
        }

        internal void Validate(ModelStateDictionary modelState, IPrincipal user, HttpPostedFileBase file)
        {
            if (file == null)
            {
                modelState.AddModelError("Resume", "Resume Required");
            }

            if (file != null && file.ContentLength == 0)
            {
                modelState.AddModelError("Resume", "Invalid Resume File");
            }

            if (!String.IsNullOrWhiteSpace(FirstName) && !String.IsNullOrWhiteSpace(LastName))
            {
                Name = String.Format("{0} {1}", FirstName.Trim(), LastName.Trim());
            }

            Recruiter = user.Identity.Name;
            RequestedOn = DateTime.Now;

            ValidateManager(modelState);
            SetRecruitersEmail(user);
        }

        internal void ValidateManager(ModelStateDictionary modelState)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = ManagersEmail });

                Manager = String.Format("NA\\{0}", exchangeRecipient.SamAccountName);
                ManagersName = exchangeRecipient.DisplayName;
                ManagersEmail = exchangeRecipient.PrimarySmtpAddress;
            }
            catch (Exception)
            {
                modelState.AddModelError("ManagersEmail", "Invalid Manager");
            }
        }

        internal void SetRecruitersEmail(IPrincipal user)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = user.Identity.Name });

                RecruitersName = exchangeRecipient.DisplayName;
                RecruitersEmail = exchangeRecipient.PrimarySmtpAddress;
            }
            catch (Exception)
            {
                //REVISIT - Failed to set Candidate Requester's Email
            }
        }

        internal void SetResume(HttpPostedFileBase file)
        {
            try
            {
                Regex regex = new Regex("[^a-zA-Z]");

                string firstName = regex.Replace(FirstName, "").ToLower();
                string lastName = regex.Replace(LastName, "").ToLower();

                int extensionIndex = file.FileName.Split('.').Count() - 1;

                string fileExtension = file.FileName.Split('.')[extensionIndex];

                string fileName = String.Format(@"{0}_{1}_resume.{2}", firstName, lastName, fileExtension);

                string filePath = String.Format(@"{0}\{1}", ConfigurationManager.AppSettings["EVALUATIONS_DIRECTORY"], fileName);

                file.SaveAs(filePath);

                ResumeFilePath = filePath;
                ResumeFileName = fileName;
            }
            catch (Exception)
            {
                //REVISIT - Failed to set Resume
            }
        }

        internal bool IsAuthorized(IPrincipal user)
        {
            try
            {
                List<string> authorizedUsers = new List<string>();

                authorizedUsers.Add(Recruiter);
                authorizedUsers.Add(Manager);
                Interviews.ToList().ForEach(x => authorizedUsers.Add(x.Organizer));
                Interviews.ToList().ForEach(x => authorizedUsers.Add(x.Interviewer));

                return authorizedUsers.Contains(user.Identity.Name);
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal bool IsRecruiter(IPrincipal user)
        {
            return user.Identity.Name == Recruiter;
        }
    }
}