using Discord;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
//using MongoDB.Bson.IO;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class SetBattleRoyaleBanner
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, SetBattleRoyaleBannerReq Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> ProfileChanges = new List<object>();
                var UpdatedData = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes;
                var BannerColorId = Body.homebaseBannerColorId;
                var BannerIconId = Body.homebaseBannerIconId;
               
                if(UpdatedData != null)
                {
                    UpdatedData.banner_icon_template = BannerIconId;
                    UpdatedData.banner_color_template = BannerColorId;

                    ProfileChanges.Add(new {
                        changeType = "statModified",
                        name = "banner_icon",
                        value = BannerIconId,
                    });

                    ProfileChanges.Add(new
                    {
                        changeType = "statModified",
                        name = "banner_color",
                        value = BannerColorId,
                    });

                    if (ProfileChanges.Count > 0)
                    {
                        profileCacheEntry.LastUpdated = DateTime.Now;
                        profileCacheEntry.AccountData.athena.RVN += 1;
                        profileCacheEntry.AccountData.athena.CommandRevision += 1;
                        profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes = UpdatedData;
                    }

                    List<dynamic> ProfileChangesV2 = new List<dynamic>();
                    if (Season.SeasonFull >= 12.20)
                    {
                        Mcp AthenaData = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                        ProfileChangesV2 = AthenaData.profileChanges;
                    }
                    else
                    {
                        ProfileChangesV2 = ProfileChanges;
                    }

                    return new Mcp()
                    {
                        profileRevision = profileCacheEntry.AccountData.athena.RVN + 1,
                        profileId = ProfileId,
                        profileChangesBaseRevision = BaseRev + 1,
                        profileChanges = ProfileChangesV2,
                        profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision + 1,
                        serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                        responseVersion = 1
                    };
                }
            }

            return new Mcp();
        }
    }
}
