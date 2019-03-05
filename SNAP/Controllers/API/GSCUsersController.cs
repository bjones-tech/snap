using SNAP.DAL;
using SNAP.Models.Persistent;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace SNAP.Controllers.API
{
    public class GSCUsersController : ApiController
    {
        private SNAPContext db = new SNAPContext();

        [HttpGet]
        [Route("api/GSCUsers")]
        public IQueryable<dynamic> GetGSCUsers()
        {
            return db.GSCUsers.Select(x => new
            {
                x.ID,
                x.Name,
                x.EmailAddress,
                x.GUID,
                x.Active
            });
        }

        [HttpPost]
        [Route("api/GSCUsers")]
        public string PostGSCUser(GSCUser gscUser)
        {
            if (!ModelState.IsValid)
            {
                return "Bad Request";
            }

            try
            {
                db.GSCUsers.Add(gscUser);
                db.SaveChanges();

                return null;
            }
            catch (Exception)
            {
                return "Failed to add GSC User";
            }
        }

        [HttpPut]
        [Route("api/GSCUsers/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGSCUser(int id, [FromBody]LastDay lastDay)
        {
            GSCUser gscUser = db.GSCUsers.Find(id);

            if (gscUser == null)
            {
                return NotFound();
            }

            gscUser.Decommission(db);

            return StatusCode(HttpStatusCode.NoContent);
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