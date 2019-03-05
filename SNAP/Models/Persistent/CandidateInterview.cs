using SNAP.Models.Helpers;
using SNAP.Models.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using System.Web.Mvc;

namespace SNAP.Models.Persistent
{
    public class CandidateInterview
    {
        public int ID { get; set; }
        public int CandidateID { get; set; }

        public virtual Candidate Candidate { get; set; }

        [Display(Name = "Interviewer")]
        public string Interviewer { get; set; }

        [Display(Name = "Interviewer")]
        public string InterviewersName { get; set; }

        [Display(Name = "Interviewer's Email")]
        public string InterviewersEmail { get; set; }

        [Display(Name = "Interview Date")]
        public DateTime? InterviewDate { get; set; }

        [Display(Name = "Interview Type")]
        public string InterviewType { get; set; }

        [Display(Name = "Organizer")]
        public string Organizer { get; set; }

        [Display(Name = "Organizer")]
        public string OrganizersName { get; set; }

        [Display(Name = "Organizer's Email")]
        public string OrganizersEmail { get; set; }

        [Display(Name = "General Appraisal")]
        public string GeneralAppraisal { get; set; }

        [Display(Name = "Technical Knowledge")]
        public string TechKnowledge { get; set; }

        [Display(Name = "Problem Solving")]
        public string ProblemSolving { get; set; }

        [Display(Name = "Teamwork")]
        public string Teamwork { get; set; }

        [Display(Name = "Communication")]
        public string Communication { get; set; }

        [Display(Name = "Cultural Fit")]
        public string CulturalFit { get; set; }

        [Display(Name = "Leadership")]
        public string Leadership { get; set; }

        [Display(Name = "Overall Strengths")]
        [DataType(DataType.MultilineText)]
        public string OverallStrengths { get; set; }

        [Display(Name = "Overall Concerns")]
        [DataType(DataType.MultilineText)]
        public string OverallConcerns { get; set; }

        [Display(Name = "Overall Evaluation")]
        [DataType(DataType.MultilineText)]
        public string OverallEvaluation { get; set; }

        [Display(Name = "Overall Rating")]
        public string OverallRating { get; set; }

        [Display(Name = "Recommendation")]
        public string Recommendation { get; set; }

        [Display(Name = "Appointment ID")]
        public string AppointmentID { get; set; }

        [Display(Name = "Interview Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(Name = "Complete")]
        public bool Complete { get; set; }

        [NotMapped]
        [Display(Name = "Evaluation Status")]
        public string EvaluationStatus { get; set; }

        [NotMapped]
        [Display(Name = "Evaluation Status Style")]
        public string EvaluationStatusStyle { get; set; }

        [NotMapped]
        [Display(Name = "Recommendation Style")]
        public string RecommendationStyle { get; set; }

        internal static List<string> InterviewTypes()
        {
            List<string> recommendations = new List<string>();

            recommendations.AddRange(new List<string>(){
                "In Person",
                "Phone Call",
                "WebEx Audio",
                "WebEx Video",
                "Web Conference"
            });

            return recommendations;
        }

        internal static List<string> Grades(bool optional)
        {
            List<string> grades = new List<string>();

            if (optional == true)
            {
                grades.Add("Not Applicable");
            }

            grades.AddRange(new List<string>(){
                "1 - Below Average",
                "2 - Average",
                "3 - Good",
                "4 - Very Good",
                "5 - Excellent"
            });

            return grades;
        }

        internal static List<string> Recommendations()
        {
            List<string> recommendations = new List<string>();

            recommendations.AddRange(new List<string>(){
                "Recommended",
                "Not Recommended",
                "Needs Further Evaluation"
            });

            return recommendations;
        }

        internal void ValidateInterviewer(ModelStateDictionary modelState)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = InterviewersEmail });

                Interviewer = String.Format("NA\\{0}", exchangeRecipient.SamAccountName);
                InterviewersName = String.Format("{0} {1}", exchangeRecipient.FirstName, exchangeRecipient.LastName);
                InterviewersEmail = exchangeRecipient.PrimarySmtpAddress;
            }
            catch (Exception)
            {
                modelState.AddModelError("InterviewersEmail", "Invalid Interviewer");
            }
        }

        private void ValidateOrganizer(ModelStateDictionary modelState)
        {
            try
            {
                dynamic exchangeRecipient = PowerShell.RunScript("Get-ExchangeRecipient", new { Identity = Organizer });

                OrganizersName = String.Format("{0} {1}", exchangeRecipient.FirstName, exchangeRecipient.LastName);
                OrganizersEmail = exchangeRecipient.PrimarySmtpAddress;
            }
            catch (Exception)
            {
                modelState.AddModelError("Organizer", "Invalid Organizer");
            }
        }

        internal void ValidateSchedule(ModelStateDictionary modelState, IPrincipal user)
        {
            if (InterviewDate == null)
            {
                modelState.AddModelError("InterviewDate", "Interview Date Required");
            }

            if (String.IsNullOrWhiteSpace(InterviewType))
            {
                modelState.AddModelError("InterviewType", "Interview Type Required");
            }

            Organizer = user.Identity.Name;

            ValidateOrganizer(modelState);
        }

        internal bool CreateAppointment()
        {
            try
            {
                string subject = new InterviewNotice(this, "Appointment").Subject;

                dynamic appointment = PowerShell.RunScript("Create-Appointment", new { ImpersonatedUser = OrganizersEmail, Subject = subject, StartDate = InterviewDate, Location = InterviewType });

                AppointmentID = appointment.ICalUid;
                InterviewDate = appointment.Start;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal void ValidateEvaluation(ModelStateDictionary modelState)
        {
            if (String.IsNullOrWhiteSpace(GeneralAppraisal))
            {
                modelState.AddModelError("GeneralAppraisal", "General Appraisal Required");
            }

            if (String.IsNullOrWhiteSpace(OverallRating))
            {
                modelState.AddModelError("OverallRating", "Overall Rating Required");
            }

            if (String.IsNullOrWhiteSpace(Recommendation))
            {
                modelState.AddModelError("Recommendation", "Recommendation Required");
            }
        }

        internal void SetStatusAndStyle()
        {
            SetEvaluationStatus();
            SetRecommendationStyle();
        }

        private void SetRecommendationStyle()
        {
            switch (Recommendation)
            {
                case "Recommended":
                    RecommendationStyle = "alert-success";
                    break;

                case "Not Recommended":
                    RecommendationStyle = "alert-danger";
                    break;

                case "Needs Further Evaluation":
                    RecommendationStyle = "alert-warning";
                    break;

                default:
                    break;
            }
        }

        private void SetEvaluationStatus()
        {
            if (Complete == true && String.IsNullOrWhiteSpace(Recommendation))
            {
                EvaluationStatus = "Cancelled";
                EvaluationStatusStyle = "alert-warning";

                return;
            }

            if (Complete == true)
            {
                EvaluationStatus = "Evaluation Complete";
                EvaluationStatusStyle = "alert-success";

                return;
            }

            if (InterviewDate == null)
            {
                EvaluationStatus = "Interview Not Scheduled";
                EvaluationStatusStyle = "alert-warning";

                return;
            }

            if (DateTime.Now < Convert.ToDateTime(InterviewDate))
            {
                EvaluationStatus = "Awaiting Interview";
                EvaluationStatusStyle = "alert-info";

                return;
            }

            if (DateTime.Now >= Convert.ToDateTime(InterviewDate))
            {
                EvaluationStatus = "Evaluation Past Due";
                EvaluationStatusStyle = "alert-danger";

                return;
            }

            EvaluationStatus = "Evaluation Past Due";
            EvaluationStatusStyle = "alert-danger";
        }

        internal void UpdateFromDynamicObject(dynamic psObject)
        {
            InterviewDate = psObject.InterviewDate;
            AppointmentID = psObject.AppointmentID;
            Complete = psObject.Complete;
        }

        internal bool IsAuthorized(IPrincipal user)
        {
            try
            {
                List<string> authorizedUsers = new List<string>();

                authorizedUsers.Add(Candidate.Recruiter);
                authorizedUsers.Add(Candidate.Manager);
                authorizedUsers.Add(Organizer);
                authorizedUsers.Add(Interviewer);

                return authorizedUsers.Contains(user.Identity.Name);
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal bool IsRecruiter(IPrincipal user)
        {
            return user.Identity.Name == Candidate.Recruiter;
        }

        internal bool IsInterviewer(IPrincipal user)
        {
            return user.Identity.Name == Interviewer;
        }

        internal bool IsOrganizer(IPrincipal user)
        {
            return user.Identity.Name == Organizer;
        }
    }
}