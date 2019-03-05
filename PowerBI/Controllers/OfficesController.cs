using PowerBI.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace PowerBI.Controllers
{
    [Authorize(Roles = "AM.SNAP.Admins")]
    public class OfficesController : ApiController
    {
        private AzureBIReportingContext db = new AzureBIReportingContext();

        // GET: api/Offices
        public IQueryable<Office> GetOffices()
        {
            return db.Offices;
        }

        // PUT: api/Offices/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOffice(int id, Office office)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != office.ID)
            {
                return BadRequest();
            }

            db.Entry(office).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfficeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Offices
        [ResponseType(typeof(Office))]
        public IHttpActionResult PostOffice(Office office)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Offices.Add(office);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = office.ID }, office);
        }

        // DELETE: api/Offices/5
        [ResponseType(typeof(Office))]
        public IHttpActionResult DeleteOffice(int id)
        {
            Office office = db.Offices.Find(id);
            if (office == null)
            {
                return NotFound();
            }

            db.Offices.Remove(office);
            db.SaveChanges();

            return Ok(office);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OfficeExists(int id)
        {
            return db.Offices.Count(e => e.ID == id) > 0;
        }
    }
}