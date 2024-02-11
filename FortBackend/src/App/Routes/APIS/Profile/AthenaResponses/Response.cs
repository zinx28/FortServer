using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FortBackend.src.App.Routes.APIS.Profile.AthenaResponses
{
    public class Response
    {
        public static async Task<Class> AthenaResponse(string AccountId, string ProfileId, int Season, string RVN, Account AthenaDataParsed)
        {
            try
            {
                bool FoundSeasonDataInProfile = false;
                foreach (dynamic SeasonData in AthenaDataParsed.commoncore.Seasons)
                {
                    if (SeasonData.SeasonNumber == Season)
                    {
                        FoundSeasonDataInProfile = true;
                    }
                }

                if (!FoundSeasonDataInProfile)
                {
                    string seasonJson = JsonConvert.SerializeObject(new Season
                    {
                        SeasonNumber = Season,
                        BookLevel = 1,
                        BookXP = 0,
                        BookPurchased = false,
                        Quests = new List<Dictionary<string, object>>(),
                        BattleStars = 0,
                        DailyQuests = new DailyQuests
                        {
                            Interval = "0001-01-01T00:00:00.000Z",
                            Rerolls = 1
                        }
                    });

                    await Handlers.PushOne<Account>("accountId", AccountId, new Dictionary<string, object>
                    {
                        {
                            "common_core.Season", BsonDocument.Parse(seasonJson)
                        }
                    });
                }


            }
            catch (Exception ex)
            {
                Logger.Error($"AthenaResponse: {ex.Message}");
            }

            return new Class();
        }
    }
}
