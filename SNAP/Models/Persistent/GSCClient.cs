using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SNAP.Models.Persistent
{
    public class GSCClient
    {
        public int ID { get; set; }

        public virtual ICollection<GSCUser> GSCUsers { get; set; }

        [Required]
        [Display(Name = "Client")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Internal Email Address")]
        public string InternalEmailAddress { get; set; }
    }
}