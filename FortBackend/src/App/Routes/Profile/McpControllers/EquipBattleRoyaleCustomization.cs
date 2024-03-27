using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Module;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class EquipBattleRoyaleCustomization
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, EquipBattleRoyaleCustomizationRequest Body)
        {

            Console.WriteLine(Body);
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                Dictionary<string, object> UpdatedData = new Dictionary<string, object>();
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                //int GrabPlacement = GrabPlacement = profileCacheEntry.AccountData.athena.Items.SelectMany((item, index) => new List<(Dictionary<string, ProfileItem> Item, int Index)> { (Item: item, Index: index) })
                //.TakeWhile(pair => !pair.Item.ContainsKey("sandbox_loadout")).Count();
                List<object> ProfileChanges = new List<object>();

                var slotName = Body.slotName.ToLower();
                var itemToSlot = Body.itemToSlot.ToLower() ?? "";
                if (profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"] != null)
                {
                    if (slotName == "itemwrap" || slotName == "dance")
                    {
                        // emote, wraps soon upcoming
                        if (Body.indexWithinSlot == -1)
                        {
                            if (slotName == "dance")
                            {
                                return new Mcp();
                            }

                            var ItemsCount = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots.itemwrap.items.Count();
                            string[] ReplacedItems = Enumerable.Repeat(itemToSlot, ItemsCount).ToArray();
                            ProfileChanges.Add(new List<object>()
                            {
                                new
                                {
                                    changeType = "statModified",
                                    name = $"favorite_{slotName}",
                                    value = ReplacedItems
                                }
                            });
                        }
                        else
                        {
                            // emotes
                            profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots.dance.items[Body.indexWithinSlot] = itemToSlot;
                            ProfileChanges.Add(new List<object>()
                            {
                                new
                                {
                                    changeType = "statModified",
                                    name = $"favorite_{slotName}",
                                    value =  profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots.dance.items
                                }
                            });
                        }
                    }
                    else
                    {
                        profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes.locker_slots_data.slots.GetSlotName(slotName).items[0] = itemToSlot;
                        ProfileChanges.Add(new List<object>()
                        {
                            new
                            {
                                changeType = "statModified",
                                name = $"favorite_{slotName}",
                                value = itemToSlot
                            }
                        });
                    }

                    if (ProfileChanges.Count > 0)
                    {
                        profileCacheEntry.AccountData.athena.RVN += 1;
                        profileCacheEntry.AccountData.athena.CommandRevision += 1;
                        profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"] = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"];
                    }

                    if (BaseRev != RVN)
                    {
                        Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                        ProfileChanges = test.profileChanges;
                    }

                    Console.WriteLine(JsonConvert.SerializeObject(new
                    {
                        profileRevision = profileCacheEntry.AccountData.athena.RVN,
                        profileId = ProfileId,
                        profileChangesBaseRevision = BaseRev,
                        profileChanges = ProfileChanges,
                        profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                        serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                        responseVersion = 1
                    }));
                    return new Mcp()
                    {
                        profileRevision = profileCacheEntry.AccountData.athena.RVN,
                        profileId = ProfileId,
                        profileChangesBaseRevision = BaseRev,
                        profileChanges = ProfileChanges,
                        profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                        serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                        responseVersion = 1
                    };
                }
            }

            return new Mcp();
        }
    }
}
