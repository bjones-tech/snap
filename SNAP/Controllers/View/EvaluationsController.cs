using SNAP.DAL;
using SNAP.Models.Helpers;
using SNAP.Models.Mail;
using SNAP.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SNAP.Controllers.View
{
    public class EvaluationsController : Controller
    {
        private SNAPContext db = new SNAPContext();

        public ActionResult Index()
        {
            return View("Candidates", db.Candidates.OrderByDescending(x => x.RequestedOn).Take(20).ToList());
        }

        public ActionResult InterviewSearch(string candidateName, string reqTitle, string reqNumber)
        {
            ViewBag.Titles = db.Candidates.Select(x => x.Title).Distinct();
            ViewBag.Numbers = db.Candidates.Select(x => x.Number).Distinct();

            List<Candidate> candidates = new List<Candidate>();

            if (!String.IsNullOrWhiteSpace(candidateName) || !String.IsNullOrWhiteSpace(reqTitle) || !String.IsNullOrWhiteSpace(reqNumber))
            {
                candidates = db.Candidates.ToList().Where(x => x.Name.ToLower().Contains(candidateName.ToLower()) && x.Title.ToLower().Contains(reqTitle.ToLower()) && (x.Number.ToLower().Contains(reqNumber.ToLower()))).ToList();
            }

            List<CandidateInterview> interviews = new List<CandidateInterview>();

            foreach (Candidate candidate in candidates)
            {
                foreach (CandidateInterview interview in candidate.Interviews)
                {
                    interviews.Add(interview);
                }
            }

            interviews.ForEach(x => x.SetStatusAndStyle());

            if (Request.IsAjaxRequest())
            {
                return PartialView("_InterviewSearchPartial", interviews);
            }

            return View(interviews);
        }

        public ActionResult Unscheduled()
        {
            List<CandidateInterview> interviews = db.CandidateInterviews
                .Where(x => x.InterviewDate == null && x.Complete == false)
                .OrderBy(x => x.Interviewer).ToList();

            interviews.ForEach(x => x.SetStatusAndStyle());

            ViewBag.Title = "Unscheduled Interviews";

            return View("Interviews", interviews);
        }

        public ActionResult PastDue()
        {
            List<CandidateInterview> interviews = db.CandidateInterviews
                .Where(x => x.Complete == false && DateTime.Now >= x.InterviewDate && (x.Recommendation == null || x.Recommendation.Equals(string.Empty)))
                .OrderBy(x => x.Interviewer).ToList();

            interviews.ForEach(x => x.SetStatusAndStyle());

            ViewBag.Title = "Past Due Evaluations";

            return View("Interviews", interviews);
        }

        public ActionResult Cancelled()
        {
            List<CandidateInterview> interviews = db.CandidateInterviews
                .Where(x => x.Complete == true && DateTime.Now >= x.InterviewDate && (x.Recommendation == null || x.Recommendation.Equals(string.Empty)))
                .OrderBy(x => x.Interviewer).ToList();

            interviews.ForEach(x => x.SetStatusAndStyle());

            ViewBag.Title = "Cancelled Auto-Reminders";

            return View("Interviews", interviews);
        }

        [Authorize(Roles = "AM.SNAP.EvaluationAdmins, AM.SNAP.CandidateRecruiters")]
        public ActionResult NewCandidate()
        {
            return View(Candidate.CreateWithInterviewList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCandidate(Candidate candidate, HttpPostedFileBase file)
        {
            candidate.Validate(ModelState, User, file);

            List<CandidateInterview> interviews = candidate.Interviews.ToList();

            interviews.RemoveAll(x => String.IsNullOrWhiteSpace(x.InterviewersEmail));

            interviews.ForEach(x => x.ValidateInterviewer(ModelState));

            if (interviews.Count() == 0)
            {
                ModelState.AddModelError("InterviewersEmail", "Minimum of 1 Interviewer Required");
            }

            if (ModelState.IsValid)
            {
                candidate.SetResume(file);
                candidate.Interviews = interviews;

                CandidateNotice candidateNotice = new CandidateNotice(candidate);

                if (TryValidateModel(candidateNotice))
                {
                    db.Candidates.Add(candidate);
                    db.SaveChanges();

                    candidateNotice.CandidateID = candidate.ID;

                    Mailer mailer = new Mailer(MessageTemplate.Evaluation, true);

                    mailer.SetFromAddress(candidate.RecruitersEmail);
                    mailer.AddRecipient(candidate.RecruitersEmail);
                    mailer.AddRecipient(candidate.ManagersEmail);
                    candidate.Interviews.ToList().ForEach(x => mailer.AddRecipient(x.InterviewersEmail));
                    mailer.SendMessage("CandidateNotice", candidateNotice, candidateNotice.Subject);

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("MailNotice", "Mail Notice Error");
            }

            return View(candidate);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Candidate candidate = db.Candidates.Find(id);

            if (candidate == null)
            {
                return HttpNotFound();
            }

            if (candidate.IsAuthorized(User) == false && UserRole.IsEvalutionAdmin(User) == false)
            {
                return View("Unauthorized");
            }

            candidate.Interviews.ToList().ForEach(x => x.SetStatusAndStyle());

            return View(candidate);
        }

        public ActionResult AddInterview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Candidate candidate = db.Candidates.Find(id);

            if (candidate == null)
            {
                return HttpNotFound();
            }

            if (candidate.IsRecruiter(User) == false && UserRole.IsEvalutionAdmin(User) == false)
            {
                return View("Unauthorized");
            }

            ViewBag.Heading = String.Format("Candidate: {0} {1}", candidate.FirstName, candidate.LastName);

            return View(new CandidateInterview() { CandidateID = candidate.ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInterview(CandidateInterview interview)
        {
            interview.ValidateInterviewer(ModelState);

            Candidate candidate = db.Candidates.Find(interview.CandidateID);

            if (candidate == null)
            {
                return HttpNotFound();
            }

            CandidateNotice candidateNotice = new CandidateNotice(candidate);

            if (ModelState.IsValid)
            {
                db.CandidateInterviews.Add(interview);
                db.SaveChanges();

                candidateNotice.CandidateID = candidate.ID;

                Mailer mailer = new Mailer(MessageTemplate.Evaluation, true);

                mailer.SetFromAddress(candidate.RecruitersEmail);
                mailer.AddRecipient(candidate.RecruitersEmail);
                mailer.AddRecipient(candidate.ManagersEmail);
                mailer.AddRecipient(interview.InterviewersEmail);
                mailer.SendMessage("CandidateNotice", candidateNotice, candidateNotice.Subject);

                return RedirectToAction("Details", new { id = interview.CandidateID });
            }

            ViewBag.Heading = String.Format("Candidate: {0} {1}", candidate.FirstName, candidate.LastName);

            return View(interview);
        }

        public ActionResult Schedule(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CandidateInterview interview = db.CandidateInterviews.Find(id);

            if (interview == null)
            {
                return HttpNotFound();
            }

            if (interview.IsRecruiter(User) == false && UserRole.IsEvalutionAdmin(User) == false)
            {
                return View("Unauthorized");
            }

            ViewBag.InterviewTypes = CandidateInterview.InterviewTypes();

            return View(interview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Schedule(CandidateInterview interview)
        {
            interview.ValidateSchedule(ModelState, User);

            interview.Candidate = db.Candidates.Find(interview.CandidateID);

            if (ModelState.IsValid)
            {
                if (interview.CreateAppointment())
                {
                    InterviewNotice scheduleNotice = new InterviewNotice(interview, "MeetingRequest");

                    if (TryValidateModel(scheduleNotice))
                    {
                        Mailer mailer = new Mailer(MessageTemplate.Default, true);

                        mailer.AddRecipient(interview.OrganizersEmail);
                        mailer.SendMessage("InterviewNotice", scheduleNotice, scheduleNotice.Subject);
                    }

                    InterviewNotice evaluationNotice = new InterviewNotice(interview, "Evaluation");

                    if (TryValidateModel(evaluationNotice))
                    {
                        Mailer mailer = new Mailer(MessageTemplate.Default, true);

                        mailer.AddRecipient(interview.InterviewersEmail);
                        mailer.SendMessage("InterviewNotice", evaluationNotice, evaluationNotice.Subject);
                    }

                    db.Entry(interview).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = interview.CandidateID });
                }

                ModelState.AddModelError("MeetingRequest", "Meeting Request Error");
            }

            ViewBag.InterviewTypes = CandidateInterview.InterviewTypes();

            return View(interview);
        }

        public ActionResult Evaluation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CandidateInterview interview = db.CandidateInterviews.Find(id);

            if (interview == null)
            {
                return HttpNotFound();
            }

            if (interview.IsAuthorized(User) == false && UserRole.IsEvalutionAdmin(User) == false)
            {
                return View("Unauthorized");
            }

            ViewBag.Grades = CandidateInterview.Grades(false);
            ViewBag.OptionalGrades = CandidateInterview.Grades(true);
            ViewBag.Recommendations = CandidateInterview.Recommendations();

            if (interview.IsInterviewer(User) == false)
            {
                return View("EvaluationReadOnly", interview);
            }

            return View(interview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Evaluation(CandidateInterview interview)
        {
            interview.ValidateEvaluation(ModelState);

            interview.Candidate = db.Candidates.Find(interview.CandidateID);

            if (ModelState.IsValid)
            {
                interview.Complete = true;

                InterviewNotice completionNotice = new InterviewNotice(interview, "Complete");

                if (TryValidateModel(completionNotice))
                {
                    Mailer mailer = new Mailer(MessageTemplate.Evaluation, true);

                    mailer.SetFromAddress(interview.InterviewersEmail);
                    mailer.AddRecipient(interview.OrganizersEmail);
                    mailer.AddRecipient(interview.Candidate.RecruitersEmail);
                    mailer.AddRecipient(interview.Candidate.ManagersEmail);
                    mailer.SendMessage("InterviewNotice", completionNotice, completionNotice.Subject);

                    db.Entry(interview).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = interview.CandidateID });
                }

                ModelState.AddModelError("MailNotice", "Mail Notice Error");
            }

            interview.Candidate = db.Candidates.Find(interview.CandidateID);

            ViewBag.Grades = CandidateInterview.Grades(false);
            ViewBag.OptionalGrades = CandidateInterview.Grades(true);
            ViewBag.Recommendations = CandidateInterview.Recommendations();

            return View(interview);
        }

        public ActionResult SendReminder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CandidateInterview interview = db.CandidateInterviews.Find(id);

            if (interview == null)
            {
                return HttpNotFound();
            }

            if (interview.IsRecruiter(User) == false)
            {
                return View("Unauthorized");
            }

            interview.Candidate = db.Candidates.Find(interview.CandidateID);

            InterviewNotice pastDueNotice = new InterviewNotice(interview, "PastDue");

            if (pastDueNotice != null)
            {
                Mailer mailer = new Mailer(MessageTemplate.Default, true);

                mailer.AddRecipient(interview.InterviewersEmail);
                mailer.SendMessage("InterviewNotice", pastDueNotice, pastDueNotice.Subject);

                return Content(String.Format("Reminder has been sent to {0}", interview.InterviewersName));
            }

            return Content("Failed to send reminder");
        }

        public ActionResult CancelReminder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CandidateInterview interview = db.CandidateInterviews.Find(id);

            if (interview == null)
            {
                return HttpNotFound();
            }

            if (interview.IsRecruiter(User) == false && interview.IsOrganizer(User) == false)
            {
                return View("Unauthorized");
            }

            try
            {
                interview.Complete = true;

                db.Entry(interview).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("PastDue");
            }
            catch (Exception)
            {
                return Content("Failed to cancel reminders");
            }
        }

        public ActionResult CancelInterview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CandidateInterview interview = db.CandidateInterviews.Find(id);

            if (interview == null)
            {
                return HttpNotFound();
            }

            return View(interview);
        }

        [HttpPost, ActionName("CancelInterview")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelInterviewConfirmed(int id)
        {
            CandidateInterview interview = db.CandidateInterviews.Find(id);

            db.CandidateInterviews.Remove(interview);
            db.SaveChanges();

            return RedirectToAction("Unscheduled");
        }

        public ActionResult DownloadResume(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Candidate candidate = db.Candidates.Find(id);

            if (candidate == null)
            {
                return HttpNotFound();
            }

            if (candidate.IsAuthorized(User) == false && UserRole.IsEvalutionAdmin(User) == false)
            {
                return View("Unauthorized");
            }

            try
            {
                return File(candidate.ResumeFilePath, "application/force-download", candidate.ResumeFileName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult DownloadLibraryOfQuestions()
        {
            try
            {
                string fileName = "library_of_questions.xlsx";

                string filePath = String.Format(@"{0}\{1}", ConfigurationManager.AppSettings["EVALUATIONS_DIRECTORY"], fileName);

                return File(filePath, "application/force-download", fileName);
            }
            catch (Exception)
            {
                return null;
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