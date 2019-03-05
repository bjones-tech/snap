using SNAP.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SNAP.Models.Persistent
{
    public class Office
    {
        public int ID { get; set; }
        public int CountryID { get; set; }

        public virtual Country Country { get; set; }

        [Required]
        [Display(Name = "Office")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Street Address")]
        [DataType(DataType.MultilineText)]
        public string StreetAddress { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Geo Coordinates")]
        public string GeoCoordinates { get; set; }

        [Required]
        [Display(Name = "AD Path")]
        public string ADPath { get; set; }

        [Required]
        [Display(Name = "AD Group Prefix")]
        public string ADGroupPrefix { get; set; }

        [Display(Name = "Networks")]
        public string Networks { get; set; }

        [Display(Name = "Lenel Panel ID")]
        public int LenelPanelID { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Landlord")]
        public string Landlord { get; set; }

        [Display(Name = "Security Deposit")]
        [DataType(DataType.Currency)]
        public float SecDeposit { get; set; }

        [Display(Name = "Seating")]
        public int Seating { get; set; }

        [Display(Name = "Lease Start Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? LeaseStartDate { get; set; }

        [Display(Name = "Lease End Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? LeaseEndDate { get; set; }

        [Display(Name = "Monthly Rent")]
        [DataType(DataType.Currency)]
        public float MonthlyRent { get; set; }

        [Display(Name = "Square Footage")]
        public float SquareFootage { get; set; }

        [Display(Name = "Rent Per Square Foot")]
        [DataType(DataType.Currency)]
        public float RentPerSqFoot { get; set; }

        internal static List<string> GetOfficeNames(string countryName, SNAPContext db)
        {
            try
            {
                List<string> officeNames = new List<string>();

                Country country = db.Countries.Where(x => x.Name == countryName).FirstOrDefault();

                if (country == null)
                {
                    return officeNames;
                }

                officeNames.Add(String.Format("{0}, Home Office", country.ISO3));
                officeNames.Add(String.Format("{0}, Client Site", country.ISO3));

                foreach (Office office in db.Offices.Where(x => x.Country.Name == countryName))
                {
                    officeNames.Add(office.Name);
                }

                return officeNames;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}