using SNAP.DAL;
using SNAP.Models.Helpers;
using SNAP.Models.Mail;
using SNAP.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SNAP.Controllers.View
{
    [Authorize(Roles = "AM.SNAP.ITAdmins")]
    public class ITAdminController : Controller
    {
        private SNAPContext db = new SNAPContext();

        public ActionResult NewHires()
        {
            DateTime retentionDate = DateTime.Now.AddDays(-10);

            return View(db.NewHires.Where(x => x.Complete == false && x.StartDate >= retentionDate).OrderBy(x => x.StartDate));
        }

        public ActionResult CreateDistributionGroup()
        {
            ViewBag.Formats = DistributionGroup.Formats();
            ViewBag.Prefixes = DistributionGroup.Prefixes();
            ViewBag.Tooltip = PowerShell.GetHTMLString("ITAdmin", "DistributionGroupGuidelines", null);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDistributionGroup(DistributionGroup distributionGroup)
        {
            distributionGroup.Validate(ModelState, User);

            if (ModelState.IsValid)
            {
                if (distributionGroup.CreateDistributionGroup())
                {
                    string description = ChangeLog.DistributionGroup(distributionGroup, db);

                    ViewBag.Description = description;

                    return View("Success");
                }

                ModelState.AddModelError("Alias", "Failed to Create Distribution Group");
            }

            ViewBag.Formats = DistributionGroup.Formats();
            ViewBag.Prefixes = DistributionGroup.Prefixes();
            ViewBag.Tooltip = PowerShell.GetHTMLString("ITAdmin", "DistributionGroupGuidelines", null);

            return View(distributionGroup);
        }

        public ActionResult CreateContactAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContactAccount(ContactAccount contactAccount)
        {
            contactAccount.Validate(ModelState, User);

            if (ModelState.IsValid)
            {
                if (contactAccount.CreateContactAccount())
                {
                    string description = ChangeLog.ContactAccount(contactAccount, db);

                    ViewBag.Description = description;

                    return View("Success");
                }

                ModelState.AddModelError("EmailAddress", "Failed to Create Contact Account");
            }

            return View(contactAccount);
        }

        public ActionResult Shipments(string recipientsName)
        {
            List<Shipment> shipments = db.Shipments.OrderByDescending(x => x.ShippedOn).ToList();

            if (!String.IsNullOrWhiteSpace(recipientsName))
            {
                shipments = shipments.Where(x => x.RecipientsName.ToLower().Contains(recipientsName.ToLower())).ToList();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ShipmentsPartial", shipments.Take(20));
            }

            return View(shipments.Take(20));
        }

        public ActionResult ShipmentDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Shipment shipment = db.Shipments.Find(id);

            if (shipment == null)
            {
                return HttpNotFound();
            }

            return View(shipment);
        }

        public ActionResult SendShippingNotice()
        {
            ViewBag.Items = Shipment.Items();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendShippingNotice(Shipment shipment)
        {
            ViewBag.Items = Shipment.Items();

            shipment.Validate(ModelState, User);

            if (ModelState.IsValid)
            {
                ShippingNotice shippingNotice = new ShippingNotice(shipment);

                if (TryValidateModel(shippingNotice))
                {
                    Mailer mailer = new Mailer(MessageTemplate.Default, true);

                    mailer.SetFromAddress(shipment.ShippersEmail);
                    mailer.AddRecipient(shipment.RecipientsEmail);
                    mailer.AddRecipient(shipment.ManagersEmail);
                    mailer.SendMessage("ShippingNotice", shippingNotice, shippingNotice.Subject);

                    db.Shipments.Add(shipment);
                    db.SaveChanges();

                    ViewBag.Description = "Notice Sent";

                    return View("Success");
                }

                ModelState.AddModelError("MailNotice", "Mail Notice Error");
            }

            return View(shipment);
        }

        public ActionResult DistributionGroupGuidelines()
        {
            return PartialView("_DistributionGroupGuidelines");
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