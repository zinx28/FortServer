using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Fortnite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpCompress.Common;
using System;
using System.Text.Json;

namespace FortBackend.src.App.Routes.ADMIN
{

    [Route("/admin/new/dashboard/content")]
    public class DashboardNewContentController : Controller
    {
        [HttpPost("data/{contentName}/{contentId}")]
        public async Task<IActionResult> DashboardContentID(string contentName, string contentId)
        {
            try
            {
                object? result = contentName.ToLower() switch
                {
                    "news" when contentId == "1" => NewsManager.ContentConfig.battleroyalenews,
                    "news" when contentId == "2" => NewsManager.ContentConfig.emergencynotice,
                    "news" when contentId == "3" => NewsManager.ContentConfig.loginmessage,
                    "news" when contentId == "4" => NewsManager.ContentConfig.playlistinformation,
                    "server" when contentId == "1" => new
                    {
                        ForcedSeason = Saved.DeserializeGameConfig.ForceSeason,
                        SeasonForced = Saved.DeserializeGameConfig.Season
                    },
                    _ => null
                };

                if (result != null)
                    return Json(result);

                return NotFound(new { message = "Invalid contentName or contentId provided." });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "DashboardContentID");
            }

            return Json(new
            {
                message = "Couldn't find content",
                error = false,
            });
        }


        [HttpPost("update")]
        public IActionResult UpdateTempDataV2([FromBody] JsonElement tempData)
        {
            try
            {
                var authToken = Request.Headers["Authorization"].ToString();

                if(authToken != null)
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        if(tempData.TryGetProperty("IniChanges", out JsonElement IniChangesData))
                        {
                            string dataValue = IniChangesData.ToString();
                            if (!string.IsNullOrEmpty(dataValue))
                            {
                                Console.WriteLine(dataValue);

                                SavingCloudStorage DeserializeSavingCloudStorage = JsonConvert.DeserializeObject<SavingCloudStorage>(dataValue);

                                if(DeserializeSavingCloudStorage != null)
                                {
                                    Console.WriteLine(DeserializeSavingCloudStorage.Title);

                                    if (System.IO.File.Exists(PathConstants.CloudStorage.IniConfig))
                                    {
                                        var ReadFile = System.IO.File.ReadAllText(PathConstants.CloudStorage.IniConfig);
                                        if (ReadFile != null)
                                        {
                                            IniConfig DeserializeConfig = JsonConvert.DeserializeObject<IniConfig>(ReadFile);
                                            if( DeserializeConfig != null)
                                            {
                                                foreach (IniConfigFiles item in DeserializeConfig.FileData)
                                                {
                                                    if(item.Name == DeserializeSavingCloudStorage.Title)
                                                    {
                                                        Console.WriteLine("Valid File Name");

                                                        foreach(IniConfigData item2 in item.Data)
                                                        {
                                                            if(item2.Title == DeserializeSavingCloudStorage.Body.Name)
                                                            {
                                                                Console.WriteLine("Valid File Name 2");
                                                                item2.Data = DeserializeSavingCloudStorage.Body.CachedData;

                                                            }
                                                        }

                                                        item.UploadedTime = DateTime.Now;
                                                    }
                                                }


                                                System.IO.File.WriteAllText(PathConstants.CloudStorage.IniConfig, JsonConvert.SerializeObject(DeserializeConfig, Formatting.Indented));
                                                IniManager.IniConfigData = DeserializeConfig;
                                            }
                                          
                                           // IniManager.IniConfigData = JsonConvert.DeserializeObject<IniConfig>(filePath)!;
                                        }
                                    }
                                }
                                //SavingCloudStorage
                                //  Console.WriteLine(dataValue.Title);
                            }

                        }

                        if (tempData.TryGetProperty("BackendConfig", out JsonElement BackenddataElement))
                        {
                            string dataValue = BackenddataElement.ToString();
                            //Console.WriteLine(dataValue);
                            if (!string.IsNullOrEmpty(dataValue))
                            {
                                if (System.IO.File.Exists(PathConstants.CachedPaths.FortGame))
                                {

                                    var FortDataConfig = JsonConvert.DeserializeObject<FortGameConfig>(dataValue);
                                    if(FortDataConfig != null)
                                    {
                                        var ReadFile = System.IO.File.ReadAllText(PathConstants.CachedPaths.FortGame);
                                        if(ReadFile != null)
                                        {
                                            FortGameConfig DeserializeConfig = JsonConvert.DeserializeObject<FortGameConfig>(ReadFile);

                                            if (DeserializeConfig != null)
                                            {
                                                Saved.DeserializeGameConfig.Season = FortDataConfig.Season;
                                                Saved.DeserializeGameConfig.ForceSeason = FortDataConfig.ForceSeason;

                                                DeserializeConfig.Season = FortDataConfig.Season;
                                                DeserializeConfig.ForceSeason = FortDataConfig.ForceSeason;


                                                System.IO.File.WriteAllText(PathConstants.CachedPaths.FortGame, JsonConvert.SerializeObject(DeserializeConfig, Formatting.Indented));
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (tempData.TryGetProperty("data", out JsonElement dataElement))
                        {
                            string dataValue = dataElement.ToString();
                            //Console.WriteLine(dataValue);
                            if (!string.IsNullOrEmpty(dataValue))
                            {
                                if (dataElement.ValueKind == JsonValueKind.Array && dataElement.GetArrayLength() == 0)
                                {
                                    Console.WriteLine("Array Empty!");
                                }
                                else
                                {
                                    Console.WriteLine(dataValue);
                                    NewsManager.ContentConfig = JsonConvert.DeserializeObject<ContentConfig>(dataValue);
                                    NewsManager.Update();
                                }
                             
                               
                            }
                        }

                        return Json(true);
                    }
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
