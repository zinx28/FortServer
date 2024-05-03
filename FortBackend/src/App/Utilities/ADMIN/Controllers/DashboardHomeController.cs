using FortLibrary.Encoders;
using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Utilities.ADMIN.Controllers
{
    public class DashboardHomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            Console.WriteLine("T");
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {
                AdminData adminData = AdminServer.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                if(adminData != null)
                {
                    Console.WriteLine("Valid User!");
                    ViewData["Username"] = adminData.AdminUserName;
                    return View("~/src/App/Utilities/ADMIN/PAGES/Dashboard/Home.cshtml");
                }
            }

            return Redirect("/");
        }

        //[HttpPost]
        //public IActionResult Index(string email, string password)
        //{

        //}
    }
}
