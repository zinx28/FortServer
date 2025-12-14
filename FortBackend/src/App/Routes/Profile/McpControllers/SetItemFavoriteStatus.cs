using Discord;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Extentions;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB.Module;
using FortLibrary.Shop;

//using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System.Linq;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class SetItemFavoriteStatus
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, SetItemFavoriteStatusReq Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> ProfileChanges = new List<object>();

                if (profileCacheEntry.AccountData.athena.Items.ContainsKey(Body.targetItemId))
                {
                    profileCacheEntry.AccountData.athena.Items[Body.targetItemId].attributes.favorite = Body.bFavorite;
                }
                else
                {
                    SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;

                    if (seasonObject != null)
                        if (seasonObject.special_items.ContainsKey(Body.targetItemId))
                        {
                            seasonObject.special_items[Body.targetItemId].attributes.favorite = Body.bFavorite;
                        }
                }

                ProfileChanges.Add(new
                {
                    itemId = Body.targetItemId,
                    attributeName = "favorite",
                    attributeValue = Body.bFavorite
                });

                if (ProfileChanges.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.Now;
                    profileCacheEntry.AccountData.athena.BumpRevisions();
                }

                if (BaseRev_G != RVN)
                {
                    Mcp AthenaData = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    ProfileChanges = AthenaData.profileChanges;
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
