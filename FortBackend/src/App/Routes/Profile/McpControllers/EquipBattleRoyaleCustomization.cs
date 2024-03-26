using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using MongoDB.Driver.Core.Servers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FortBackend.src.App.Utilities.Helpers.Middleware;


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
                int GrabPlacement = GrabPlacement = profileCacheEntry.AccountData.athena.Items.SelectMany((item, index) => new List<(Dictionary<string, object> Item, int Index)> { (Item: item, Index: index) })
                .TakeWhile(pair => !pair.Item.ContainsKey("sandbox_loadout")).Count();
                List<object> ProfileChanges = new List<object>();
                if (Body.slotName == "ItemWrap" || Body.slotName == "Dance")
                {
                    // emote, wraps soon upcoming
                    if (Body.indexWithinSlot == -1)
                    {
                        if (Body.slotName == "Dance")
                        {
                            return new Mcp();
                        }
                        var SandBoxLoadout = JsonConvert.DeserializeObject<SandboxLoadout>(JsonConvert.SerializeObject(profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"]));

                        if (SandBoxLoadout != null)
                        {
                            var ItemsCount = SandBoxLoadout.attributes.locker_slots_data.slots.itemwrap.items.Count();
                            string[] ReplacedItems = Enumerable.Repeat(Body.itemToSlot.ToLower(), ItemsCount).ToArray();

                            profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"] = SandBoxLoadout;
                            //UpdatedData.Add($"athena.items.{GrabPlacement}.sandbox_loadout.attributes.locker_slots_data.slots.{Body.slotName.ToLower()}.items", ReplacedItems);
                            ProfileChanges.Add(new List<object>()
                            {
                                new
                                {
                                    changeType = "statModified",
                                    name = $"favorite_{Body.slotName.ToLower()}",
                                    value = ReplacedItems
                                }
                            });
                        }
                    }
                    else
                    {
                        var SandBoxLoadout = JsonConvert.DeserializeObject<SandboxLoadout>(JsonConvert.SerializeObject(profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"]));

                        // emotes
                        if (SandBoxLoadout != null)
                        {
                            if (Body.itemToSlot == "")
                            {
                            

                                SandBoxLoadout.attributes.locker_slots_data.slots.dance.items[Body.indexWithinSlot] = "";
                                ProfileChanges.Add(new List<object>()
                                {
                                    new
                                    {
                                        changeType = "statModified",
                                        name = $"favorite_{Body.slotName.ToLower()}",
                                        value =  SandBoxLoadout.attributes.locker_slots_data.slots.dance.items
                                    }
                                });
                            }else
                            {
                                SandBoxLoadout.attributes.locker_slots_data.slots.dance.items[Body.indexWithinSlot] = Body.itemToSlot.ToLower();
                                ProfileChanges.Add(new List<object>()
                                {
                                    new
                                    {
                                        changeType = "statModified",
                                        name = $"favorite_{Body.slotName.ToLower()}",
                                        value =  SandBoxLoadout.attributes.locker_slots_data.slots.dance.items
                                    }
                                });
                            }

                            profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"] = SandBoxLoadout;

                        }

                    }
                }
                else
                {
                    var SandBoxLoadout = JsonConvert.DeserializeObject<SandboxLoadout>(JsonConvert.SerializeObject(profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"]));
                    if (SandBoxLoadout != null)
                    {
                        var slotName = Body.slotName.ToLower();
                        var itemToSlot = Body.itemToSlot.ToLower() ?? "";
                        SandBoxLoadout.attributes.locker_slots_data.slots.GetSlotName(slotName).items[0] = itemToSlot;
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
                    profileCacheEntry.AccountData.athena.Items[GrabPlacement]["sandbox_loadout"] = SandBoxLoadout;
                }

                if (ProfileChanges.Count > 0)
                {
                    profileCacheEntry.AccountData.athena.RVN += 1;
                    profileCacheEntry.AccountData.athena.CommandRevision += 1;
                    //UpdatedData.Add($"athena.RVN", profileCacheEntry.AccountData.athena.RVN);
                    //UpdatedData.Add($"athena.CommandRevision", profileCacheEntry.AccountData.athena.CommandRevision);

                    //await Handlers.UpdateOne<Account>("accountId", AccountId, UpdatedData);
                }
             
                if (BaseRev != RVN)
                {
                    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    ProfileChanges = test.profileChanges;
                } 


                //var test2 = 
                //if(AccountDataParsed.athena.RVN)

                //AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, AccountDataParsed);
                Console.WriteLine(JsonConvert.SerializeObject(new
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev,
                    profileChanges = ProfileChanges,
                    //new List<object>()
                    //{
                    //    //new
                    //    //{
                    //    //    changeType = "statModified",
                    //    //    name = $"favorite_{Body.category.ToLower()}",
                    //    //    value = Body.itemToSlot
                    //    //}
                    //},
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
                    //new List<object>()
                    //{
                    //    //new
                    //    //{
                    //    //    changeType = "statModified",
                    //    //    name = $"favorite_{Body.category.ToLower()}",
                    //    //    value = Body.itemToSlot
                    //    //}
                    //},
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                    responseVersion = 1
                };
            }

            return new Mcp();
        }
    }
}
