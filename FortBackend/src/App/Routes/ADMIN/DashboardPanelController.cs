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
using static FortBackend.src.App.Routes.ADMIN.NewFolder.ContentUpdate;
using static FortBackend.src.App.Utilities.MongoDB.Helpers.GrabAdminData;

namespace FortBackend.src.App.Routes.ADMIN
{
    public class UserEditClassRQ
    {
        public int selectedRole { get; set; } = -1;
        public string accountId { get; set; } = string.Empty;
    }
    
    public class GrantDataRQ
    {
        public string DiscordID { get; set; } = string.Empty;
    }
    [Route("/admin/new/dashboard/panel")]
    public class DashboardPanelController : Controller
    {
        [HttpPost("user/edit")]
        public async Task<IActionResult> UserEdit()
        {
            
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {

                using (var reader = new StreamReader(HttpContext.Request.Body))
                {
                    var requestBody = await reader.ReadToEndAsync();

                    UserEditClassRQ contentRequest = JsonConvert.DeserializeObject<UserEditClassRQ>(requestBody)!;

                    

                    if (string.IsNullOrEmpty(contentRequest.accountId))
                    {
                        return Json(new
                        {
                            message = "Account Id is null",
                            error = true,
                        });
                    }


                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken)!;
                    if (adminData != null)
                    {
                        // moderators cannot edit / add users
                        if (adminData.RoleId > AdminDashboardRoles.Moderator)
                        {
                            if (contentRequest.selectedRole == -1)
                            {
                                return Json(new
                                {
                                    message = "BAD FORM!! :3",
                                    error = true,
                                });
                            }

                            // ggs vaild ids only!!
                            if (contentRequest.selectedRole == AdminDashboardRoles.Moderator ||
                                contentRequest.selectedRole == AdminDashboardRoles.Admin)
                            {


                                AdminProfileCacheEntry adminProfileCacheEntry = GrabAdminData.AdminUsers?.FirstOrDefault(e => e.profileCacheEntry.AccountId == contentRequest.accountId)!;

                                if (adminProfileCacheEntry != null)
                                {
                                    Logger.Log("WANTING TO UPDATE USERS DATA");
                                    await GrabAdminData.EditAdminV2(adminProfileCacheEntry.profileCacheEntry.UserData.Username,
                                        new AdminDataInfo
                                        {
                                            AccountId = contentRequest.accountId,
                                            DiscordId = adminProfileCacheEntry.adminInfo.DiscordId,
                                            Role = contentRequest.selectedRole
                                        }, adminData);

                                    return Json(new
                                    {
                                        message = "Updated User!",
                                        error = false
                                    });
                                }
                            }
                        }
                    }

                }
            }

            return Json(new
            {
                message = "Failed To Edit User",
                error = true
            });
        }

        // this gets called on every user
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
                using (var reader = new StreamReader(HttpContext.Request.Body))
                {
                    var requestBody = await reader.ReadToEndAsync();

                    GrantDataRQ grantDataRQ = JsonConvert.DeserializeObject<GrantDataRQ>(requestBody)!;

                    if(grantDataRQ != null)
                    {
                        if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                        {
                            AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken)!;
                            if (adminData != null)
                            {
                                // moderators cannot edit / add users
                                if (adminData.RoleId > AdminDashboardRoles.Moderator)
                                {
                                    if (!string.IsNullOrEmpty(grantDataRQ.DiscordID))
                                    {
                                        NewAdminClass AddAdmin = await GrabAdminData.AddAdmin(grantDataRQ.DiscordID, adminData);
                                        if (!string.IsNullOrEmpty(AddAdmin.UserName))
                                        {
                                            return Json(new
                                            {
                                                message = "Added Account!",
                                                data = new
                                                {
                                                    DisplayName = AddAdmin.UserName,
                                                    AccountId = AddAdmin.AccountID
                                                },
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
