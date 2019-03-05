using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SNAP.Models.Helpers
{
    public class Computer
    {
        [Display(Name = "SID")]
        public string SID { get; set; }

        [Display(Name = "Hostname")]
        public string Hostname { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Managed By")]
        public string ManagedBy { get; set; }

        [Display(Name = "Operating System")]
        public string OperatingSystem { get; set; }

        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [Display(Name = "Maintenance Window")]
        public string MaintenanceWindow { get; set; }

        [Display(Name = "Auto Patch")]
        public bool AutoPatch { get; set; }

        [Display(Name = "Update Status")]
        public string UpdateStatus { get; set; }

        [Display(Name = "Update Status Style")]
        public string UpdateStatusStyle { get; set; }

        [Display(Name = "Last Boot Up Time")]
        public string LastBootUpTime { get; set; }

        private static Computer CreateFromSCCMComputer(dynamic psObject)
        {
            try
            {
                Computer computer = new Computer();

                computer.SID = psObject.SID;
                computer.Hostname = psObject.Hostname;
                computer.OperatingSystem = psObject.OperatingSystem;

                computer.Username = null;
                computer.Description = null;
                computer.ManagedBy = null;
                computer.Manufacturer = null;
                computer.Model = null;
                computer.SerialNumber = null;
                computer.MaintenanceWindow = null;
                computer.AutoPatch = false;

                if (psObject.Username is System.DBNull == false)
                {
                    computer.Username = psObject.Username;
                }

                if (psObject.Description is System.DBNull == false)
                {
                    computer.Description = psObject.Description;
                }

                if (psObject.ManagedBy is System.DBNull == false)
                {
                    computer.ManagedBy = psObject.ManagedBy;
                }

                if (psObject.Manufacturer is System.DBNull == false)
                {
                    computer.Manufacturer = psObject.Manufacturer;
                }

                if (psObject.Model is System.DBNull == false)
                {
                    computer.Model = psObject.Model;
                }

                if (psObject.SerialNumber is System.DBNull == false)
                {
                    computer.SerialNumber = psObject.SerialNumber;
                }

                if (psObject.MaintenanceWindow is System.DBNull == false)
                {
                    computer.MaintenanceWindow = psObject.MaintenanceWindow;
                }

                if (psObject.AutoPatch is System.DBNull == false)
                {
                    computer.AutoPatch = psObject.AutoPatch;
                }

                return computer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Computer CreateFromADComputer(dynamic psObject)
        {
            try
            {
                Computer computer = new Computer();

                computer.SID = psObject.SID.ToString();
                computer.Hostname = psObject.Name;
                computer.Description = psObject.Description;
                computer.ManagedBy = psObject.ManagedBy;
                computer.OperatingSystem = psObject.OperatingSystem;
                computer.MaintenanceWindow = psObject.Department;
                computer.AutoPatch = psObject.Title == "True";

                return computer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static List<Computer> GetComputerList(string query)
        {
            try
            {
                List<Computer> computers = new List<Computer>();

                dynamic psObjectArray = PowerShell.RunScript("Get-ComputerList", new { Query = query });

                foreach (dynamic psObject in psObjectArray)
                {
                    Computer computer = null;

                    switch (query)
                    {
                        case "ServersUndiscovered":
                            computer = CreateFromADComputer(psObject);
                            break;

                        default:
                            computer = CreateFromSCCMComputer(psObject);
                            break;
                    }

                    if (computer != null)
                    {
                        computers.Add(computer);
                    }
                }

                return computers;
            }
            catch (Exception)
            {
                return new List<Computer>();
            }
        }

        internal Computer GetSoftwareUpdateStatus()
        {
            try
            {
                dynamic psObject = PowerShell.RunScript("Get-SoftwareUpdateStatus", new { Hostname = this.Hostname });

                UpdateStatus = psObject.Status;
                UpdateStatusStyle = psObject.StatusStyle;
                LastBootUpTime = psObject.LastBootUpTime;

                return this;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static string GetAutoPatchLogs(string type)
        {
            try
            {
                dynamic autoPatchLogs = PowerShell.RunScript("Get-AutoPatchLogs");

                switch (type)
                {
                    case "Actions":
                        return autoPatchLogs.Actions.ToString();

                    case "Alerts":
                        return autoPatchLogs.Alerts.ToString();

                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal void UpdateAutoPatchGroup()
        {
            try
            {
                dynamic psObject = PowerShell.RunScript("Update-AutoPatchGroup", new { Hostname = this.Hostname, AutoPatch = this.AutoPatch });
            }
            catch (Exception)
            {
                // REVISIT
            }
        }
    }
}