using Discord;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Extentions;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;
//using MongoDB.Bson.IO;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class SetCosmeticLockerBanner
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, SetCosmeticLockerBannerReq Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> ProfileChanges = new List<object>();

                if(!string.IsNullOrEmpty(Body.lockerItem))
                {
                    var UpdatedData = profileCacheEntry.AccountData.athena.loadouts_data![Body.lockerItem].attributes;

                    if (!string.IsNullOrEmpty(Body.bannerIconTemplateName))
                    {
                        UpdatedData.banner_icon_template = Body.bannerIconTemplateName;

                        ProfileChanges.Add(new
                        {
                            changeType = "itemAttrChanged",
                            itemId = Body.lockerItem,
                            attributeName = "banner_icon_template",
                            attributeValue = UpdatedData.banner_icon_template
                        });

                    }

                    if (!string.IsNullOrEmpty(Body.bannerColorTemplateName))
                    {
                        UpdatedData.banner_color_template = Body.bannerColorTemplateName;

                        ProfileChanges.Add(new
                        {
                            changeType = "itemAttrChanged",
                            itemId = Body.lockerItem,
                            attributeName = "banner_color_template",
                            attributeValue = UpdatedData.banner_color_template
                        });
                    }
                }
                

                if (ProfileChanges.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.Now;
                    profileCacheEntry.AccountData.athena.BumpRevisions();
                }

                if (BaseRev_G != RVN)
                {
                    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    ProfileChanges = test.profileChanges;
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev,
                    profileChanges = ProfileChanges,
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };
                
            }

            return new Mcp();
        }
    }
}
