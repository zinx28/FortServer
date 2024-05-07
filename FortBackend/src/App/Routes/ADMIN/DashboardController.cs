using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.Encoders;
using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.ADMIN
{
    [Route("/admin/dashboard")]
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {
                AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                if (adminData != null)
                {
                    if (adminData.bIsSetup)
                    {
                        return Redirect("/admin/setup");
                    }
                    //Console.WriteLine("Valid User!");
                    ViewData["Username"] = adminData.AdminUserName;
                    //return View("~/src/App/Utilities/ADMIN/Dashboard/Home.cshtml");
                    return Redirect("/admin/dashboard/home");
                }
            }

            return Redirect("/admin/login");
        }


        //[HttpPost]
        //public IActionResult Index(string email, string password)
        //{

        //}
    }
}
