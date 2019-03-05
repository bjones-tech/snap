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
    public class NewHiresController : ApiController
    {
        private SNAPContext db = new SNAPContext();

        // GET: api/NewHires
        public IQueryable<NewHire> GetNewHires()
        {
            return db.NewHires;
        }

        // PUT: api/NewHires/{id}
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNewHire(int id, NewHire newHire)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != newHire.ID)
            {
                return BadRequest();
            }

            db.Entry(newHire).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewHireExists(id))
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

        // DELETE: api/NewHires/{id}
        [ResponseType(typeof(NewHire))]
        public IHttpActionResult DeleteNewHire(int id)
        {
            NewHire newHire = db.NewHires.Find(id);
            if (newHire == null)
            {
                return NotFound();
            }

            db.NewHires.Remove(newHire);
            db.SaveChanges();

            return Ok(newHire);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NewHireExists(int id)
        {
            return db.NewHires.Count(e => e.ID == id) > 0;
        }
    }
}