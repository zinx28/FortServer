using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.ConfigHelpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace FortBackend.src.App.Routes.ADMIN
{
    [Route("/admin/dashboard/content")]
    public class DashboardContentController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {
                AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                if (adminData != null)
                {
                    Console.WriteLine("Valid User!");
                    ViewData["Username"] = adminData.AdminUserName;
                    return View("~/src/App/Utilities/ADMIN/PAGES/Dashboard/Content.cshtml");
                }
            }

            return Redirect("/admin/login");
        }

        [HttpPost("update")]
        public ActionResult UpdateTempDataV2([FromBody] string tempData)
        {
            try
            {
                if (!string.IsNullOrEmpty(tempData))
                {
                    NewsManager.ContentConfig = JsonConvert.DeserializeObject<ContentConfig>(tempData);
                    return Json(true);
                }

                return Json(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating temp data: {ex.Message}");
                return Json(false);
            }
        }
    }
}
