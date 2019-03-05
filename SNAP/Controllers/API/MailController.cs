using SNAP.DAL;
using SNAP.Models.Helpers;
using SNAP.Models.Mail;
using SNAP.Models.Persistent;
using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace SNAP.Controllers.API
{
    public class MailController : ApiController
    {
        private SNAPContext db = new SNAPContext();

        [HttpPost]
        [Route("api/Mail/NewHireServiceRequest")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SendNewHireServiceRequest(NewHire newHire)
        {
            try
            {
                ServiceRequest serviceRequest = ServiceRequest.NewHire(newHire, db);

                if (serviceRequest != null)
                {
                    Mailer mailer = new Mailer(MessageTemplate.ServiceRequest, false);

                    if (newHire.ITaaS == true)
                    {
                        mailer = new Mailer(MessageTemplate.ITAASRequest, false);
                    }

                    mailer.SetFromAddress(newHire.ManagersEmail);
                    mailer.SendMessage("ServiceRequest", serviceRequest, serviceRequest.Subject);

                    return StatusCode(HttpStatusCode.NoContent);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("api/Mail/NewHireAccountDetails")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SendNewHireAccountDetails(NewHire newHire)
        {
            try
            {
                AccountDetails accountDetails = AccountDetails.NewHire(newHire);

                if (accountDetails != null)
                {
                    //Account Details
                    Mailer mailer = new Mailer(MessageTemplate.Default, true);

                    if (newHire.IsContingent == true)
                    {
                        mailer = new Mailer(MessageTemplate.Contingent, true);
                    }

                    mailer.AddRecipient(newHire.ManagersEmail);
                    mailer.SendMessage("AccountDetails", accountDetails, accountDetails.Subject);

                    //Password Notice
                    mailer = new Mailer(MessageTemplate.Default, false);

                    if (newHire.IsContingent == true)
                    {
                        mailer = new Mailer(MessageTemplate.Contingent, false);
                    }

                    mailer.AddRecipient(newHire.ManagersEmail);
                    mailer.AddBcc("AM.SNAP.Mail@dimensiondata.com");
                    mailer.SendMessage("PasswordNotice", accountDetails, accountDetails.Subject);

                    return StatusCode(HttpStatusCode.NoContent);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("api/Mail/LastDayServiceRequest")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SendLastDayServiceRequest(LastDay lastDay)
        {
            try
            {
                ServiceRequest serviceRequest = ServiceRequest.LastDay(lastDay, db);

                if (serviceRequest != null)
                {
                    Mailer mailer = new Mailer(MessageTemplate.ServiceRequest, false);

                    if (lastDay.ITaaS == true)
                    {
                        mailer = new Mailer(MessageTemplate.ITAASRequest, false);
                    }

                    mailer.SetFromAddress(lastDay.ManagersEmail);
                    mailer.SendMessage("ServiceRequest", serviceRequest, serviceRequest.Subject);

                    return StatusCode(HttpStatusCode.NoContent);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("api/Mail/EvaluationPastDueNotice")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SendEvaluationPastDueNotice(CandidateInterview interview)
        {
            try
            {
                interview.Candidate = db.Candidates.Find(interview.CandidateID);

                InterviewNotice pastDueNotice = new InterviewNotice(interview, "PastDue");

                if (pastDueNotice != null)
                {
                    Mailer mailer = new Mailer(MessageTemplate.Default, true);

                    mailer.AddRecipient(interview.InterviewersEmail);
                    mailer.SendMessage("InterviewNotice", pastDueNotice, pastDueNotice.Subject);

                    return StatusCode(HttpStatusCode.NoContent);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("api/Mail/ErrorNotice")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SendErrorNotice(dynamic psObject)
        {
            try
            {
                ErrorNotice errorNotice = ErrorNotice.CreateFromDynamicObject(psObject);

                if (errorNotice != null)
                {
                    Mailer mailer = new Mailer(MessageTemplate.ErrorNotice, false);

                    mailer.SendMessage("ErrorNotice", errorNotice, errorNotice.Subject);

                    return StatusCode(HttpStatusCode.NoContent);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("api/Mail/PasswordExpirationNotice")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SendPasswordExpirationNotice(dynamic psObject)
        {
            try
            {
                PasswordExpirationNotice passwordExpirationNotice = PasswordExpirationNotice.CreateFromDynamicObject(psObject);

                if (passwordExpirationNotice != null)
                {
                    Mailer mailer = new Mailer(MessageTemplate.Default, true);

                    string emailAddress = psObject.EmailAddress;

                    mailer.AddRecipient(emailAddress);
                    mailer.SendMessage("PasswordExpirationNotice", passwordExpirationNotice, passwordExpirationNotice.Subject);

                    return StatusCode(HttpStatusCode.NoContent);
                }

                return BadRequest();
            }
            catch (Exception)
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
    }
}
