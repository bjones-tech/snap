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
    [Authorize(Roles = "AM.SNAP.Facilities")]
    public class OfficesController : Controller
    {
        private SNAPContext db = new SNAPContext();

        public ActionResult Index(string name, string leaseRange)
        {
            List<Office> offices = db.Offices.Include(o => o.Country).ToList();

            if (!String.IsNullOrWhiteSpace(name))
            {
                offices = offices.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            if (leaseRange == "<")
            {
                offices = offices.Where(x => Convert.ToDateTime(x.LeaseEndDate) <= DateTime.Today.AddYears(1) && x.LeaseEndDate != null).ToList();
            }

            if (leaseRange == ">")
            {
                offices = offices.Where(x => Convert.ToDateTime(x.LeaseEndDate) > DateTime.Today.AddYears(1) && x.LeaseEndDate != null).ToList();
            }

            List<SelectListItem> leaseRanges = new List<SelectListItem>();

            leaseRanges.Add(new SelectListItem
            {
                Text = "Lease ends under 1 year",
                Value = "<"
            });

            leaseRanges.Add(new SelectListItem
            {
                Text = "Lease ends after 1 year",
                Value = ">"
            });

            ViewBag.LeaseRanges = leaseRanges;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", offices);
            }

            return View(offices);
        }

        public ActionResult Create()
        {
            ViewBag.CountryID = new SelectList(db.Countries, "ID", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Office office)
        {
            if (ModelState.IsValid)
            {
                db.Offices.Add(office);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CountryID = new SelectList(db.Countries, "ID", "Name", office.CountryID);

            return View(office);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Office office = db.Offices.Find(id);

            if (office == null)
            {
                return HttpNotFound();
            }

            ViewBag.CountryID = new SelectList(db.Countries, "ID", "Name", office.CountryID);

            return View(office);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Office office)
        {
            if (ModelState.IsValid)
            {
                db.Entry(office).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CountryID = new SelectList(db.Countries, "ID", "Name", office.CountryID);

            return View(office);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Office office = db.Offices.Find(id);

            if (office == null)
            {
                return HttpNotFound();
            }

            return View(office);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Office office = db.Offices.Find(id);

            db.Offices.Remove(office);
            db.SaveChanges();

            return RedirectToAction("Index");
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
