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

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class SetCosmeticLockerSlot
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, SetCosmeticLockerSlotRequest Body)
        {

            Console.WriteLine(Body);
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                Dictionary<string, object> UpdatedData = new Dictionary<string, object>();
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                int GrabPlacement = GrabPlacement = profileCacheEntry.AccountData.athena.Items.SelectMany((item, index) => new List<(Dictionary<string, object> Item, int Index)> { (Item: item, Index: index) })
                .TakeWhile(pair => !pair.Item.ContainsKey("sandbox_loadout")).Count();

                if (Body.category == "ItemWrap" || Body.category == "Dance")
                {
                    // emote, wraps soon upcoming
                    if (Body.slotIndex == -1)
                    {
                        if (Body.category == "Dance")
                        {
                            return new Mcp();
                        }
                        var SandBoxLoadout = JsonConvert.DeserializeObject<SandboxLoadout>(JsonConvert.SerializeObject(profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"]));

                        if (SandBoxLoadout != null)
                        {
                            var ItemsCount = SandBoxLoadout.attributes.locker_slots_data.slots.itemwrap.items.Count();
                            string[] ReplacedItems = Enumerable.Repeat(Body.itemToSlot.ToLower(), ItemsCount).ToArray();

                            profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"] = SandBoxLoadout;//["attributes"];//.locker_slots_data.slots[Body.category.ToLower()].items = ReplacedItems;
                            //UpdatedData.Add($"athena.items.{GrabPlacement}.sandbox_loadout.attributes.locker_slots_data.slots.{Body.category.ToLower()}.items", ReplacedItems);
                        }
                    }
                    else
                    {
                        dynamic SandBoxLoadout = JsonConvert.SerializeObject(profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"]);

                        // emotes
                        if (Body.itemToSlot == "")
                        {
                            SandBoxLoadout.attributes.locker_slots_data.slots[Body.category.ToLower()].items[Body.slotIndex] = "";
                            profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"] = SandBoxLoadout;
                        }
                        else
                        {
                            SandBoxLoadout.attributes.locker_slots_data.slots[Body.category.ToLower()].items[Body.slotIndex] = Body.itemToSlot.ToLower();
                            profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"] = SandBoxLoadout;
                        }
                    }
                }
                else
                {
                    dynamic SandBoxLoadout = JsonConvert.SerializeObject(profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"]);

                    if (Body.itemToSlot == "")
                    {
                        SandBoxLoadout.attributes.locker_slots_data.slots[Body.category.ToLower()].items = new List<string> { "" };
                        profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"] = SandBoxLoadout;
                    }
                    else
                    {
                        SandBoxLoadout.attributes.locker_slots_data.slots[Body.category.ToLower()].items = new List<string> {
                            Body.itemToSlot.ToLower()
                        };
                        profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"] = SandBoxLoadout;
                    }
                }
                profileCacheEntry.AccountData.athena.RVN += 1;
                profileCacheEntry.AccountData.athena.CommandRevision += 1;
                //UpdatedData.Add($"athena.CommandRevision", profileCacheEntry.AccountData.athena.CommandRevision + 1);
               // await Handlers.UpdateOne<Account>("accountId", AccountId, UpdatedData);
                List<dynamic> BigA = new List<dynamic>();
                if (Season.SeasonFull >= 12.20)
                {
                    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    BigA = test.profileChanges;
                }
                else
                {
                    BigA = new List<object>()
                    {
                        new
                        {
                            changeType = "statModified",
                            name = $"favorite_{Body.category.ToLower()}",
                            value = Body.itemToSlot
                        }
                    };
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN + 1,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev + 1,
                    profileChanges = BigA,
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision + 1,
                    serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                    responseVersion = 1
                };
            }

            return new Mcp();
        }
    }
}
