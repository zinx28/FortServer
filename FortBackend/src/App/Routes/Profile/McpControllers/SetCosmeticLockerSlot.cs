using Discord;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB.Module;
//using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System.Linq;
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
                //var UpdatedData = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data;
                var slotName = Body.category.ToLower();
                var itemToSlot = Body.itemToSlot.ToLower() ?? "";
                var IndexWithinSlot = Body.slotIndex;
                if (IndexWithinSlot > 6)
                {
                    IndexWithinSlot = 6;
                }

                List<string> SpecialItems = new List<string>
                {
                    "",
                    "cid_random",
                    "glider_random",
                    "pickaxe_random",
                    "pickaxe_random",
                    "lsid_random",
                };

                if (itemToSlot != "" &&
                        !(itemToSlot.Contains(":") &&
                        (SpecialItems.Contains(itemToSlot) ||
                            SpecialItems.Contains(itemToSlot.Split(":")[1]) ||
                            itemToSlot == ":")))
                {
                    if (!Saved.DeserializeConfig.FullLockerForEveryone)
                    {
                        AthenaItem FoundAccItem = profileCacheEntry.AccountData.athena.Items.FirstOrDefault(e => e.Key.ToLower() == itemToSlot).Value;
                        if (FoundAccItem == null)
                        {
                            throw new BaseError
                            {
                                errorCode = "errors.com.epicgames.fortnite.invalid_parameter",
                                errorMessage = $"Profile does not own item {itemToSlot} (slot {IndexWithinSlot})",
                                messageVars = new List<string> { itemToSlot },
                                numericErrorCode = 16040,
                                originatingService = "any",
                                intent = "prod",
                                error_description = $"Profile does not own item {itemToSlot} (slot {IndexWithinSlot})",
                            };
                        }
                    }
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

                        List<string> ReplacedItems = Enumerable.Repeat(itemToSlot, 7).ToList();
                        profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots.itemwrap.items = ReplacedItems;
                        ProfileChanges.Add(new
                        {
                            changeType = "itemAttrChanged",
                            itemId = Body.lockerItem,
                            attributeName = "locker_slots_data",
                            attributeValue = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data
                        });
                    }
                    else
                    {
                        profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots.GetSlotName(slotName).items[IndexWithinSlot] = itemToSlot;
                        ProfileChanges.Add(new
                        {
                            changeType = "itemAttrChanged",
                            itemId = Body.lockerItem,
                            attributeName = "locker_slots_data",
                            attributeValue = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data
                        });
                    }
                }
                else
                {
                    profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots.GetSlotName(slotName).items = new List<string>() { itemToSlot };
                    ProfileChanges.Add(new
                    {
                        changeType = "itemAttrChanged",
                        itemId = Body.lockerItem,
                        attributeName = "locker_slots_data",
                        attributeValue = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data
                    });
                }


                if (Body.variantUpdates.Count > 0)
                {
                    var Variants = new List<AthenaItemVariants>();
                    if (!Saved.DeserializeConfig.FullLockerForEveryone)
                    {
                        if (profileCacheEntry.AccountData.athena.Items[Body.itemToSlot] != null)
                        {
                            Variants = profileCacheEntry.AccountData.athena.Items[Body.itemToSlot].attributes.variants;
                        }
                    }
                    else
                    {
                        // not the best full locker tbh
                        var item = Saved.BackendCachedData.FullLocker_AthenaItems.FirstOrDefault(e => e.Key == Body.itemToSlot);
                        if (item.Value != null)
                        {
                            Variants = item.Value.attributes.variants;
                        }
                    }

                    foreach (var variant in Body.variantUpdates)
                    {
                        var FindVar = Variants.FirstOrDefault(e => e.channel == variant.channel);
                        if (FindVar != null)
                        {
                            FindVar.active = variant.active;
                        }
                    }

                    if (Variants.Count > 0)
                    {
                        ProfileChanges.Add(new
                        {
                            changeType = "itemAttrChanged",
                            itemId = Body.itemToSlot,
                            attributeName = "variants",
                            attributeValue = Variants
                        });

                        if (!Saved.DeserializeConfig.FullLockerForEveryone)
                            profileCacheEntry.AccountData.athena.Items[Body.itemToSlot].attributes.variants = Variants;

                        profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots.GetSlotName(slotName).activevariants = Variants;
                    }
                }

                if (ProfileChanges.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.Now;
                    profileCacheEntry.AccountData.athena.RVN += 1;
                    profileCacheEntry.AccountData.athena.CommandRevision += 1;
                   // profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data = UpdatedData;
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
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };
            }

            return new Mcp();
        }
    }
}
