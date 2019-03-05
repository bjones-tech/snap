using SNAP.Models.Persistent;
using System;
using System.ComponentModel.DataAnnotations;

namespace SNAP.Models.Mail
{
    public class CandidateNotice
    {
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Heading")]
        public string Heading { get; set; }

        [Required]
        [Display(Name = "Candidate ID")]
        public int CandidateID { get; set; }

        [Required]
        [Display(Name = "CANDIDATE")]
        public string Name { get; set; }

        public CandidateNotice()
        {

        }

        public CandidateNotice(Candidate candidate)
        {
            Heading = "Candidate Interview Notice";
            Subject = String.Format("{0} | {1}", Heading, candidate.Name);
            Name = candidate.Name;
        }
    }
}