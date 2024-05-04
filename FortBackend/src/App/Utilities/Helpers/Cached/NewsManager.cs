using FortBackend.src.App.Utilities.Constants;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.FortniteServices.Content;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Helpers.Cached
{
    public class NewsManager
    {
        public static ContentJson ContentJsonResponse = new ContentJson();
        public static ContentConfig ContentConfig = new ContentConfig();

        public static void Init()
        {
            var jsonData = System.IO.File.ReadAllText(PathConstants.Content);

            if (!string.IsNullOrEmpty(jsonData))
            {
                ContentConfig = JsonConvert.DeserializeObject<ContentConfig>(jsonData)!;

                if (ContentConfig != null)
                {
                    ContentConfig.battleroyalenews.motds.ForEach(x =>
                    {
                        ContentJsonResponse.battleroyalenews.news.motds.Add(new NewContentMotds()
                        {
                            image = x.image,
                            title = x.title,
                            body = x.body,
                        });

                        ContentJsonResponse.battleroyalnewsv2.news.motds.Add(new NewContentV2Motds()
                        {
                            image = x.image,
                            title = x.title,
                            body = x.body,
                        });
                    });

                    ContentJsonResponse.loginMessage.loginmessage.message.title = ContentConfig.loginmessage.title;
                    ContentJsonResponse.loginMessage.loginmessage.message.body = ContentConfig.loginmessage.body;

                    ContentConfig.battleroyalenews.messages.ForEach(x =>
                    {
                        ContentJsonResponse.battleroyalenews.news.messages.Add(new NewContentMessages()
                        {
                            image = x.image,
                            title = x.title,
                            body = x.body,
                        });
                    });

                    ContentConfig.emergencynotice.ForEach(x =>
                    {
                        ContentJsonResponse.emergencynotice.news.messages.Add(new EmergencyNoticeNewsMessages()
                        {
                            title = x.title,
                            body = x.body,
                        });

                        ContentJsonResponse.emergencynoticev2.emergencynotices.emergencynotices.Add(new EmergencyNoticeNewsV2Messages()
                        {
                            title = x.title,
                            body = x.body,
                        });
                    });

                    ContentConfig.shopSections.ForEach(x =>
                    {
                        ContentJsonResponse.shopSections.sectionList.sections.Add(new ShopSectionsSectionsSEctions
                        {
                            sectionId = x.sectionId,
                            sectionDisplayName = x.sectionDisplayName,
                            landingPriority = x.landingPriority,
                        });
                    });

                    ContentConfig.tournamentinformation.ForEach(x =>
                    {
                        ContentJsonResponse.tournamentinformation.tournament_info.tournaments.Add(x);
                    });

                    ContentConfig.playlistinformation.ForEach(x =>
                    {
                        ContentJsonResponse.playlistinformation.playlist_info.playlists.Add(x);
                    });
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
            ContentJsonResponse = new ContentJson(); // clear ofc

            if (ContentConfig != null)
            {
                System.IO.File.WriteAllText(PathConstants.Content, JsonConvert.SerializeObject(ContentConfig));

                ContentConfig.battleroyalenews.motds.ForEach(x =>
                {
                    ContentJsonResponse.battleroyalenews.news.motds.Add(new NewContentMotds()
                    {
                        image = x.image,
                        title = x.title,
                        body = x.body,
                    });

                    ContentJsonResponse.battleroyalnewsv2.news.motds.Add(new NewContentV2Motds()
                    {
                        image = x.image,
                        title = x.title,
                        body = x.body,
                    });
                });

                ContentJsonResponse.loginMessage.loginmessage.message.title = ContentConfig.loginmessage.title;
                ContentJsonResponse.loginMessage.loginmessage.message.body = ContentConfig.loginmessage.body;

                ContentConfig.battleroyalenews.messages.ForEach(x =>
                {
                    ContentJsonResponse.battleroyalenews.news.messages.Add(new NewContentMessages()
                    {
                        image = x.image,
                        title = x.title,
                        body = x.body,
                    });
                });

                ContentConfig.emergencynotice.ForEach(x =>
                {
                    ContentJsonResponse.emergencynotice.news.messages.Add(new EmergencyNoticeNewsMessages()
                    {
                        title = x.title,
                        body = x.body,
                    });

                    ContentJsonResponse.emergencynoticev2.emergencynotices.emergencynotices.Add(new EmergencyNoticeNewsV2Messages()
                    {
                        title = x.title,
                        body = x.body,
                    });
                });

                ContentConfig.shopSections.ForEach(x =>
                {
                    ContentJsonResponse.shopSections.sectionList.sections.Add(new ShopSectionsSectionsSEctions
                    {
                        sectionId = x.sectionId,
                        sectionDisplayName = x.sectionDisplayName,
                        landingPriority = x.landingPriority,
                    });
                });

                ContentConfig.tournamentinformation.ForEach(x =>
                {
                    ContentJsonResponse.tournamentinformation.tournament_info.tournaments.Add(x);
                });

                ContentConfig.playlistinformation.ForEach(x =>
                {
                    ContentJsonResponse.playlistinformation.playlist_info.playlists.Add(x);
                });
            }
            Logger.Log("RE-LOADED NEWS CONFIG");
        }
    }
}
