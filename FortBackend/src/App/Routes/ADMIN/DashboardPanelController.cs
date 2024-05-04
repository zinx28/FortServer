using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;

namespace FortBackend.src.App.Routes.ADMIN
{

    [Route("/admin/dashboard/panel")]
    public class DashboardPanelController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {
                //GrabAllAdmin
                AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                if (adminData != null)
                {
                   // await GrabAdminData.GrabAllAdmin();
                    
                    Console.WriteLine("Valid User!");
                    ViewData["Username"] = adminData.AdminUserName;
                    return View("~/src/App/Utilities/ADMIN/PAGES/Dashboard/AdminPanel.cshtml");
                }
            }

            return Redirect("/admin/login");
        }

        [HttpPost("grant")]
        public async Task<IActionResult> GrantPost([FromBody] JsonElement tempData)
        {
            var authToken = Request.Headers["Authorization"].ToString();

            if (authToken != null)
            {
                AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                if (adminData != null)
                {
                    // moderators cannot edit / add users
                    if (adminData.RoleId > AdminDashboardRoles.Moderator)
                    {
                        if (tempData.TryGetProperty("data", out JsonElement dataElement))
                        {
                            string dataValue = dataElement.ToString();
                            Console.WriteLine(dataValue);
                            if (!string.IsNullOrEmpty(dataValue))
                            {
                                bool AddAdmin = await GrabAdminData.AddAdmin(dataValue);
                                if (AddAdmin)
                                {
                                    return Redirect("/admin/dashboard/panel");
                                }
                                return Json(AddAdmin);
                            }
                        }
                    }
                }
            }

            return Redirect("/admin/dashboard/panel");
        }
    }
}
