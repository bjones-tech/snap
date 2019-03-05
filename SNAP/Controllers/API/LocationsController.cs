using SNAP.DAL;
using SNAP.Models.Persistent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace SNAP.Controllers.API
{
    public class LocationsController : ApiController
    {
        private SNAPContext db = new SNAPContext();

        [HttpGet]
        [Route("api/Locations/Countries")]
        public IQueryable<dynamic> GetCountries()
        {
            return db.Countries.Select(x => new
            {
                x.ID,
                x.Name,
                x.ADName,
                x.ISO2,
                x.ISO3
            });
        }

        [HttpGet]
        [Route("api/Locations/Offices")]
        public IQueryable<dynamic> GetOffices()
        {
            return db.Offices.Select(x => new
            {
                x.ID,
                x.CountryID,
                x.Name,
                x.StreetAddress,
                x.City,
                x.State,
                x.PostalCode,
                x.GeoCoordinates,
                x.ADPath,
                x.ADGroupPrefix,
                x.Networks,
                x.LenelPanelID,
                x.Type,
                x.Landlord,
                x.SecDeposit,
                x.Seating,
                x.LeaseStartDate,
                x.LeaseEndDate,
                x.MonthlyRent,
                x.SquareFootage,
                x.RentPerSqFoot
            });
        }

        [HttpPut]
        [Route("api/Locations/Offices/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOffice(int id, dynamic psObject)
        {
            try
            {
                Office office = db.Offices.Find(id);

                office.GeoCoordinates = psObject.GeoCoordinates;
                office.Type = psObject.Type;
                office.Landlord = psObject.Landlord;
                office.SecDeposit = psObject.SecDeposit;
                office.Seating = psObject.Seating;
                office.LeaseStartDate = psObject.LeaseStartDate;
                office.LeaseEndDate = psObject.LeaseEndDate;
                office.MonthlyRent = psObject.MonthlyRent;
                office.SquareFootage = psObject.SquareFootage;
                office.RentPerSqFoot = psObject.RentPerSqFoot;

                db.Entry(office).State = EntityState.Modified;
                db.SaveChanges();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("api/Locations/OfficeNames/{countryName}")]
        public IEnumerable<string> GetOfficeNames(string countryName)
        {
            return Office.GetOfficeNames(countryName, db);
        }

        [HttpGet]
        [Route("api/Locations/OfficeState/{officeName}")]
        public string GetOfficeState(string officeName)
        {
            Office office = db.Offices.Where(x => x.Name == officeName).FirstOrDefault();

            if (office == null)
            {
                return null;
            }

            return office.State;
        }

        [HttpGet]
        [Route("api/Locations/CountryISO2/{countryName}")]
        public string GetCountryISO2(string countryName)
        {
            Country country = db.Countries.Where(x => x.Name == countryName).FirstOrDefault();

            if (country == null)
            {
                return null;
            }

            return country.ISO2;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}