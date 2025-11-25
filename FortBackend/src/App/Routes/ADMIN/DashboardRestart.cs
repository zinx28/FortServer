using Discord;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics.Dashboard;
using FortLibrary.Encoders;
using FortLibrary.MongoDB.Modules;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace FortBackend.src.App.Routes.ADMIN
{
    public class DashboardRestart : Controller
    {
        private readonly IHostApplicationLifetime _lifetime;

        public DashboardRestart(IHostApplicationLifetime lifetime)
        {
            _lifetime = lifetime;
        }
        [HttpPost("/admin/restart")] 
        public IActionResult RestartServer()
        {
            Response.ContentType = "application/json";

            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken)!;
                    if (adminData != null)
                    {
                        if (adminData.RoleId > AdminDashboardRoles.Moderator)
                        {
                            if (adminData.bIsSetup)
                            {
                                return Json(new
                                {
                                    message = "This account is currently locked :D, please finish setup",
                                    error = true
                                });
                            }

                            ProcessExitHandler.RestartServer(_lifetime);

                            return Json(new { 
                                message = "Restarting servers, dashboard will become unresponsive!",
                                error = false
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                message = "You do not have permission to use this!", // cute
                                error = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "LoginCheck");
            }

            return Json(new { message = "Not Logged In", error = true });
        }
    }
}