using PowerBI.Models;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace PowerBI.Controllers
{
    [Authorize(Roles = "AM.SNAP.Admins")]
    public class LenelAttendanceController : ApiController
    {
        private AzureBIReportingContext db = new AzureBIReportingContext();

        // GET: api/LenelAttendance
        public IQueryable<LenelAttendance> GetLenelAttendance()
        {
            return db.LenelAttendance;
        }

        // POST: api/LenelAttendance
        [ResponseType(typeof(LenelAttendance))]
        public IHttpActionResult PostLenelAttendance(LenelAttendance lenelAttendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LenelAttendance.Add(lenelAttendance);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = lenelAttendance.ID }, lenelAttendance);
        }

        // DELETE: api/LenelAttendance/5
        [ResponseType(typeof(LenelAttendance))]
        public IHttpActionResult DeleteLenelAttendance(int id)
        {
            LenelAttendance lenelAttendance = db.LenelAttendance.Find(id);
            if (lenelAttendance == null)
            {
                return NotFound();
            }

            db.LenelAttendance.Remove(lenelAttendance);
            db.SaveChanges();

            return Ok(lenelAttendance);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LenelAttendanceExists(int id)
        {
            return db.LenelAttendance.Count(e => e.ID == id) > 0;
        }
    }
}