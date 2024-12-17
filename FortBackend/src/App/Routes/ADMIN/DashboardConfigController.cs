using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics;
using FortLibrary.Dynamics.Dashboard;
using FortLibrary.Encoders;
using FortLibrary.EpicResponses.Fortnite;
using FortLibrary.Shop;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpCompress.Common;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace FortBackend.src.App.Routes.ADMIN
{

    [Route("/admin/new/dashboard/content")]
    public class DashboardConfigController : Controller
    {
        [HttpPost("ConfigData")]
        public async Task<IActionResult> ConfigData()
        {
            try
            {
                var authToken = Request.Headers["Authorization"].ToString().ToLower().Split("bearer ")[1]; ;

                if (authToken != null)
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken.ToLower() == authToken);
                    if (adminData != null)
                    {
                        if (adminData.RoleId > AdminDashboardRoles.Moderator)
                        {
                            return Ok(DashboardConfigData.GetDashboardConfigData());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "ConfigData");
            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });
        }

        [HttpPost("ConfigData/{index}")]
        public async Task<IActionResult> ConfigDataViewData(string index)
        {
            try
            {
                int Index = int.TryParse(index, out int TempIndex) ? TempIndex : 0;
                if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken.ToLower() == authToken.ToLower()!)!;
                    if (adminData != null)
                    {
                        if (adminData.RoleId > AdminDashboardRoles.Moderator)
                        {
                            var ConfigDataView = DashboardConfigData.GetDashboardConfigData()[Index];
                            if (ConfigDataView != null)
                                return Ok(ConfigDataView);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "ConfigData");
            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });

        }
    }
}