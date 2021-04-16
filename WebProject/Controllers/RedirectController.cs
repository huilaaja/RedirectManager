using System.Web.Mvc;

namespace WebProject.Controllers
{
    [Authorize(Roles = "WebAdmins, Administrators")]
    public class RedirectController : Controller
    {
        [Route("Admin/RedirectManager")]
        public ActionResult Index()
        {
            return View("/Views/Admin/RedirectManager.cshtml");
        }
    }
}