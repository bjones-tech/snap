using SNAP.DAL;
using SNAP.Models.Helpers;
using SNAP.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SNAP.Controllers.View
{
    public class ReportsController : Controller
    {
        private SNAPContext db = new SNAPContext();

        public ActionResult WindowsWorkstations(string hostname, string username, string serialNumber)
        {
            List<Computer> windowsWorkstations = Computer.GetComputerList("Workstations");

            if (!String.IsNullOrWhiteSpace(hostname)) { windowsWorkstations = windowsWorkstations.Where(x => x.Hostname.ToLower().Contains(hostname.ToLower())).ToList(); }
            if (!String.IsNullOrWhiteSpace(username)) { windowsWorkstations = windowsWorkstations.Where(x => !String.IsNullOrWhiteSpace(x.Username) && x.Username.ToLower().Contains(username.ToLower())).ToList(); }
            if (!String.IsNullOrWhiteSpace(serialNumber)) { windowsWorkstations = windowsWorkstations.Where(x => !String.IsNullOrWhiteSpace(x.SerialNumber) && x.SerialNumber.ToLower().Contains(serialNumber.ToLower())).ToList(); }
            
            ViewBag.Count = windowsWorkstations.Count();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_WindowsWorkstationsPartial", windowsWorkstations.Take(20));
            }

            return View("WindowsWorkstations", windowsWorkstations.Take(20));
        }

        public ActionResult WindowsServersDiscovered(string hostname, string description, string managedBy, string maintenanceWindow, string patchMethod)
        {
            List<Computer> windowsServers = Computer.GetComputerList("ServersDiscovered");

            if (!String.IsNullOrWhiteSpace(hostname)) { windowsServers = windowsServers.Where(x => x.Hostname.ToLower().Contains(hostname.ToLower())).ToList(); }
            if (!String.IsNullOrWhiteSpace(description)) { windowsServers = windowsServers.Where(x => !String.IsNullOrWhiteSpace(x.Description) && x.Description.ToLower().Contains(description.ToLower())).ToList(); }
            if (!String.IsNullOrWhiteSpace(managedBy)) { windowsServers = windowsServers.Where(x => !String.IsNullOrWhiteSpace(x.ManagedBy) && x.ManagedBy.ToLower().Contains(managedBy.ToLower())).ToList(); }
            if (!String.IsNullOrWhiteSpace(maintenanceWindow)) { windowsServers = windowsServers.Where(x => x.MaintenanceWindow.ToLower().Contains(maintenanceWindow.ToLower())).ToList(); }

            switch (patchMethod)
            {
                case "Auto":
                    windowsServers = windowsServers.Where(x => x.AutoPatch == true).ToList();
                    break;

                case "Manual":
                    windowsServers = windowsServers.Where(x => x.AutoPatch == false).ToList();
                    break;
            }

            ViewBag.ManagedBy = windowsServers.Where(x => !String.IsNullOrWhiteSpace(x.ManagedBy)).OrderBy(x => x.ManagedBy).Select(x => x.ManagedBy).Distinct();
            ViewBag.MaintenanceWindows = windowsServers.OrderBy(x => x.MaintenanceWindow).Select(x => x.MaintenanceWindow).Distinct();
            ViewBag.PatchMethod = new List<string> { "Auto", "Manual" };
            ViewBag.Count = windowsServers.Count();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_WindowsServersPartial", windowsServers);
            }

            return View("WindowsServersDiscovered", windowsServers);
        }

        public ActionResult WindowsServersUndiscovered(string hostname, string description, string managedBy, string maintenanceWindow, string patchMethod)
        {
            List<Computer> windowsServers = Computer.GetComputerList("ServersUndiscovered");

            if (!String.IsNullOrWhiteSpace(hostname)) { windowsServers = windowsServers.Where(x => x.Hostname.ToLower().Contains(hostname.ToLower())).ToList(); }
            if (!String.IsNullOrWhiteSpace(description)) { windowsServers = windowsServers.Where(x => !String.IsNullOrWhiteSpace(x.Description) && x.Description.ToLower().Contains(description.ToLower())).ToList(); }
            if (!String.IsNullOrWhiteSpace(managedBy)) { windowsServers = windowsServers.Where(x => !String.IsNullOrWhiteSpace(x.ManagedBy) && x.ManagedBy.ToLower().Contains(managedBy.ToLower())).ToList(); }
            if (!String.IsNullOrWhiteSpace(maintenanceWindow)) { windowsServers = windowsServers.Where(x => x.MaintenanceWindow.ToLower().Contains(maintenanceWindow.ToLower())).ToList(); }

            switch (patchMethod)
            {
                case "Auto":
                    windowsServers = windowsServers.Where(x => x.AutoPatch == true).ToList();
                    break;

                case "Manual":
                    windowsServers = windowsServers.Where(x => x.AutoPatch == false).ToList();
                    break;
            }

            ViewBag.ManagedBy = windowsServers.Where(x => !String.IsNullOrWhiteSpace(x.ManagedBy)).OrderBy(x => x.ManagedBy).Select(x => x.ManagedBy).Distinct();
            ViewBag.MaintenanceWindows = windowsServers.OrderBy(x => x.MaintenanceWindow).Select(x => x.MaintenanceWindow).Distinct();
            ViewBag.PatchMethod = new List<string> { "Auto", "Manual" };
            ViewBag.Count = windowsServers.Count();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_WindowsServersPartial", windowsServers);
            }

            return View("WindowsServersUndiscovered", windowsServers);
        }

        public ActionResult SystemsBI()
        {
            return View();
        }

        public ActionResult AutoPatchLogs()
        {
            ViewBag.Actions = Computer.GetAutoPatchLogs("Actions");
            ViewBag.Alerts = Computer.GetAutoPatchLogs("Alerts");

            return View();
        }

        [Authorize(Roles = "AM.SNAP.Admins")]
        public ActionResult ChangeLogs(string changeLogEvent)
        {
            ViewBag.Events = db.ChangeLogs.Select(x => x.Event).Distinct();

            List<ChangeLog> changeLogs = db.ChangeLogs.OrderByDescending(x => x.Timestamp).ToList();

            if (!String.IsNullOrWhiteSpace(changeLogEvent))
            {
                changeLogs = db.ChangeLogs.Where(x => x.Event == changeLogEvent).OrderByDescending(x => x.Timestamp).ToList();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ChangeLogsPartial", changeLogs.Take(20));
            }

            return View(changeLogs.Take(20));
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