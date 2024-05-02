using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Utilities.ADMIN.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("~/src/App/Utilities/ADMIN/Pages/Index.cshtml");
        }
    }
}
