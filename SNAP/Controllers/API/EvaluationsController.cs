using SNAP.DAL;
using SNAP.Models.Persistent;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace SNAP.Controllers.API
{
    public class EvaluationsController : ApiController
    {
        private SNAPContext db = new SNAPContext();

        [HttpGet]
        [Route("api/Evaluations/Interviews")]
        public IQueryable<dynamic> GetInterviews()
        {
            return db.CandidateInterviews.Select(x => new
            {
                x.ID,
                x.CandidateID,
                x.InterviewDate,
                x.InterviewersEmail,
                x.OrganizersEmail,
                x.AppointmentID,
                x.Recommendation,
                x.Complete
            });
        }

        [HttpPut]
        [Route("api/Evaluations/Interviews/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutInterview(int id, dynamic psObject)
        {
            try
            {
                CandidateInterview interview = db.CandidateInterviews.Find(id);

                interview.UpdateFromDynamicObject(psObject);

                db.Entry(interview).State = EntityState.Modified;
                db.SaveChanges();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch
            {
                return BadRequest();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool InterviewExists(int id)
        {
            return db.CandidateInterviews.Count(e => e.ID == id) > 0;
        }
    }
}