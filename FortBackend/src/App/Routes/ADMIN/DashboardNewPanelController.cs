using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics.Dashboard;
using FortLibrary.MongoDB.Module;
using FortLibrary.MongoDB.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;

namespace FortBackend.src.App.Routes.ADMIN
{

    [Route("/admin/new/dashboard/panel")]
    public class DashboardNewPanelController : Controller
    {
        [HttpPost("user/edit")]
        public async Task<IActionResult> UserEdit()
        {
            
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {
                int RoleID = -1;
                string AccountId = "";

                var FormRequest = HttpContext.Request.Form;

                if (FormRequest.TryGetValue("RoleIdRadios", out var roleIdL))
                {
                    if(roleIdL == "adminoption")
                    {
                        RoleID = AdminDashboardRoles.Admin;
                    }
                    else if(roleIdL == "moderatoroption")
                    {
                        RoleID = AdminDashboardRoles.Moderator;
                    }
                }

                if (FormRequest.TryGetValue("accountId", out var accountIdL))
                {
                    if (!string.IsNullOrEmpty(accountIdL))
                    {
                        AccountId = accountIdL;
                    }
                }

                if (string.IsNullOrEmpty(AccountId))
                {
                    return Json(new
                    {
                        message = "Weird!!!!!!",
                        error = true,
                    });
                }


                AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                if (adminData != null)
                {
                    // moderators cannot edit / add users
                    if (adminData.RoleId > AdminDashboardRoles.Moderator)
                    {
                        if(RoleID == -1)
                        {
                            return Json(new
                            {
                                message = "BAD FORM!! :3",
                                error = true,
                            });
                        }

                        if (!string.IsNullOrEmpty(AccountId))
                        {
                            AdminProfileCacheEntry adminProfileCacheEntry = GrabAdminData.AdminUsers?.FirstOrDefault(e => e.profileCacheEntry.AccountId == AccountId)!;

                            if (adminProfileCacheEntry != null)
                            {
                                Console.WriteLine("WANTING TO UPDATE USERS DATA");
                                await GrabAdminData.EditAdminV2(adminProfileCacheEntry.profileCacheEntry.UserData.Username, 
                                    new AdminDataInfo
                                    {
                                        AccountId = AccountId,
                                        DiscordId = adminProfileCacheEntry.adminInfo.DiscordId,
                                        Role = RoleID
                                    }, adminData);

                                return Json(true);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("FAILED");
            return Redirect("/admin/dashboard/panel");
        }

        // this gets called on every user (people no longer can edit the json to change other data!)
        [HttpPost("user/edit/{accountId}")]
        public async Task<IActionResult> UserEditAcc(string accountId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId))
                {
                    return Json(new
                    {
                        message = "Weird!!!!!!",
                        error = true,
                    });
                }

                if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                   
                    if (adminData != null)
                    {
                        // moderators cannot edit / add users
                        if (adminData.RoleId > AdminDashboardRoles.Moderator)
                        {
                            
                            AdminProfileCacheEntry adminProfileCacheEntry = GrabAdminData.AdminUsers?.FirstOrDefault(e => e.profileCacheEntry.AccountId == accountId)!;
              
                            //Console.WriteLine(dataValue);
                            if (adminProfileCacheEntry != null)
                            {
                               
                                return Json(new
                                {
                                    UserData = adminProfileCacheEntry.profileCacheEntry.UserData,
                                    AccountId = adminProfileCacheEntry.profileCacheEntry.AccountId,
                                    adminInfo = adminProfileCacheEntry.adminInfo
                                }) ;
                                
                            }

                        }
                    }
                }
            }catch (Exception ex)
            {
                Logger.Error(ex.Message, "UserEditAcc");
            }

            return Json(new
            {
                message = "Couldn't find account",
                error = true,
            });
        }

        [HttpPost("grant")]
        public async Task<IActionResult> GrantPost()
        {
            Response.ContentType = "application/json";
            try
            {
                var DiscordId = "";
                var FormRequest = HttpContext.Request.Form;

                if (FormRequest.TryGetValue("discordId", out var discordIdL))
                {
                    Console.WriteLine(discordIdL);
                    DiscordId = discordIdL;
                }

                if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        // moderators cannot edit / add users
                        if (adminData.RoleId > AdminDashboardRoles.Moderator)
                        {
                            if (!string.IsNullOrEmpty(DiscordId))
                            {
                                bool AddAdmin = await GrabAdminData.AddAdmin(DiscordId, adminData);
                                if (AddAdmin)
                                {
                                    return Json(new
                                    {
                                        message = "Added Account!",
                                        error = false,
                                    });
                                }
                                return Json(new
                                {
                                    message = "Failed To Account!",
                                    error = true,
                                });
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                message = "You're account doesnt reach the requirements",
                                error = true,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "GrantPost");
            }

            return Json(new
            {
                message = "Unknown",
                error = true,
            });
        }
    }
}
