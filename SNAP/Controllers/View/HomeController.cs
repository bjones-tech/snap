using System.Web.Mvc;

namespace SNAP.Controllers.View
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}