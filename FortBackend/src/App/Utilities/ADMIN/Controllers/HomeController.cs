using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Utilities.ADMIN.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/src/App/Utilities/ADMIN/Pages/Index.cshtml");
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            Console.WriteLine(email);
            Console.WriteLine($"Password {password}");
            if (email == Saved.Saved.DeserializeConfig.AdminEmail)
            {
                if(password == Saved.Saved.DeserializeConfig.AdminPassword)
                {
                    Console.WriteLine($"w");
                    ViewBag.ErrorMessage = "Correct!";
                    return View("~/src/App/Utilities/ADMIN/Pages/Index.cshtml");
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password.";

            return View("~/src/App/Utilities/ADMIN/Pages/Index.cshtml");
        }
    }
}
