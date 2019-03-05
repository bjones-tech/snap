using SNAP.Models.Persistent;
using System;
using System.ComponentModel.DataAnnotations;

namespace SNAP.Models.Mail
{
    public class InterviewNotice
    {
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Heading")]
        public string Heading { get; set; }

        [Required]
        [Display(Name = "Notice Type")]
        public string NoticeType { get; set; }

        [Required]
        [Display(Name = "Interview ID")]
        public int InterviewID { get; set; }

        [Required]
        [Display(Name = "Candidate ID")]
        public int CandidateID { get; set; }

        [Required]
        [Display(Name = "CANDIDATE")]
        public string Candidate { get; set; }
        
        [Display(Name = "ORGANIZER")]
        public string Organizer { get; set; }

        [Display(Name = "INTERVIEW DATE")]
        public string InterviewDate { get; set; }

        public InterviewNotice()
        {

        }

        public InterviewNotice(CandidateInterview interview, string noticeType)
        {
            NoticeType = noticeType;
            InterviewID = interview.ID;
            CandidateID = interview.CandidateID;
            Candidate = interview.Candidate.Name;
            Organizer = interview.OrganizersName;

            if (interview.InterviewDate != null)
            {
                InterviewDate = Convert.ToDateTime(interview.InterviewDate).ToString("MMMM dd, yyyy");
            }

            switch (NoticeType)
            {
                case "Appointment":
                    Heading = "New Interview Appointment";
                    Subject = String.Format("{0} Interview ({1}): {2} with {3}", interview.InterviewType, interview.Candidate.Title, interview.InterviewersName, interview.Candidate.Name);
                    break;

                case "MeetingRequest":
                    Heading = "New Interview Meeting Request";
                    Subject = String.Format("Meeting Request has been created for {0}", interview.Candidate.Name);
                    break;

                case "Evaluation":
                    Heading = "Interview Evaluation";
                    Subject = String.Format("Interview Evaluation for {0}", interview.Candidate.Name);
                    break;

                case "PastDue":
                    Heading = "Interview Evaluation Past Due";
                    Subject = String.Format("Interview Evaluation for {0} is Past Due", interview.Candidate.Name);
                    break;

                case "Complete":
                    Heading = "Interview Evaluation Complete";
                    Subject = String.Format("Interview Evaluation for {0} is Complete", interview.Candidate.Name);
                    break;

                default:
                    break;
            }
        }
    }
}