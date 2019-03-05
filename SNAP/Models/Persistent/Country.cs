using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SNAP.Models.Persistent
{
    public class Country
    {
        public int ID { get; set; }

        public virtual ICollection<Office> Offices { get; set; }

        [Required]
        [Display(Name = "Country", AutoGenerateFilter = true)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "AD Name")]
        public string ADName { get; set; }

        [Required]
        [Display(Name = "ISO alpha-2")]
        public string ISO2 { get; set; }

        [Required]
        [Display(Name = "ISO alpha-3")]
        public string ISO3 { get; set; }
    }
}