using PowerBI.Models;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace PowerBI.Controllers
{
    [Authorize(Roles = "AM.SNAP.Admins")]
    public class NetworkActivityController : ApiController
    {
        private AzureBIReportingContext db = new AzureBIReportingContext();

        // GET: api/NetworkActivity
        public IQueryable<NetworkActivity> GetNetworkActivity()
        {
            return db.NetworkActivity;
        }

        // POST: api/NetworkActivity
        [ResponseType(typeof(NetworkActivity))]
        public IHttpActionResult PostNetworkActivity(NetworkActivity networkActivity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.NetworkActivity.Add(networkActivity);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = networkActivity.ID }, networkActivity);
        }

        // DELETE: api/NetworkActivity/5
        [ResponseType(typeof(NetworkActivity))]
        public IHttpActionResult DeleteNetworkActivity(int id)
        {
            NetworkActivity networkActivity = db.NetworkActivity.Find(id);
            if (networkActivity == null)
            {
                return NotFound();
            }

            db.NetworkActivity.Remove(networkActivity);
            db.SaveChanges();

            return Ok(networkActivity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NetworkActivityExists(int id)
        {
            return db.NetworkActivity.Count(e => e.ID == id) > 0;
        }
    }
}