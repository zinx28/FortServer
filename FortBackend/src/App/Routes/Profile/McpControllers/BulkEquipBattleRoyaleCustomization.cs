using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using FortLibrary.EpicResponses.Profile;
using FortLibrary;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class BulkEquipBattleRoyaleCustomization
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, BulkEquipBattleRoyaleCustomizationResponse Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> ProfileChanges = new List<object>();
                var UpdatedData = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots;

                foreach(var item in Body.LoadoutData)
                {
                    string SlotName = item.slotName.ToLower();
                    string itemToSlot = item.slotName;
                    int IndexWithinSlot = item.indexWithinSlot;
                    if (IndexWithinSlot > 6) IndexWithinSlot = 6;

                    if (SlotName == "dance" || SlotName == "itemwrap")
                    {
                        if (IndexWithinSlot == -1)
                        {
                            if (SlotName == "dance")
                            {
                                return new Mcp();
                            }
                            List<string> ReplacedItems = Enumerable.Repeat(itemToSlot, 6).ToList();
                            UpdatedData.GetSlotName(SlotName).items = ReplacedItems;
                        }
                        else
                        {
                            UpdatedData.GetSlotName(SlotName).items[IndexWithinSlot] = itemToSlot;
                        }
                    }
                    else
                    {
                        UpdatedData.GetSlotName(SlotName).items = new List<string>
                        {
                            item.itemToSlot.ToLower(),
                        };
                    }

                    // sets item seen to true <3
                    if(profileCacheEntry.AccountData.athena.Items.ContainsKey(item.itemToSlot))
                    {
                        profileCacheEntry.AccountData.athena.Items[item.itemToSlot].attributes.item_seen = true;
                    }
                 

                    ProfileChanges.Add(new {
                        changeType ="statModified",
                        name = $"favorite_{item.slotName.ToLower()}",
                        value = UpdatedData.GetSlotName(item.slotName.ToLower()).items,
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

