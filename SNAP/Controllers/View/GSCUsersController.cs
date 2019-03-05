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
    [Authorize(Roles = "AM.SNAP.GSCUserAdmins")]
    public class GSCUsersController : Controller
    {
        private SNAPContext db = new SNAPContext();

        public ActionResult Index(string client, string name)
        {
            List<GSCUser> gscUsers = db.GSCUsers.Where(x => x.Active == true).ToList();

            if (!String.IsNullOrWhiteSpace(client))
            {
                gscUsers = gscUsers.Where(x => x.GSCClient.Name == client).ToList();
            }

            if (!String.IsNullOrWhiteSpace(name))
            {
                gscUsers = gscUsers.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            ViewBag.Clients = db.GSCClients.Select(x => x.Name).ToList();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", gscUsers);
            }

            return View(gscUsers);
        }

        public ActionResult Separated()
        {
            return View(db.GSCUsers.Where(x => x.Active == false).ToList());
        }

        public ActionResult Clients()
        {
            return View(db.GSCClients.ToList());
        }

        public ActionResult CreateClient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClient(GSCClient gscClient)
        {
            if (ModelState.IsValid)
            {
                db.GSCClients.Add(gscClient);
                db.SaveChanges();

                return RedirectToAction("Clients");
            }

            return View(gscClient);
        }

        public ActionResult Create()
        {
            ViewBag.GSCClientID = new SelectList(db.GSCClients, "ID", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GSCUser gscUser)
        {
            GSCClient gscClient = db.GSCClients.Find(gscUser.GSCClientID);

            if (gscClient == null)
            {
                return HttpNotFound();
            }

            ModelState.Clear();

            gscUser.Validate(ModelState, gscUser.EmailAddress, gscClient);

            if (ModelState.IsValid)
            {
                db.GSCUsers.Add(gscUser);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.GSCClientID = new SelectList(db.GSCClients, "ID", "Name", gscClient.ID);

            return View(gscUser);
        }

        public ActionResult EditClient(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GSCClient gscClient = db.GSCClients.Find(id);

            if (gscClient == null)
            {
                return HttpNotFound();
            }

            return View(gscClient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClient(GSCClient gscClient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gscClient).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Clients");
            }

            return View(gscClient);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GSCUser gscUser = db.GSCUsers.Find(id);

            if (gscUser == null)
            {
                return HttpNotFound();
            }

            GSCClient gscClient = db.GSCClients.Find(gscUser.GSCClientID);

            if (gscClient == null)
            {
                return HttpNotFound();
            }

            ViewBag.GSCClientID = new SelectList(db.GSCClients, "ID", "Name", gscClient.ID);

            return View(gscUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GSCUser gscUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gscUser).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            GSCClient gscClient = db.GSCClients.Find(gscUser.GSCClientID);

            if (gscClient == null)
            {
                return HttpNotFound();
            }

            ViewBag.GSCClientID = new SelectList(db.GSCClients, "ID", "Name", gscClient.ID);

            return View(gscUser);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GSCUser gscUser = db.GSCUsers.Find(id);

            if (gscUser == null)
            {
                return HttpNotFound();
            }

            return View(gscUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GSCUser gscUser = db.GSCUsers.Find(id);

            gscUser.Decommission(db);

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
