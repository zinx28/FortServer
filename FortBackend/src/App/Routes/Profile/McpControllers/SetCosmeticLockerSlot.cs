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
    public class SetCosmeticLockerSlot
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, SetCosmeticLockerSlotRequest Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> ProfileChanges = new List<object>();
                var UpdatedData = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data;
                var slotName = Body.category.ToLower();
                var itemToSlot = Body.itemToSlot.ToLower() ?? "";
                var IndexWithinSlot = Body.slotIndex;
                if (IndexWithinSlot > 6)
                {
                    IndexWithinSlot = 6;
                }

                if (slotName == "itemwrap" || slotName == "dance")
                {
                    // emote, wraps soon upcoming
                    if (IndexWithinSlot == -1)
                    {
                        if (slotName == "Dance")
                        {
                            return new Mcp();
                        }

                        List<string> ReplacedItems = Enumerable.Repeat(itemToSlot, 6).ToList();
                        UpdatedData.slots.itemwrap.items = ReplacedItems;
                        ProfileChanges.Add(new List<object>()
                        {
                            new
                            {
                                 changeType = "itemAttrChanged",
                                 itemId = Body.lockerItem,
                                 attributeName = "locker_slots_data",
                                 attributeValue = UpdatedData
                            }
                        });
                    }
                    else
                    {
                        UpdatedData.slots.GetSlotName(slotName).items[IndexWithinSlot] = itemToSlot;
                        ProfileChanges.Add(new List<object>()
                        {
                            new
                            {
                                 changeType = "itemAttrChanged",
                                 itemId = Body.lockerItem,
                                 attributeName = "locker_slots_data",
                                 attributeValue = UpdatedData
                            }
                        });
                    }
                }
                else
                {
                    UpdatedData.slots.GetSlotName(slotName).items = new List<string>() { itemToSlot };
                    ProfileChanges.Add(new List<object>()
                    {
                        new
                        {
                                changeType = "itemAttrChanged",
                                itemId = Body.lockerItem,
                                attributeName = "locker_slots_data",
                                attributeValue = UpdatedData
                        }
                    });
                }

                if (ProfileChanges.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.Now;
                    profileCacheEntry.AccountData.athena.RVN += 1;
                    profileCacheEntry.AccountData.athena.CommandRevision += 1;
                    profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data = UpdatedData;
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

            return new Mcp();
        }
    }
}
