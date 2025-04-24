using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.Dynamics.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FortBackend.src.App.Routes.ADMIN.NewFolder
{
    [Route("/dashboard/v2/content")]
    public class ContentController : Controller
    {
        /*
         * Returns LIST of items ~ doesn't give the data (isnt the best but i didnt want context/dropdown boxes)
         */
        [HttpPost("id/{contentName}/{contentId}")]
        public async Task<IActionResult> DashboardContentID(string contentName, string contentId)
        {
            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        // goofy ahh code if it works it works
                        var battleroyaleNews = NewsManager.ContentConfig.battleroyalenews;

                        List<string> MotdsNews = battleroyaleNews.motds
                            .Select((_, index) => $"Motds {index + 1}")
                            .ToList();

                        MotdsNews.AddRange(battleroyaleNews.messages
                            .Select((_, index) => $"News {index + 1}"));

                        object? result = contentName.ToLower() switch
                        {
                            "news" when contentId == "1" => MotdsNews,
                            "news" when contentId == "2" => NewsManager.ContentConfig.emergencynotice
                                .Select((_, index) => index == 0 ? "Emergency" : $"Emergency {index}")
                                .ToList(),
                            "news" when contentId == "3" => new List<string> { NewsManager.ContentConfig.loginmessage.title.en }, // who cares full data!
                            "news" when contentId == "4" => NewsManager.ContentConfig.playlistinformation.Select(e => e.playlist_name).ToList(),
                            "server" when contentId == "1" => new
                            {
                                // not implemetned to change yet
                                ForcedSeason = Saved.DeserializeGameConfig.ForceSeason,
                                SeasonForced = Saved.DeserializeGameConfig.Season
                            },

                            _ => null
                        };

                        if (result != null)
                            return Json(result);

                        return Json(new
                        {
                            message = "Invalid contentName or contentId provided.",
                            error = true,
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        message = "INVAILD LOGIN",
                        error = true,
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "DashboardContentID");
            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });
        }

        [HttpPost("data/{contentName}/{contentId}/{index}")]
        public async Task<IActionResult> DashboardContentID(string contentName, string contentId, string index)
        {
            try
            {
                int Index = int.TryParse(index, out int TempIndex) ? TempIndex : 0;

                if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        var battleroyaleNews = NewsManager.ContentConfig.battleroyalenews;



                        var NewCombineed = battleroyaleNews.motds
                            .Select(e => new { Type = "Motds", Data = e })
                            .Concat(battleroyaleNews.messages
                            .Select(e => new { Type = "News", Data = e }))
                            .ToList();

                        object? result = contentName.ToLower() switch
                        {
                            "news" when contentId == "1" => NewCombineed[Index],
                            "news" when contentId == "2" => new {
                                Type = "Emergency",
                                Data = NewsManager.ContentConfig.emergencynotice[Index]
                            },
                            "news" when contentId == "3" => new
                            {
                                Type = "LoginMessage",
                                Data = NewsManager.ContentConfig.loginmessage,
                            },
                            "news" when contentId == "4" => new
                            {
                                Type = "Playlist",
                                Data = NewsManager.ContentConfig.playlistinformation[Index],
                            },
                            "server" when contentId == "1" => new
                            {
                                // not implemetned to change yet
                                ForcedSeason = Saved.DeserializeGameConfig.ForceSeason,
                                Season = Saved.DeserializeGameConfig.Season,
                                WeeklyQuests = Saved.DeserializeGameConfig.WeeklyQuest,
                                ShopRotation = Saved.DeserializeGameConfig.ShopRotation,
                            },

                            _ => null
                        };

                        if (result != null)
                            return Json(result);

                        return Json(new
                        {
                            message = "Invalid contentName or contentId provided.",
                            error = true,
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        message = "INVAILD LOGIN",
                        error = true,
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "DashboardContentID");
            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });
        }

        [HttpPost("update/{contentName}/{contentId}/{index}")]
        public async Task<IActionResult> DashboardUpdateContentID(string contentName, string contentId, string index)
        {
            Response.ContentType = "application/json";
            try
            {
                int Index = int.TryParse(index, out int TempIndex) ? TempIndex : 0;

                if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        // prob add permissions differently + bot will use moderater from dashboard in the ftuure
                        if(contentName == "news" || contentName == "ini" || contentName == "server")
                        {
                            using (var reader = new StreamReader(HttpContext.Request.Body))
                            {
                                var requestBody = await reader.ReadToEndAsync();

                                return Json(ContentUpdate.Update(requestBody, contentName, contentId, Index));
                            }
                        }
                        else
                        {
                            // only admins can edit data so no point allowing!
                            if (adminData.RoleId > AdminDashboardRoles.Moderator)
                            {
                                using (var reader = new StreamReader(HttpContext.Request.Body))
                                {
                                    var requestBody = await reader.ReadToEndAsync();

                                    return Json(ContentUpdate.Update(requestBody, contentName, contentId, Index));
                                }
                            }
                            else
                            {
                                return Json(new
                                {
                                    message = "You don't have permission to edit this item",
                                    error = true,
                                });
                            }
                        }
                      
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "DashboardContentID");
            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });
        }
    }
}
