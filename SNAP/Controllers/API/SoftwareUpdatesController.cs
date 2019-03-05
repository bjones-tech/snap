using SNAP.Models.Helpers;
using System.Web.Http;

namespace SNAP.Controllers.API
{
    public class SoftwareUpdatesController : ApiController
    {
        [HttpPost]
        [Route("api/SoftwareUpdates/GetStatus")]
        public Computer GetStatus(Computer computer)
        {
            return computer.GetSoftwareUpdateStatus();
        }

        [HttpPost]
        [Route("api/SoftwareUpdates/UpdateAutoPatchGroup")]
        public void SetAutoPatch(Computer computer)
        {
            computer.UpdateAutoPatchGroup();
        }
    }
}