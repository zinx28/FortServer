using FortBackend.src.App.Utilities.Constants;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.FortniteServices.Content;
using Newtonsoft.Json;
using System.Reflection;

namespace FortBackend.src.App.Utilities.Helpers.Cached
{
    public class NewsManager
    {
        public static Dictionary<string, ContentJson> ContentJsonResponse = new();
        public static Dictionary<string, motdTarget> MotdJsonResponse = new();
        //  public static ContentJson ContentJsonResponse = new ContentJson();
        public static ContentConfig ContentConfig = new();

        public static motdTarget MotdTarget = new();
        //public static void Init()
        //{

        //}
        public static void Init()
        {
            var jsonData = System.IO.File.ReadAllText(PathConstants.Content);

            if (!string.IsNullOrEmpty(jsonData))
            {
                Logger.Log("Loading News", "News");
                PropertyInfo[] properties = typeof(Languages).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    string propertyName = property.Name;

                    ContentConfig = JsonConvert.DeserializeObject<ContentConfig>(jsonData)!;

                    if (ContentConfig != null)
                    {
                        ContentJson contentJson = new();
                        motdTarget MotdcontentJson = new();

                        // 

                        ContentConfig.battleroyalenews.motds.ForEach(x =>
                        {
                            contentJson.battleroyalenews.news.motds.Add(new()
                            {
                                image = x.image,
                                tileImage = x.image,
                                tabTitleOverride = x.GetLanguage(x.title, propertyName),
                                title = x.GetLanguage(x.title, propertyName),
                                body = x.GetLanguage(x.body, propertyName),
                            });

                            contentJson.battleroyalnewsv2.news.motds.Add(new()
                            {
                                image = x.image,
                                tileImage = x.image,
                                title = x.GetLanguage(x.title, propertyName),
                                body = x.GetLanguage(x.body, propertyName),
                                tabTitleOverride = x.GetLanguage(x.title, propertyName)
                            });

                            MotdcontentJson.contentItems.Add(new()
                            {
                                contentFields = new()
                                {
                                    body = x.GetLanguage(x.body, propertyName),
                                    title = x.GetLanguage(x.title, propertyName),
                                    tabTitleOverride = x.GetLanguage(x.title, propertyName),
                                    image = new()
                                    {
                                        url = x.image
                                    },
                                    tileImage = new()
                                    {
                                        url = x.image
                                    }
                                }
                            });
                        });

                     //   MotdTarget.contentItems



                        contentJson.loginMessage.loginmessage.message.title = ContentConfig.loginmessage.GetLanguage(ContentConfig.loginmessage.title, propertyName);
                        contentJson.loginMessage.loginmessage.message.body = ContentConfig.loginmessage.GetLanguage(ContentConfig.loginmessage.body, propertyName);

                        ContentConfig.battleroyalenews.messages.ForEach(x =>
                        {
                            contentJson.battleroyalenews.news.messages.Add(new()
                            {
                                image = x.image,
                                title = x.GetLanguage(x.title, propertyName),
                                body = x.GetLanguage(x.body, propertyName),
                            });
                        });

                        ContentConfig.emergencynotice.ForEach(x =>
                        {
                            contentJson.emergencynotice.news.messages.Add(new()
                            {
                                title = x.GetLanguage(x.title, propertyName),
                                body = x.GetLanguage(x.body, propertyName),
                            });

                            contentJson.emergencynoticev2.emergencynotices.emergencynotices.Add(new()
                            {
                                title = x.GetLanguage(x.title, propertyName),
                                body = x.GetLanguage(x.body, propertyName),
                            });
                        });

                        ContentConfig.shopSections.ForEach(x =>
                        {
                            contentJson.shopSections.sectionList.sections.Add(new()
                            {
                                sectionId = x.sectionId,
                                sectionDisplayName = x.sectionDisplayName,
                                landingPriority = x.landingPriority,
                            });
                        });

                        ContentConfig.tournamentinformation.ForEach(x =>
                        {
                            contentJson.tournamentinformation.tournament_info.tournaments.Add(x);
                        });

                        ContentConfig.playlistinformation.ForEach(x =>
                        {
                            contentJson.playlistinformation.playlist_info.playlists.Add(new()
                            {
                                image = x.image,
                                playlist_name = x.playlist_name,
                                hidden = x.hidden,
                                description = x.GetLanguage(x.description, propertyName),
                                display_name = x.GetLanguage(x.display_name, propertyName),
                            });
                        });

                        Logger.Log($"Adding {propertyName}", "News");
                        ContentJsonResponse.Add(propertyName, contentJson);
                        MotdJsonResponse.Add(propertyName, MotdcontentJson);
                    }
                }
                Logger.Log("RE-LOADED NEWS CONFIG");
            }
            else
            {
                Logger.Error("CONTENT FILE IS NULL OR EMPTY");

            }
           
        }

        public static void Update()
        {
            ContentJsonResponse = new(); // clear ofc
            MotdJsonResponse = new();

            if (ContentConfig != null)
            {
                System.IO.File.WriteAllText(PathConstants.Content, JsonConvert.SerializeObject(ContentConfig));

                
                PropertyInfo[] properties = typeof(Languages).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    string propertyName = property.Name;

                    ContentJson contentJson = new ContentJson();
                    motdTarget MotdcontentJson = new();

                    ContentConfig.battleroyalenews.motds.ForEach(x =>
                    {
                        contentJson.battleroyalenews.news.motds.Add(new NewContentMotds()
                        {
                            image = x.image,
                            title = x.GetLanguage(x.title, propertyName),
                            body = x.GetLanguage(x.body, propertyName),
                        });

                        contentJson.battleroyalnewsv2.news.motds.Add(new NewContentV2Motds()
                        {
                            image = x.image,
                            title = x.GetLanguage(x.title, propertyName),
                            body = x.GetLanguage(x.body, propertyName),
                        });

                        MotdcontentJson.contentItems.Add(new()
                        {
                            contentFields = new()
                            {
                                body = x.GetLanguage(x.body, propertyName),
                                title = x.GetLanguage(x.title, propertyName),
                                tabTitleOverride = x.GetLanguage(x.title, propertyName),
                                image = new()
                                {
                                    url = x.image
                                },
                                tileImage = new()
                                {
                                    url = x.image
                                }
                            }
                        });
                    });

                    contentJson.loginMessage.loginmessage.message.title = ContentConfig.loginmessage.GetLanguage(ContentConfig.loginmessage.title, propertyName);
                    contentJson.loginMessage.loginmessage.message.body = ContentConfig.loginmessage.GetLanguage(ContentConfig.loginmessage.body, propertyName);

                    ContentConfig.battleroyalenews.messages.ForEach(x =>
                    {
                        contentJson.battleroyalenews.news.messages.Add(new NewContentMessages()
                        {
                            image = x.image,
                            title = x.GetLanguage(x.title, propertyName),
                            body = x.GetLanguage(x.body, propertyName),
                        });
                    });

                    ContentConfig.emergencynotice.ForEach(x =>
                    {
                        contentJson.emergencynotice.news.messages.Add(new EmergencyNoticeNewsMessages()
                        {
                            title = x.GetLanguage(x.title, propertyName),
                            body = x.GetLanguage(x.body, propertyName),
                        });

                        contentJson.emergencynoticev2.emergencynotices.emergencynotices.Add(new EmergencyNoticeNewsV2Messages()
                        {
                            title = x.GetLanguage(x.title, propertyName),
                            body = x.GetLanguage(x.body, propertyName),
                        });
                    });

                    ContentConfig.shopSections.ForEach(x =>
                    {
                        contentJson.shopSections.sectionList.sections.Add(new ShopSectionsSectionsSEctions
                        {
                            sectionId = x.sectionId,
                            sectionDisplayName = x.sectionDisplayName,
                            landingPriority = x.landingPriority,
                        });
                    });

                    ContentConfig.tournamentinformation.ForEach(x =>
                    {
                        contentJson.tournamentinformation.tournament_info.tournaments.Add(x);
                    });

                    ContentConfig.playlistinformation.ForEach(x =>
                    {
                        contentJson.playlistinformation.playlist_info.playlists.Add(new PlayListObject
                        {
                            image = x.image,
                            playlist_name = x.playlist_name,
                            hidden = x.hidden,
                            description = x.GetLanguage(x.description, propertyName),
                            display_name = x.GetLanguage(x.display_name, propertyName),
                        });
                    });

                    ContentJsonResponse.Add(propertyName, contentJson);
                    MotdJsonResponse.Add(propertyName, MotdcontentJson);
                }
            }
            Logger.Log("RE-LOADED NEWS CONFIG");
        }
    }
}
