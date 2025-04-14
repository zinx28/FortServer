using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Reflection;
using static FortBackend.src.App.Utilities.ADMIN.DashboardConfigData;


namespace FortBackend.src.App.Routes.ADMIN.NewFolder
{
    /*
     *  Goal is to support any request and update will it ever no! IM SKUNEKD ASF!
     */
    public class ContentUpdate
    {
        public class IniContextRequest
        {
            public int index { get; set; }
            public dynamic value { get; set; } // skunked response
        }
        public class ServerContentRequest
        {
            public bool ForcedSeason { get; set; }
            public int Season { get; set; } = 0;
            public int WeeklyQuests { get; set; } = 0;
            public bool ShopRotation { get; set; } 
        }
        public class ContentRequest
        {
            public Languages title { get; set; } = new();
            public Languages? display_name { get; set; } = new();
            public Languages? body { get; set; } = new();
            public Languages? description { get; set; } = new();
            public string? image { get; set; } = null;
        }
        public static object Update(string Body, string ContentName, string ContentID, int Index)
        {
          
            // News Updates
            Console.WriteLine(Body);
            Console.WriteLine(ContentName);
            Console.WriteLine(ContentID);
            Console.WriteLine(Index);
            if (ContentName == "news")
            {
                ContentRequest contentRequest = JsonConvert.DeserializeObject<ContentRequest>(Body)!;
                if (contentRequest != null)
                {
                    if (ContentID == "1" && !string.IsNullOrEmpty(contentRequest.image))
                    {
                        // News Content
                        var battleroyaleNews = NewsManager.ContentConfig.battleroyalenews;

                        var NewCombineed = battleroyaleNews.motds
                            .Select(e => e)
                            .Concat(battleroyaleNews.messages
                            .Select(e => e))
                            .ToList();

                        NewCombineed[Index].title = contentRequest.title;
                        if (contentRequest.body is not null)
                            NewCombineed[Index].body = contentRequest.body;

                        NewCombineed[Index].image = contentRequest.image;

                        NewsManager.Update();

                        return new
                        {
                            message = "Updated Content",
                            error = false,
                        };
                    }
                    else if (ContentID == "2")
                    {
                        // Emergency
                        var EmergencyContent = NewsManager.ContentConfig.emergencynotice[Index];

                        EmergencyContent.title = contentRequest.title;
                        if (contentRequest.body is not null)
                            EmergencyContent.body = contentRequest.body;

                        NewsManager.Update();

                        return new
                        {
                            message = "Updated Content",
                            error = false,
                        };
                    }
                    else if (ContentID == "3")
                    {
                        var LoginMessage = NewsManager.ContentConfig.loginmessage;

                        LoginMessage.title = contentRequest.title;
                        if (contentRequest.body is not null)
                            LoginMessage.body = contentRequest.body;
 

                        NewsManager.Update();

                        return new
                        {
                            message = "Updated Content",
                            error = false,
                        };
                    }
                    else if (ContentID == "4")
                    {
                        var PlaylistInfo = NewsManager.ContentConfig.playlistinformation[Index];

                        if (contentRequest.display_name is not null)
                            PlaylistInfo.display_name = contentRequest.display_name;

                        if (contentRequest.image is not null)
                            PlaylistInfo.image = contentRequest.image;

                        if (contentRequest.description is not null)
                            PlaylistInfo.description = contentRequest.description;

                        NewsManager.Update();

                        return new
                        {
                            message = "Updated Content",
                            error = false,
                        };
                    }
                }
            }
            else if (ContentName == "server")
            {
                // server updates
                ServerContentRequest serverContentRequest = JsonConvert.DeserializeObject<ServerContentRequest>(Body)!;
                if (serverContentRequest != null)
                {
                    // i mean theres only 1 so why check
                    Saved.DeserializeGameConfig.ForceSeason = serverContentRequest.ForcedSeason;
                    Saved.DeserializeGameConfig.Season = serverContentRequest.Season;
                    Saved.DeserializeGameConfig.WeeklyQuest = serverContentRequest.WeeklyQuests;
                    Saved.DeserializeGameConfig.ShopRotation = serverContentRequest.ShopRotation;

                    System.IO.File.WriteAllText(PathConstants.CachedPaths.FortGame, JsonConvert.SerializeObject(Saved.DeserializeGameConfig, Formatting.Indented));

                    return new
                    {
                        message = "Updated Content",
                        error = false,
                    };
                }
            }
            else if(ContentName == "ini")
            {
                // INI UPDATES ~ should actually be fine now with body
                int ContentIDIndex = int.TryParse(ContentID, out int TempIndex) ? TempIndex : 0;

                IniConfigFiles iniConfigFiles = IniManager.IniConfigData.FileData[ContentIDIndex];

                if (iniConfigFiles != null)
                {
                    IniConfigData iniConfigData = iniConfigFiles.Data[Index];
                    if (iniConfigData != null && iniConfigData.Data.Count > 0) {
                        List<IniContextRequest> ResponseConv = JsonConvert.DeserializeObject<List<IniContextRequest>>(Body)!;

                        // could be out of index?
                        ResponseConv.ForEach(x =>
                        {
                            iniConfigData.Data[x.index].Value = x.value;
                        });

                        System.IO.File.WriteAllText(PathConstants.CloudStorage.IniConfig, JsonConvert.SerializeObject(IniManager.IniConfigData, Formatting.Indented));

                        return new
                        {
                            message = "Updated Content",
                            error = false,
                        };
                    }
                }

            }
            else if(ContentName == "config")
            {
                // this is like skunked and i dont want to recode
                int ContentIDIndex = int.TryParse(ContentID, out int TempIndex) ? TempIndex : 0;

                List<ConfigData> Configtop = DashboardConfigData.GetDashboardConfigData()[ContentIDIndex].Data;
                if (Configtop != null && Configtop.Count > 0) // shouldnt ever be 0 :/
                {
                    List<IniContextRequest> ResponseConv = JsonConvert.DeserializeObject<List<IniContextRequest>>(Body)!;
                    FortConfig fortConfig = Saved.DeserializeConfig;

                    Console.WriteLine("YHEA");

                    // the dashboardconfigdata isnt used anywhere frtom the dashbaord and changing data wont simply work!
                    ResponseConv.ForEach(x =>
                    {
                        ConfigData Configdata = Configtop[x.index];
                        Console.WriteLine(Configdata.Title);
                        Console.WriteLine(JsonConvert.SerializeObject(Configdata));
                        PropertyInfo property = typeof(FortConfig).GetProperty(Configdata.METADATA)!;
                        if (property != null)
                        {
                            var currentValue = property.GetValue(fortConfig);
                            if (currentValue != null)
                            {
                                Console.WriteLine("E");
                                if (Configdata.Type == "string" || Configdata.Type == "ulong")
                                {
                                    if (!currentValue.Equals(x.value))
                                    {
                                        property.SetValue(fortConfig, x.value);
                                    }
                                }
                                else if (Configdata.Type == "bool")
                                {
                                    // converts value to bool...
                                    bool BoolValue = false;
                                    Console.WriteLine("TEST");
                                    Console.WriteLine(x.value);
                                    try { BoolValue = bool.Parse(x.value); } catch { };

                                    if (!currentValue.Equals(BoolValue))
                                    {
                                        property.SetValue(fortConfig, BoolValue);
                                    }
                                }
                                else if (Configdata.Type == "int")
                                {
                                    // converts value to int
                                    int IntValue = 0;
                                    try { IntValue = int.Parse(x.value); } catch { };

                                    if (!currentValue.Equals(IntValue))
                                    {
                                        property.SetValue(fortConfig, IntValue);
                                    }
                                }
                            }
                        }
                    });

                    var FortConfigPath = PathConstants.CachedPaths.FortConfig;
                    if (System.IO.File.Exists(FortConfigPath))
                    {
                        var configJson = JsonConvert.SerializeObject(fortConfig, Formatting.Indented);
                        System.IO.File.WriteAllText(FortConfigPath, configJson);

                        return new
                        {
                            message = "Updated Content",
                            error = false,
                        };
                    }
                }
            }

            return new
            {
                message = "Failed To Update Content",
                error = false,
            };
        }
    }
}
