using System;
using System.Configuration;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace SNAP.Models.Helpers
{
    public class PowerShell
    {
        internal static dynamic RunScript(string script, object inputObject = null)
        {
            System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create();

            Command command = new Command(String.Format(@"{0}\{1}.ps1", ConfigurationManager.AppSettings["SNAP_SCRIPTS_DIRECTORY"], script));

            command.Parameters.Add("SNAP_OBJECT", inputObject);
            command.Parameters.Add("SNAP_URI", ConfigurationManager.AppSettings["SNAP_URI"]);

            ps.Commands.AddCommand(command);

            dynamic scriptResult = null;

            foreach (PSObject result in ps.Invoke())
            {
                scriptResult = result.Members["ScriptResult"].Value;
            }

            return scriptResult;
        }

        internal static string GetHTMLString(string controller, string view, object model)
        {
            try
            {
                return RunScript("Get-HTMLString", new { Controller = controller, View = view, Model = model });
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}