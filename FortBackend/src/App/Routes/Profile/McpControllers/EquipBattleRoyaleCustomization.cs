using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.EpicResponses.Profile;
using FortLibrary;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class EquipBattleRoyaleCustomization
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, EquipBattleRoyaleCustomizationRequest Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> ProfileChanges = new List<object>();
                var UpdatedData = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots;
                var slotName = Body.slotName.ToLower();
                var itemToSlot = Body.itemToSlot.ToLower() ?? "";
                var IndexWithinSlot = Body.indexWithinSlot;
                if (IndexWithinSlot > 6)
                {
                    IndexWithinSlot = 6;
                }

                if (UpdatedData != null)
                {
                    if (slotName == "itemwrap" || slotName == "dance")
                    {
                        // emote, wraps
                        if (IndexWithinSlot == -1)
                        {
                            if (slotName == "dance")
                            {
                                return new Mcp();
                            }
                            List<string> ReplacedItems = Enumerable.Repeat(itemToSlot, 6).ToList();
                            UpdatedData.itemwrap.items = ReplacedItems;
                            ProfileChanges.Add(new {
                                changeType = "statModified",
                                name = $"favorite_{slotName}",
                                value = ReplacedItems
                            });
                        }
                        else
                        {
                            UpdatedData.GetSlotName(slotName).items[IndexWithinSlot] = itemToSlot;
                            ProfileChanges.Add(new {
                                changeType = "statModified",
                                name = $"favorite_{slotName}",
                                value =  UpdatedData.GetSlotName(slotName).items
                            });
                        }
                    }
                    else
                    {
                        UpdatedData.GetSlotName(slotName).items = new List<string>() { itemToSlot };
     
                        ProfileChanges.Add(new {
                            changeType = "statModified",
                            name = $"favorite_{slotName}",
                            value = itemToSlot
                        });
                    }

                  

                    if (ProfileChanges.Count > 0)
                    {
                        profileCacheEntry.LastUpdated = DateTime.Now;
                        profileCacheEntry.AccountData.athena.RVN += 1;
                        profileCacheEntry.AccountData.athena.CommandRevision += 1;
                        profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots = UpdatedData;
                    }

                    if (BaseRev != RVN)
                    {
                        Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                        ProfileChanges = test.profileChanges;
                    }

                    //if(Season.Season == 1)
                    //{
                    //    return new Mcp()
                    //    {
                    //        profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    //        profileId = ProfileId,
                    //        profileChangesBaseRevision = BaseRev,
                    //        profileChanges = ProfileChanges,
                    //        profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    //        serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                    //        responseVersion = 1
                    //    };
                    //}

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
            }

            return new Mcp();
        }
    }
}
