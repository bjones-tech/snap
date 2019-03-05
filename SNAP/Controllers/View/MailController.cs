using SNAP.Models.Mail;
using SNAP.Models.Persistent;
using System;
using System.Web.Mvc;

namespace SNAP.Controllers.View
{
    public class MailController : Controller
    {
        [HttpPost]
        public ActionResult NewHireNotice(NewHireNotice newHireNotice)
        {
            return View(newHireNotice);
        }

        [HttpPost]
        public ActionResult LastDayNotice(LastDayNotice lastDayNotice)
        {
            return View(lastDayNotice);
        }

        [HttpPost]
        public ActionResult ServiceRequest(ServiceRequest serviceRequest)
        {
            return View(serviceRequest);
        }

        [HttpPost]
        public ActionResult AccountDetails(AccountDetails accountDetails)
        {
            return View(accountDetails);
        }

        [HttpPost]
        public ActionResult PasswordNotice(AccountDetails accountDetails)
        {
            return Content(String.Format("PASSWORD: {0}", accountDetails.Password));
        }

        [HttpPost]
        public ActionResult ErrorNotice(ErrorNotice errorNotice)
        {
            return Content(String.Format("ERROR: {0}", errorNotice.ErrorMessage));
        }

        [HttpPost]
        public ActionResult PasswordExpirationNotice(PasswordExpirationNotice passwordExpirationNotice)
        {
            return View(passwordExpirationNotice);
        }

        [HttpPost]
        public ActionResult CandidateNotice(CandidateNotice candidateNotice)
        {
            return View(candidateNotice);
        }

        [HttpPost]
        public ActionResult InterviewNotice(InterviewNotice interviewNotice)
        {
            return View(interviewNotice);
        }

        [HttpPost]
        public ActionResult ShippingNotice(ShippingNotice shippingNotice)
        {
            return View(shippingNotice);
        }

        [HttpPost]
        public ActionResult GSCUserNotice(GSCUser gscUser)
        {
            return View(gscUser);
        }
    }
}