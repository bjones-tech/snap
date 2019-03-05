using System.ComponentModel.DataAnnotations;
using SNAP.Models.Persistent;
using System;
using SNAP.DAL;
using System.Linq;

namespace SNAP.Models.Mail
{
    public class ServiceRequest
    {
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "NAME")]
        public string Name { get; set; }

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

        internal static ServiceRequest NewHire(NewHire newHire, SNAPContext db)
        {
            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();

                serviceRequest.Name = String.Format("{0} {1}", newHire.FirstName, newHire.LastName);
                serviceRequest.Title = newHire.Title;
                serviceRequest.Department = newHire.Department;
                serviceRequest.Manager = newHire.Manager;
                serviceRequest.Office = newHire.Office;
                serviceRequest.Notes = String.Format("{0}\n{1}", newHire.PublicNotes, newHire.PrivateNotes);

                Country country = db.Countries.Where(x => x.Name == newHire.Country).First();

                string workerType = "#CONT";

                if (newHire.WorkerType == "Employee-Regular")
                {
                    workerType = "#FTE";
                }

                string region = country.ISO2;

                if (newHire.ITaaS == true)
                {
                    region = "ITAAS";
                }

                serviceRequest.Subject = String.Format("AM.{0} SNAP New Hire | {1} | {2} | {3}",
                    region,
                    serviceRequest.Name,
                    newHire.StartDate.ToString("MMMM dd, yyyy"),
                    workerType);

                if (newHire.HireType == "Employee Transfer")
                {
                    serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#Transfer");
                }
                
                switch (newHire.O365License)
                {
                    case "E1":
                        serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#E1");
                        break;
                        
                    case "E1/Windows":
                        serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#Computer");
                        serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#E1");
                        break;

                    case "E3, EMS":
                        serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#Computer");
                        serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#E3");
                        break;

                    default:
                        break;
                }

                if (newHire.PhoneRequired == true)
                {
                    serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#Phone");
                }

                if (db.Offices.Any(o => o.Name == newHire.Office) == false)
                {
                    serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#Remote");
                }

                if (newHire.Department.Contains("Managed Services") && newHire.Title.Contains("Engineer"))
                {
                    serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#GSC");
                }

                if (newHire.Department.Contains("Solutions") || newHire.Department.Contains("PreSales"))
                {
                    serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#Solutions");
                }

                return serviceRequest;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static ServiceRequest LastDay(LastDay lastDay, SNAPContext db)
        {
            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();

                serviceRequest.Name = lastDay.Name;
                serviceRequest.Title = lastDay.Title;
                serviceRequest.Department = lastDay.Department;
                serviceRequest.Manager = lastDay.Manager;
                serviceRequest.Office = lastDay.Office;
                serviceRequest.Notes = String.Format("{0}\n{1}", lastDay.PublicNotes, lastDay.PrivateNotes);

                Country country = db.Countries.Where(x => x.ISO2 == lastDay.Country).First();

                string workerType = "#FTE";

                if (lastDay.IsContingent == true)
                {
                    workerType = "#CONT";
                }

                string region = country.ISO2;

                if (lastDay.ITaaS == true)
                {
                    region = "ITAAS";
                }

                serviceRequest.Subject = String.Format("AM.{0} SNAP Last Day | {1} | {2} | {3}",
                    region,
                    serviceRequest.Name,
                    Convert.ToDateTime(lastDay.EndDate).ToString("MMMM dd, yyyy"),
                    workerType);

                if (lastDay.Immediate == true)
                {
                    serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#Immediate");
                }

                if (db.Offices.Any(o => o.Name == lastDay.Office) == false)
                {
                    serviceRequest.Subject = String.Format("{0} {1}", serviceRequest.Subject, "#Remote");
                }

                return serviceRequest;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}