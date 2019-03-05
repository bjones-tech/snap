using SNAP.DAL;
using SNAP.Models.Persistent;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace SNAP.Controllers.API
{
    public class LastDaysController : ApiController
    {
        private SNAPContext db = new SNAPContext();

        // GET: api/LastDays
        public IQueryable<LastDay> GetLastDays()
        {
            return db.LastDays;
        }

        // PUT: api/LastDays/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLastDay(int id, LastDay lastDay)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != lastDay.ID)
            {
                return BadRequest();
            }

            db.Entry(lastDay).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LastDayExists(id))
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

        // DELETE: api/LastDays/5
        [ResponseType(typeof(LastDay))]
        public IHttpActionResult DeleteLastDay(int id)
        {
            LastDay lastDay = db.LastDays.Find(id);
            if (lastDay == null)
            {
                return NotFound();
            }

            db.LastDays.Remove(lastDay);
            db.SaveChanges();

            return Ok(lastDay);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LastDayExists(int id)
        {
            return db.LastDays.Count(e => e.ID == id) > 0;
        }
    }
}