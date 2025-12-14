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
    public class SetItemFavoriteStatusBatch
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, SetItemFavoriteStatusBatchReq Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> ProfileChanges = new List<object>();
                SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;

                for (int i = 0; i < Body.itemIds.Count; i++)
                {
                    var Item = Body.itemIds[i];
                    var Favorite = Body.itemFavStatus[i];

                    if (profileCacheEntry.AccountData.athena.Items.ContainsKey(Item))
                    {
                        profileCacheEntry.AccountData.athena.Items[Item].attributes.favorite = Favorite;
                    }
                    else
                    {
                        if (seasonObject != null)
                        {
                            if (seasonObject.special_items.ContainsKey(Item))
                            {
                                seasonObject.special_items[Item].attributes.favorite = Favorite;
                            }
                        }
                    }

                    ProfileChanges.Add(new
                    {
                        itemId = Item,
                        attributeName = "item_seen",
                        attributeValue = Favorite
                    });
                }


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
