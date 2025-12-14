using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.Utilities.Shop;
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
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using static FortBackend.src.App.Utilities.ADMIN.DashboardConfigData;
using static MongoDB.Driver.WriteConcern;

namespace FortBackend.src.App.Routes.ADMIN
{

    [Route("/admin/new/dashboard/content")]
    public class DashboardContentController : Controller
    {

        [HttpPost("dataV2/{contentName}/{contentId}")]
        public async Task<IActionResult> DashboardContentIDIni(string contentName, string contentId)
        {
            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        object? result = contentName.ToLower() switch
                        {
                            "ini" when contentId == "1" => IniManager.IniConfigData.FileData.Select(section => new
                            {
                                FileName = section.Name,
                                Data = section.Data.Select(e => new
                                {
                                    e.Title,
                                })
                            }),
                            "ini" when contentId == "2" => IniManager.IniConfigData.FileData.Select(section => new
                            {
                                FileName = section.Name,
                                IniValue = IniManager.GrabRawIniFile(section.Name)
                            }),
                            "cup" when contentId == "tournaments" => CupCache.cacheCupsDatas,


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
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "DashboardContentIDIni");
            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });
        }

        [HttpPost("data/{contentName}/{contentId}")]
        public async Task<IActionResult> DashboardContentID(string contentName, string contentId)
        {
            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
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
      
        // less support so just ini atm
        [HttpPost("data/{contentName}/{contentId}/{index}")]
        public async Task<IActionResult> DashboardContentIdIni(string contentName, string contentId, string index)
        {
            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        int ContentId = int.TryParse(contentId, out int TempcontentId) ? TempcontentId : 0;
                        int Index = int.TryParse(index, out int TempIndex) ? TempIndex : 0;

                        object? result = contentName.ToLower() switch
                        {
                            "ini" => IniManager.IniConfigData.FileData[ContentId].Data[Index],
                            //"config" when contentId == "1" => new
                            //{
                            //    ProjectName = Saved.DeserializeConfig.ProjectName,
                            //    JWT_KEY = Saved.DeserializeConfig.JWTKEY,
                            //    BackendPort = Saved.DeserializeConfig.BackendPort,
                            //    MatchmakerPort = Saved.DeserializeConfig.MatchmakerPort,
                            //    XmppPort = Saved.DeserializeConfig.XmppPort,
                            //    CustomMatchmaker = Saved.DeserializeConfig.CustomMatchmaker, // GSIP GSPORT needs this on
                            //    GameServerIP = Saved.DeserializeConfig.GameServerIP,
                            //    GameServerPort = Saved.DeserializeConfig.GameServerPort,
                            //    EnableLogs = Saved.DeserializeConfig.EnableLogs,
                            //    Cloudflare = Saved.DeserializeConfig.Cloudflare, // i might make edit take you to a different page

                            //},
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
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "DashboardContentID_INI");
            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });
        }

        [HttpPost("refresh-shop")]
        public async Task<IActionResult> RefreshShop()
        {
            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        await GenerateShop.Init();
                        return Json(new
                        {
                            message = "Refreshed Shop",
                            error = false,
                        });
                    }   
                }

            }
            catch (Exception ex)
            {

            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });
        }

        // REMOVING IN THE FUTURE
        [HttpPost("update")]
        public IActionResult UpdateTempDataV2()
        {
            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        var FormRequest = HttpContext.Request.Form;

                        if (FormRequest.TryGetValue("context", out var Context))
                        {
                            if (!string.IsNullOrEmpty(Context))
                            {
                                //var Context = contextL;
                                string? Title = FormRequest["title"];
                                string? Body = FormRequest["body"];
                                float numberBox = float.TryParse(FormRequest["NumberBox"], out float tempNumber) ? tempNumber : 0;
                                string ? SectionPart = FormRequest["newsId"];
                                int SectionId = int.TryParse(FormRequest["sectionId"], out int TempcontentId) ? TempcontentId : 0;
                                string? RadioBox = FormRequest["RadioBox"];
                                bool RadioValue = true;
                                if (RadioBox == null) {
                                     RadioValue = false;
                                }

                                int ArrayIndex = int.TryParse(FormRequest["arrayIndex"], out int tempIndex) ? tempIndex : 0;

                                if(Context == "news")
                                {
                                    if(SectionId == 1)
                                    {
                                        if(SectionPart == "Messages")
                                        {
                                            var test = NewsManager.ContentConfig.battleroyalenews.messages[ArrayIndex];
                                            
                                            test.title.en = Title;
                                            test.body.en = Body;

                                            NewsManager.Update();

                                            return Json(new
                                            {
                                                message = "Updated Content",
                                                error = false,
                                            });
                                        }
                                        else if(SectionPart == "Motds")
                                        {
                                            var test = NewsManager.ContentConfig.battleroyalenews.motds[ArrayIndex];

                                            test.title.en = Title;
                                            test.body.en = Body;

                                            NewsManager.Update();

                                            return Json(new
                                            {
                                                message = "Updated Content",
                                                error = false,
                                            });
                                        }
                                    } 
                                    else if (SectionId == 2)
                                    {
                                        if (SectionPart == "Emergency")
                                        {
                                            var test = NewsManager.ContentConfig.emergencynotice[ArrayIndex];

                                            test.title.en = Title;
                                            test.body.en = Body;

                                            NewsManager.Update();

                                            return Json(new
                                            {
                                                message = "Updated Content",
                                                error = false,
                                            });
                                        }
                                    }
                                    else if(SectionId == 3)
                                    {
                                        var test = NewsManager.ContentConfig.loginmessage;

                                        test.title.en = Title;
                                        test.body.en = Body;

                                        NewsManager.Update();

                                        return Json(new
                                        {
                                            message = "Updated Content",
                                            error = false,
                                        });
                                    }
                                    else if(SectionId == 4)
                                    {
                                        var test = NewsManager.ContentConfig.playlistinformation[ArrayIndex];

                                        test.display_name.en = Title;
                                        test.description.en = Body;

                                        NewsManager.Update();

                                        return Json(new
                                        {
                                            message = "Updated Content",
                                            error = false,
                                        });
                                    }
                                }
                                else if(Context == "server") 
                                {
                                    if (SectionId == 1)
                                    {
                                        Saved.DeserializeGameConfig.Season = numberBox;
                                        Saved.DeserializeGameConfig.ForceSeason = RadioValue;

                                        System.IO.File.WriteAllText(PathConstants.CachedPaths.FortGame, JsonConvert.SerializeObject(Saved.DeserializeGameConfig, Formatting.Indented));

                                        return Json(new
                                        {
                                            message = "Updated Content",
                                            error = false,
                                        });
                                    }
                                }
                                else if(Context == "ini")
                                {

                                    int NewsID = int.TryParse(FormRequest["newsId"], out int tempNewsID) ? tempNewsID : 0;

                                    IniConfigFiles iniConfigFiles = IniManager.IniConfigData.FileData[SectionId];
                                    if(iniConfigFiles != null)
                                    {
                                        iniConfigFiles.UploadedTime = DateTime.UtcNow;

                                        IniConfigValues iniConfigValues = iniConfigFiles.Data[NewsID].Data[ArrayIndex];

                                        if (iniConfigValues != null)
                                        {

                                            iniConfigValues.Name = Title;
                                            iniConfigValues.Value = Body;
                                        }

                                        System.IO.File.WriteAllText(PathConstants.CloudStorage.IniConfig, JsonConvert.SerializeObject(IniManager.IniConfigData, Formatting.Indented));
                                    }
                                 
                                    
                                }
                                // Yippie!!
                                else if( Context == "config")
                                {
                                    // CANNOT ALLOW ANY MODS OR RANDOMS EVEN TRY TO EDIT CONFIG
                                    if (adminData.RoleId > AdminDashboardRoles.Moderator)
                                    {
                                        ConfigTop Configtop = DashboardConfigData.GetDashboardConfigData()[SectionId];
                                        if (Configtop != null)
                                        {
                                            ConfigData Configdata = Configtop.Data[ArrayIndex];

                                            if (Configdata != null)
                                            {
                                                FortConfig fortConfig = Saved.DeserializeConfig;
                                                PropertyInfo property = typeof(FortConfig).GetProperty(SectionPart);
                                                if (property != null)
                                                {
                                                    var currentValue = property.GetValue(fortConfig);
                                                    if(currentValue != null) 
                                                    {
                                                        if (Configdata.Type == "string" || Configdata.Type == "ulong")
                                                        {
                                                            if (!currentValue.Equals(Body))
                                                            {
                                                                property.SetValue(fortConfig, Body);
                                                            }
                                                        }
                                                        else if(Configdata.Type == "bool")
                                                        {
                                                            if (!currentValue.Equals(RadioValue))
                                                            {
                                                                property.SetValue(fortConfig, RadioValue);
                                                            }
                                                        }
                                                        else if(Configdata.Type == "int")
                                                        {
                                                            if (!currentValue.Equals(numberBox))
                                                            {
                                                                property.SetValue(fortConfig, numberBox);
                                                            }
                                                        }
                                                        else if (Configdata.Type == "float")
                                                        {
                                                            if (!currentValue.Equals(numberBox))
                                                            {
                                                                property.SetValue(fortConfig, numberBox);
                                                            }
                                                        }


                                                        // just gonna do it every time
                                                        var FortConfigPath = PathConstants.CachedPaths.FortConfig;
                                                        if (System.IO.File.Exists(FortConfigPath))
                                                        {
                                                            var configJson = JsonConvert.SerializeObject(fortConfig, Formatting.Indented);
                                                            System.IO.File.WriteAllText(FortConfigPath, configJson);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        /*   if (tempData.TryGetProperty("IniChanges", out JsonElement IniChangesData))
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

                                                           item.UploadedTime = DateTime.UtcNow;
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
                           }*/

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating temp data: {ex.Message}", "[Dashboard]");
                return Json(false);
            }

            return Json(new
            {
                message = "Failed To Update Content",
                error = false,
            });
        }
    }
}
