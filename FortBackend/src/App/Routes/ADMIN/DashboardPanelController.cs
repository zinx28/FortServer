using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
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
                    if (adminData.bIsSetup)
                    {
                        return Redirect("/admin/setup");
                    }

                    // await GrabAdminData.GrabAllAdmin();
                    ViewData["roleId"] = adminData.RoleId;

                    //Console.WriteLine((int)ViewData["roleId"]);


                    //Console.WriteLine("Valid User!");
                    ViewData["Username"] = adminData.AdminUserName;
                    return View("~/src/App/Utilities/ADMIN/Dashboard/AdminPanel.cshtml");
                }
            }

            return Redirect("/admin/login");
        }

        [HttpPost("user/edit")]
        public async Task<IActionResult> UserEdit([FromBody] JsonElement tempData)
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
                            //Console.WriteLine(dataValue);
                            if (!string.IsNullOrEmpty(dataValue))
                            {
                                AdminProfileCacheEntry Updateeditthingy = JsonConvert.DeserializeObject<AdminProfileCacheEntry>(dataValue);
                                if(Updateeditthingy != null)
                                {
                                    
                                    Console.WriteLine("WANTING TO UPDATE USERS DATA");
                                    await GrabAdminData.EditAdmin(Updateeditthingy.adminInfo);

                                    return Json(true);
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("FAILED");
            return Redirect("/admin/dashboard/panel");
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
