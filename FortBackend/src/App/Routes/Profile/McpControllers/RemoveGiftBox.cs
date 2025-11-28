using FortLibrary.EpicResponses.Profile;
using FortLibrary;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Discord.Helpers.command;
using FortLibrary.EpicResponses.Profile.Query;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using FortLibrary.EpicResponses.Profile.Query.Items;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class RemoveGiftBox
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, RemoveGiftBoxReq requestBodyy)
        {
            string currentDate = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            if (ProfileId == "common_core")
            {
                int BaseRev = profileCacheEntry.AccountData.commoncore.RVN;

                List<object> MultiUpdates = new List<object>();
                List<object> ProfileChanges = new List<object>();

                if(!string.IsNullOrEmpty(requestBodyy.giftBoxItemId))
                {
                    Logger.Warn(requestBodyy.giftBoxItemId, "SingleBox");
                    string GiftBoxID = requestBodyy.giftBoxItemId.ToString();
                    GiftCommonCoreItem tests = profileCacheEntry.AccountData.commoncore.Gifts.FirstOrDefault(e => e.Key == GiftBoxID).Value;
                    if(tests != null)
                    {
                        Logger.Log("Removed GiftBox");
                        profileCacheEntry.AccountData.commoncore.Gifts.Remove(GiftBoxID);
                        MultiUpdates.Add(new
                        {
                            changeType = "itemRemoved",
                            itemId = GiftBoxID
                        });
                    }
                }
                else if(requestBodyy.giftBoxItemIds.Count() > 0)
                {
                    Logger.PlainLog(requestBodyy.giftBoxItemIds.Count());
                    foreach (var GiftBoxID in requestBodyy.giftBoxItemIds)
                    {
                        GiftCommonCoreItem GiftBoxItem = profileCacheEntry.AccountData.commoncore.Gifts.FirstOrDefault(e => e.Key == GiftBoxID).Value;
                        if (GiftBoxItem != null)
                        {
                            Logger.Log("Removed GiftBox");
                            profileCacheEntry.AccountData.commoncore.Gifts.Remove(GiftBoxID);
                            MultiUpdates.Add(new
                            {
                                changeType = "itemRemoved",
                                itemId = GiftBoxID
                            });
                        }
                        
                    }
                }


                if (MultiUpdates.Count > 0)
                {
                    profileCacheEntry.AccountData.commoncore.RVN += 1;
                    profileCacheEntry.AccountData.commoncore.CommandRevision += 1;
                    profileCacheEntry.LastUpdated = DateTime.UtcNow;
                }

                if (BaseRev != RVN)
                {
                    Mcp test = await CommonCoreResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    ProfileChanges = test.profileChanges;
                }
                else
                {
                    ProfileChanges = MultiUpdates;
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev,
                    profileChanges = ProfileChanges,
                    profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };

            }
            else
            {
                Logger.Error("TRHIS SHOULDNT BE CALLED", ProfileId);
            }
            return new Mcp();
        }
    }
}
