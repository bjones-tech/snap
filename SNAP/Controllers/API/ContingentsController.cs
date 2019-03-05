using SNAP.DAL;
using SNAP.Models.Helpers;
using SNAP.Models.Persistent;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace SNAP.Controllers.API
{
    public class ContingentsController : ApiController
    {
        private SNAPContext db = new SNAPContext();

        // GET: api/Contingents
        public IQueryable<Contingent> GetContingents()
        {
            return db.Contingents;
        }

        // PUT: api/Contingents/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutContingent(int id, Contingent contingent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contingent.ID)
            {
                return BadRequest();
            }

            db.Entry(contingent).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContingentExists(id))
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

        // POST: api/Contingents
        [ResponseType(typeof(Contingent))]
        public IHttpActionResult PostContingent(dynamic psObject)
        {
            try
            {
                ADUser adUser = ADUser.CreateFromDynamicObject(psObject);

                Contingent contingent = Contingent.Create(adUser);

                if (contingent != null)
                {
                    db.Contingents.Add(contingent);
                    db.SaveChanges();

                    return CreatedAtRoute("DefaultApi", new { id = contingent.ID }, adUser);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // DELETE: api/Contingents/{id}
        [ResponseType(typeof(Contingent))]
        public IHttpActionResult DeleteContingent(int id)
        {
            Contingent contingent = db.Contingents.Find(id);

            if (contingent == null)
            {
                return NotFound();
            }

            db.Contingents.Remove(contingent);
            db.SaveChanges();

            return Ok(contingent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContingentExists(int id)
        {
            return db.Contingents.Count(e => e.ID == id) > 0;
        }
    }
}