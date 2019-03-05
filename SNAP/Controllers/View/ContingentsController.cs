using SNAP.DAL;
using SNAP.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SNAP.Controllers.View
{
    [Authorize(Roles = "AM.SNAP.ContingentAdmins")]
    public class ContingentsController : Controller
    {
        private SNAPContext db = new SNAPContext();

        public ActionResult Index(string firstName, string lastName)
        {
            List<Contingent> contingents = db.Contingents.OrderBy(x => x.LastName).ToList();

            if (!String.IsNullOrWhiteSpace(firstName) || !String.IsNullOrWhiteSpace(lastName))
            {
                contingents = contingents.Where(x => x.FirstName.ToLower().StartsWith(firstName.ToLower()) && x.LastName.ToLower().StartsWith(lastName.ToLower())).ToList();
            }

            contingents.ForEach(x => x.SetStatusStyle());

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", contingents.Take(20));
            }

            return View(contingents.Take(20));
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contingent contingent = db.Contingents.Find(id);

            if (contingent == null)
            {
                return HttpNotFound();
            }

            return View(contingent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contingent contingent)
        {
            contingent.Validate(ModelState);

            if (ModelState.IsValid)
            {
                if (contingent.UpdateAccount() == false)
                {
                    ModelState.AddModelError("AccountUpdate", "Failed To Update Account");

                    return View(contingent);
                }

                db.Entry(contingent).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(contingent);
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