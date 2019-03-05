using PowerBI.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace PowerBI.Controllers
{
    public class SCCMController : ApiController
    {
        private AzureBIReportingContext db = new AzureBIReportingContext();

        [HttpGet]
        [Route("api/SCCM/WindowsServers/Template")]
        [Authorize(Roles = "AM.SNAP.ReadOnly")]
        public SCCMWindowsServer GetSCCMWindowsServer()
        {
            return new SCCMWindowsServer();
        }

        [HttpGet]
        [Route("api/SCCM/WindowsServers")]
        [Authorize(Roles = "AM.SNAP.ReadOnly")]
        public IQueryable<SCCMWindowsServer> GetSCCMWindowsServers()
        {
            return db.SCCMWindowsServers;
        }

        [HttpPut]
        [Route("api/SCCM/WindowsServers/{id}")]
        [Authorize(Roles = "AM.SNAP.Admins")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSCCMWindowsServer(int id, SCCMWindowsServer sCCMWindowsServer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sCCMWindowsServer.ID)
            {
                return BadRequest();
            }

            db.Entry(sCCMWindowsServer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SCCMWindowsServerExists(id))
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

        [HttpPost]
        [Route("api/SCCM/WindowsServers")]
        [Authorize(Roles = "AM.SNAP.Admins")]
        [ResponseType(typeof(SCCMWindowsServer))]
        public IHttpActionResult PostSCCMWindowsServer(SCCMWindowsServer sCCMWindowsServer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SCCMWindowsServers.Add(sCCMWindowsServer);
            db.SaveChanges();

            return Ok(sCCMWindowsServer);
        }

        [HttpDelete]
        [Route("api/SCCM/WindowsServers/{id}")]
        [Authorize(Roles = "AM.SNAP.Admins")]
        [ResponseType(typeof(SCCMWindowsServer))]
        public IHttpActionResult DeleteSCCMWindowsServer(int id)
        {
            SCCMWindowsServer sCCMWindowsServer = db.SCCMWindowsServers.Find(id);
            if (sCCMWindowsServer == null)
            {
                return NotFound();
            }

            db.SCCMWindowsServers.Remove(sCCMWindowsServer);
            db.SaveChanges();

            return Ok(sCCMWindowsServer);
        }

        [HttpGet]
        [Route("api/SCCM/WindowsWorkstations")]
        [Authorize(Roles = "AM.SNAP.ReadOnly")]
        public IQueryable<SCCMWindowsWorkstation> GetSCCMWindowsWorkstations()
        {
            return db.SCCMWindowsWorkstations;
        }

        [HttpPut]
        [Route("api/SCCM/WindowsWorkstations/{id}")]
        [Authorize(Roles = "AM.SNAP.Admins")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSCCMWindowsWorkstation(int id, SCCMWindowsWorkstation sCCMWindowsWorkstation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sCCMWindowsWorkstation.ID)
            {
                return BadRequest();
            }

            db.Entry(sCCMWindowsWorkstation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SCCMWindowsWorkstationExists(id))
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

        [HttpPost]
        [Route("api/SCCM/WindowsWorkstations")]
        [Authorize(Roles = "AM.SNAP.Admins")]
        [ResponseType(typeof(SCCMWindowsWorkstation))]
        public IHttpActionResult PostSCCMWindowsWorkstation(SCCMWindowsWorkstation sCCMWindowsWorkstation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SCCMWindowsWorkstations.Add(sCCMWindowsWorkstation);
            db.SaveChanges();

            return Ok(sCCMWindowsWorkstation);
        }

        [HttpDelete]
        [Route("api/SCCM/WindowsWorkstations/{id}")]
        [Authorize(Roles = "AM.SNAP.Admins")]
        [ResponseType(typeof(SCCMWindowsWorkstation))]
        public IHttpActionResult DeleteSCCMWindowsWorkstation(int id)
        {
            SCCMWindowsWorkstation sCCMWindowsWorkstation = db.SCCMWindowsWorkstations.Find(id);
            if (sCCMWindowsWorkstation == null)
            {
                return NotFound();
            }

            db.SCCMWindowsWorkstations.Remove(sCCMWindowsWorkstation);
            db.SaveChanges();

            return Ok(sCCMWindowsWorkstation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SCCMWindowsServerExists(int id)
        {
            return db.SCCMWindowsServers.Count(e => e.ID == id) > 0;
        }

        private bool SCCMWindowsWorkstationExists(int id)
        {
            return db.SCCMWindowsWorkstations.Count(e => e.ID == id) > 0;
        }
    }
}
