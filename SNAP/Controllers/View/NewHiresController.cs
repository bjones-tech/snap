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
    public class NewHiresController : Controller
    {
        private SNAPContext db = new SNAPContext();

        public ActionResult Index(string firstName, string lastName)
        {
            List<NewHire> newHires = db.NewHires.Where(x => x.Requester == User.Identity.Name).OrderByDescending(x => x.StartDate).ToList();

            if (!String.IsNullOrWhiteSpace(firstName) || !String.IsNullOrWhiteSpace(lastName))
            {
                newHires = newHires.Where(x => x.FirstName.ToLower().StartsWith(firstName.ToLower()) && x.LastName.ToLower().StartsWith(lastName.ToLower())).ToList();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", newHires.Take(20));
            }

            return View(newHires.Take(20));
        }

        public ActionResult AllRequests(string firstName, string lastName)
        {
            List<NewHire> newHires = new List<NewHire>();

            if (UserRole.IsEmployeeAdmin(User))
            {
                newHires.AddRange(db.NewHires.Where(x => x.IsContingent == false).OrderByDescending(x => x.StartDate).ToList());
            }

            if (UserRole.IsContingentAdmin(User))
            {
                newHires.AddRange(db.NewHires.Where(x => x.IsContingent == true).OrderByDescending(x => x.StartDate).ToList());
            }

            if (!String.IsNullOrWhiteSpace(firstName) || !String.IsNullOrWhiteSpace(lastName))
            {
                newHires = newHires.Where(x => x.FirstName.ToLower().StartsWith(firstName.ToLower()) && x.LastName.ToLower().StartsWith(lastName.ToLower())).ToList();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", newHires.Take(20));
            }

            return View("Index", newHires.Take(20));
        }

        public ActionResult NewEmployeeLookup()
        {
            ViewBag.WorkerIDHelp = PowerShell.GetHTMLString("NewHires", "WorkerIDHelp", null);
            ViewBag.HireTypeGuidelines = PowerShell.GetHTMLString("NewHires", "HireTypeGuidelines", null);

            return View(NewHire.EmployeeHireTypes());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewEmployeeLookup(string workerId, string hireType)
        {
            ViewBag.WorkerIDHelp = PowerShell.GetHTMLString("NewHires", "WorkerIDHelp", null);
            ViewBag.HireTypeGuidelines = PowerShell.GetHTMLString("NewHires", "HireTypeGuidelines", null);

            if (String.IsNullOrWhiteSpace(workerId))
            {
                ModelState.AddModelError("WorkerID", "Worker ID Required");
                return View(NewHire.EmployeeHireTypes());
            }

            NewHire employee = NewHire.EmployeeLookup(workerId, hireType, db);

            if (employee == null)
            {
                ModelState.AddModelError("WorkerID", "Workday Lookup Error");
                return View(NewHire.EmployeeHireTypes());
            }

            if (employee.EmailAddress == null)
            {
                employee.SetEmailAddress();
            }

            ViewBag.Countries = db.Countries.Select(x => x.Name).Distinct();
            ViewBag.Offices = Office.GetOfficeNames(employee.Country, db);

            ViewBag.O365Profiles = O365Profile.GetSelectList();
            ViewBag.O365ProfileDetails = PowerShell.GetHTMLString("NewHires", "O365ProfileDetails", null);

            return View("NewEmployee", employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewEmployee(NewHire employee)
        {
            employee.ValidateEmployee(ModelState, User);

            //if (db.NewHires.Any(x => x.WorkerID == employee.WorkerID))
            //{
            //    ModelState.AddModelError("Request", String.Format("{0} Already Requested", employee.HireType));
            //}

            if (ModelState.IsValid)
            {
                NewHireNotice newHireNotice = new NewHireNotice(employee, "New");

                if (TryValidateModel(newHireNotice) == true)
                {
                    if (employee.Suppress == false)
                    {
                        Mailer mailer = new Mailer(MessageTemplate.NewHireEmployee, true);

                        mailer.SetFromAddress(employee.RequestersEmail);
                        mailer.AddRecipient(employee.ManagersEmail);

                        if (employee.ITaaS == true)
                        {
                            mailer.AddITaaSNotificationGroup();
                        }

                        mailer.SendMessage("NewHireNotice", newHireNotice, newHireNotice.Subject);
                    }
                    
                    db.NewHires.Add(employee);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("MailNotice", "Mail Notice Error");
            }

            ViewBag.Countries = db.Countries.Select(x => x.Name).Distinct();
            ViewBag.Offices = Office.GetOfficeNames(employee.Country, db);

            ViewBag.O365Profiles = O365Profile.GetSelectList();
            ViewBag.O365ProfileDetails = PowerShell.GetHTMLString("NewHires", "O365ProfileDetails", null);

            return View(employee);
        }

        public ActionResult NewContingent()
        {
            NewHire contingent = NewHire.Contingent();

            if (contingent == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Countries = db.Countries.Select(x => x.Name).Distinct();
            ViewBag.Offices = Office.GetOfficeNames(contingent.Country, db);

            ViewBag.O365Profiles = O365Profile.GetSelectList();
            ViewBag.O365ProfileDetails = PowerShell.GetHTMLString("NewHires", "O365ProfileDetails", null);

            return View(contingent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewContingent(NewHire contingent)
        {
            contingent.ValidateContingent(ModelState, User);

            if (db.NewHires.Any(x => x.FirstName == contingent.FirstName && x.LastName == contingent.LastName))
            {
                ModelState.AddModelError("Request", String.Format("{0} Already Requested", contingent.HireType));
            }

            if (ModelState.IsValid)
            {
                contingent.SetEmailAddress();

                NewHireNotice newHireNotice = new NewHireNotice(contingent, "New");

                if (TryValidateModel(newHireNotice) == true)
                {
                    Mailer mailer = new Mailer(MessageTemplate.NewHireContingent, true);

                    mailer.SetFromAddress(contingent.RequestersEmail);
                    mailer.AddRecipient(contingent.ManagersEmail);
                    mailer.SendMessage("NewHireNotice", newHireNotice, newHireNotice.Subject);

                    db.NewHires.Add(contingent);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("MailNotice", "Mail Notice Error");
            }

            ViewBag.Countries = db.Countries.Select(x => x.Name).Distinct();
            ViewBag.Offices = Office.GetOfficeNames(contingent.Country, db);

            ViewBag.O365Profiles = O365Profile.GetSelectList();
            ViewBag.O365ProfileDetails = PowerShell.GetHTMLString("NewHires", "O365ProfileDetails", null);

            return View(contingent);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NewHire newHire = db.NewHires.Find(id);

            if (newHire == null)
            {
                return HttpNotFound();
            }

            ViewBag.Countries = db.Countries.Select(x => x.Name).Distinct();
            ViewBag.Offices = Office.GetOfficeNames(newHire.Country, db);

            return View(newHire);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, NewHire newHire)
        {
            newHire.ValidateManager(ModelState);
            newHire.SetRequestersEmail(User);
            newHire.SetServiceDate();

            if (ModelState.IsValid)
            {
                NewHireNotice newHireNotice = new NewHireNotice(newHire, "Update");

                if (TryValidateModel(newHireNotice) == true)
                {
                    if (newHire.Suppress == false)
                    {
                        Mailer mailer = new Mailer(MessageTemplate.NewHireEmployee, true);

                        if (newHire.IsContingent == true)
                        {
                            mailer = new Mailer(MessageTemplate.NewHireContingent, true);
                        }

                        mailer.SetFromAddress(newHire.RequestersEmail);
                        mailer.AddRecipient(newHire.ManagersEmail);

                        if (newHire.ITaaS == true)
                        {
                            mailer.AddITaaSNotificationGroup();
                        }

                        mailer.SendMessage("NewHireNotice", newHireNotice, newHireNotice.Subject);
                    }

                    db.Entry(newHire).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("MailNotice", "Mail Notice Error");
            }

            ViewBag.Countries = db.Countries.Select(x => x.Name).Distinct();
            ViewBag.Offices = Office.GetOfficeNames(newHire.Country, db);

            return View(newHire);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NewHire newHire = db.NewHires.Find(id);

            if (newHire == null)
            {
                return HttpNotFound();
            }

            return View(newHire);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NewHire newHire = db.NewHires.Find(id);

            NewHireNotice newHireNotice = new NewHireNotice(newHire, "Cancel");

            if (TryValidateModel(newHireNotice) == true)
            {
                Mailer mailer = new Mailer(MessageTemplate.NewHireEmployee, true);

                if (newHire.IsContingent == true)
                {
                    mailer = new Mailer(MessageTemplate.NewHireContingent, true);
                }

                mailer.SetFromAddress(newHire.RequestersEmail);
                mailer.AddRecipient(newHire.ManagersEmail);
                mailer.SendMessage("NewHireNotice", newHireNotice, newHireNotice.Subject);

                db.NewHires.Remove(newHire);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("MailNotice", "Mail Notice Error");

            return View();
        }

        [Authorize(Roles = "AM.SNAP.Admins")]
        public ActionResult NewHireErrors()
        {
            return View(db.NewHires.Where(x => x.ErrorLog != null && x.ErrorLog != String.Empty));
        }

        [Authorize(Roles = "AM.SNAP.Admins")]
        public ActionResult NewHireFix(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NewHire newHire = db.NewHires.Find(id);

            if (newHire == null)
            {
                return HttpNotFound();
            }

            return View(newHire);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewHireFix(NewHire newHire)
        {
            if (ModelState.IsValid)
            {
                newHire.ErrorLog = null;

                db.Entry(newHire).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("NewHireErrors");
            }

            return View(newHire);
        }

        public ActionResult WorkerIDHelp()
        {
            return PartialView("_WorkerIDHelp");
        }

        public ActionResult HireTypeGuidelines()
        {
            return PartialView("_HireTypeGuidelines");
        }

        public ActionResult O365ProfileDetails()
        {
            List<O365Profile> o365Profiles = O365Profile.GetProfileList();

            return PartialView("_O365ProfileDetails", o365Profiles);
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
