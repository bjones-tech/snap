using SNAP.DAL;
using SNAP.Models.Helpers;
using SNAP.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace SNAP.Controllers.View
{
    [Authorize(Roles = "AM.SNAP.ITAdmins, AM.SNAP.PasswordAdmins")]
    public class ADUsersController : Controller
    {
        private SNAPContext db = new SNAPContext();

        public ActionResult Index(string firstName, string lastName)
        {
            List<ADUser> adUsers = new List<ADUser>();

            if (!String.IsNullOrWhiteSpace(firstName) || !String.IsNullOrWhiteSpace(lastName))
            {
                adUsers = ADUser.GetList(firstName, lastName);
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", adUsers);
            }

            return View(adUsers);
        }

        public ActionResult Edit(string guid)
        {
            if (guid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ADUser adUser = ADUser.CreateFromIdentity(guid);

            if (adUser == null)
            {
                return HttpNotFound();
            }

            return View(adUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ADUser adUser)
        {
            if (adUser.UpdateAccount() == false)
            {
                ModelState.AddModelError("AccountUpdate", "Failed To Update Account");

                return View(adUser);
            }

            return RedirectToAction("Index");
        }

        public ActionResult ResetPassword(string guid)
        {
            if (guid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ADUser adUser = ADUser.CreateFromIdentity(guid);

            if (adUser == null)
            {
                return HttpNotFound();
            }

            ViewBag.Tooltip = PowerShell.GetHTMLString("ADUsers", "PasswordPolicy", null);

            return View(adUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ADUser adUser, string password, string verifiedPassword)
        {
            if (adUser.ResetPassword(password, verifiedPassword, ModelState) == true)
            {
                string description = ChangeLog.PasswordReset(adUser, User, db);

                ViewBag.Description = description;

                return View("Success");
            }

            ViewBag.Tooltip = PowerShell.GetHTMLString("ADUsers", "PasswordPolicy", null);

            return View(adUser);
        }

        public ActionResult UnlockAccount(string guid)
        {
            ViewBag.Tooltip = PowerShell.GetHTMLString("ADUsers", "PasswordPolicy", null);

            ADUser adUser = ADUser.CreateFromIdentity(guid);

            if (adUser != null)
            {
                adUser.UnlockAccount();
            }

            return RedirectToAction("Edit", new { GUID = guid });
        }

        public ActionResult PasswordPolicy()
        {
            return PartialView("_PasswordPolicy");
        }

        public string GeneratePassword()
        {
            return ADUser.GeneratePassword();
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