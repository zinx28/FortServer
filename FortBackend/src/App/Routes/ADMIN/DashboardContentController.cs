using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.Saved;
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
    public class TempDataModel
    {
        public string data { get; set; } = string.Empty;
    }

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
                    if (adminData.bIsSetup)
                    {
                        return Redirect("/admin/setup");
                    }

                    //Console.WriteLine("Valid User!");
                    ViewData["Username"] = adminData.AdminUserName;
                    return View("~/src/App/Utilities/ADMIN/PAGES/Dashboard/Content.cshtml");
                }
            }

            return Redirect("/admin/login");
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
                                NewsManager.ContentConfig = JsonConvert.DeserializeObject<ContentConfig>(dataValue);
                                NewsManager.Update();
                               
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
