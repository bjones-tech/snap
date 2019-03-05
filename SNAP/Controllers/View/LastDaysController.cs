using SNAP.DAL;
using SNAP.Models.Helpers;
using SNAP.Models.Mail;
using SNAP.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SNAP.Controllers.View
{
    [Authorize(Roles = "AM.SNAP.EmployeeAdmins, AM.SNAP.ContingentAdmins")]
    public class LastDaysController : Controller
    {
        private SNAPContext db = new SNAPContext();

        public ActionResult Index(string name)
        {
            List<LastDay> lastDays = db.LastDays.Where(x => x.Requester == User.Identity.Name).OrderByDescending(x => x.EndDate).ToList();

            if (!String.IsNullOrWhiteSpace(name))
            {
                lastDays = lastDays.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", lastDays.Take(20).Reverse());
            }

            return View(lastDays.Take(20).Reverse());
        }

        public ActionResult AllRequests(string name)
        {
            List<LastDay> lastDays = new List<LastDay>();

            if (UserRole.IsEmployeeAdmin(User))
            {
                lastDays.AddRange(db.LastDays.Where(x => x.IsContingent == false).OrderByDescending(x => x.EndDate).ToList());
            }

            if (UserRole.IsContingentAdmin(User))
            {
                lastDays.AddRange(db.LastDays.Where(x => x.IsContingent == true).OrderByDescending(x => x.EndDate).ToList());
            }

            if (!String.IsNullOrWhiteSpace(name))
            {
                lastDays = lastDays.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", lastDays.Take(20).Reverse());
            }

            return View("Index", lastDays.Take(20).Reverse());
        }

        public ActionResult LastDayLookup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LastDayLookup(string emailAddress)
        {
            if (String.IsNullOrWhiteSpace(emailAddress))
            {
                ModelState.AddModelError("EmailAddress", "Email Address Required");
                return View();
            }

            ModelState.Clear();

            LastDay lastDay = LastDay.Create(emailAddress);

            if (lastDay == null)
            {
                ModelState.AddModelError("EmailAddress", "User Lookup Error");
                return View(new { EmailAddress = emailAddress });
            }

            if (UserRole.IsContingentAdmin(User) && !UserRole.IsEmployeeAdmin(User) && lastDay.IsContingent == false)
            {
                ModelState.AddModelError("EmailAddress", "Invalid Contingent");
                return View(new { EmailAddress = emailAddress });
            }

            return View("LastDayRequest", lastDay);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LastDayRequest(LastDay lastDay)
        {
            lastDay.Validate(ModelState, User);

            //if (db.LastDays.Any(x => x.EmailAddress == lastDay.EmailAddress))
            //{
            //    ModelState.AddModelError("Request", "Last Day Already Requested");
            //}

            if (ModelState.IsValid)
            {
                LastDayNotice lastDayNotice = new LastDayNotice(lastDay, "New");

                if (TryValidateModel(lastDayNotice) == true)
                {
                    if (lastDay.Immediate == true)
                    {
                        lastDay.Decommission();
                    }
                    else
                    {
                        if (db.GSCUsers.Any(x => x.GUID == lastDay.GUID && x.Active == true))
                        {
                            foreach (GSCUser gscUser in db.GSCUsers.Where(x => x.GUID == lastDay.GUID && x.Active == true))
                            {
                                var gscClient = db.GSCClients.Find(gscUser.GSCClientID);

                                gscUser.GSCClient = gscClient;
                                gscUser.EndDate = lastDay.EndDate;
                                gscUser.MailExternalCompany();
                            }
                        }
                    }

                    if (lastDay.Suppress == false)
                    {
                        Mailer mailer = new Mailer(MessageTemplate.LastDayEmployee, true);

                        if (lastDay.IsContingent == true)
                        {
                            mailer = new Mailer(MessageTemplate.LastDayContingent, true);
                        }

                        mailer.SetFromAddress(lastDay.RequestersEmail);
                        mailer.AddRecipient(lastDay.ManagersEmail);

                        if (lastDay.ITaaS == true)
                        {
                            mailer.AddITaaSNotificationGroup();
                        }

                        mailer.SendMessage("LastDayNotice", lastDayNotice, lastDayNotice.Subject);
                    }

                    db.LastDays.Add(lastDay);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("MailNotice", "Mail Notice Error");
            }

            return View(lastDay);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LastDay lastDay = db.LastDays.Find(id);

            if (lastDay == null)
            {
                return HttpNotFound();
            }

            return View(lastDay);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, LastDay lastDay)
        {
            lastDay.ValidateManager(ModelState);
            lastDay.SetRequestersEmail(User);

            if (ModelState.IsValid)
            {
                LastDayNotice lastDayNotice = new LastDayNotice(lastDay, "Update");

                if (TryValidateModel(lastDayNotice) == true)
                {
                    if (lastDay.Immediate == true && (lastDay.Decommissioned == false || lastDay.ServiceRequest == false))
                    {
                        lastDay.Decommission();
                    }

                    if (lastDay.Suppress == false)
                    {
                        Mailer mailer = new Mailer(MessageTemplate.LastDayEmployee, true);

                        if (lastDay.IsContingent == true)
                        {
                            mailer = new Mailer(MessageTemplate.LastDayContingent, true);
                        }

                        mailer.SetFromAddress(lastDay.RequestersEmail);
                        mailer.AddRecipient(lastDay.ManagersEmail);

                        if (lastDay.ITaaS == true)
                        {
                            mailer.AddITaaSNotificationGroup();
                        }

                        mailer.SendMessage("LastDayNotice", lastDayNotice, lastDayNotice.Subject);
                    }

                    db.Entry(lastDay).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("MailNotice", "Mail Notice Error");
            }

            return View(lastDay);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LastDay lastDay = db.LastDays.Find(id);

            if (lastDay == null)
            {
                return HttpNotFound();
            }

            return View(lastDay);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LastDay lastDay = db.LastDays.Find(id);

            LastDayNotice lastDayNotice = new LastDayNotice(lastDay, "Cancel");

            if (TryValidateModel(lastDayNotice) == true)
            {
                if (lastDay.Suppress == false)
                {
                    Mailer mailer = new Mailer(MessageTemplate.LastDayEmployee, true);

                    if (lastDay.IsContingent == true)
                    {
                        mailer = new Mailer(MessageTemplate.LastDayContingent, true);
                    }

                    mailer.SetFromAddress(lastDay.RequestersEmail);
                    mailer.AddRecipient(lastDay.ManagersEmail);

                    if (lastDay.ITaaS == true)
                    {
                        mailer.AddITaaSNotificationGroup();
                    }

                    mailer.SendMessage("LastDayNotice", lastDayNotice, lastDayNotice.Subject);
                }

                db.LastDays.Remove(lastDay);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("MailNotice", "Mail Notice Error");

            return View();
        }

        [Authorize(Roles = "AM.SNAP.Admins")]
        public ActionResult LastDayErrors()
        {
            return View(db.LastDays.Where(x => x.ErrorLog != null && x.ErrorLog != String.Empty));
        }

        [Authorize(Roles = "AM.SNAP.Admins")]
        public ActionResult LastDayFix(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LastDay lastDay = db.LastDays.Find(id);

            if (lastDay == null)
            {
                return HttpNotFound();
            }

            return View(lastDay);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LastDayFix(LastDay lastDay)
        {
            if (ModelState.IsValid)
            {
                lastDay.ErrorLog = null;

                db.Entry(lastDay).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("LastDayErrors");
            }

            return View(lastDay);
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