using Discord;
using FortBackend.src.App.Routes.APIS.Profile.McpControllers.AthenaResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Newtonsoft.Json;

namespace FortBackend.src.App.Routes.APIS.Profile.McpControllers
{
    public class SetCosmeticLockerSlot
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, int Season, int RVN, Account AccountDataParsed, SetCosmeticLockerSlotRequest Body)
        {

            Console.WriteLine(Body);
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                Dictionary<string, object> UpdatedData = new Dictionary<string, object>();
                int BaseRev = AccountDataParsed.athena.RVN;
                if (Body.category == "ItemWrap" || Body.category == "Dance")
                {
                    // emote, wraps soon upcoming
                    if (Body.slotIndex == -1)
                    {
                        if (Body.category == "Dance")
                        {
                            return new Mcp();
                        }
                        var SandBoxLoadout = JsonConvert.DeserializeObject<SandboxLoadout>(JsonConvert.SerializeObject(AccountDataParsed.athena.Items[0][AccountDataParsed.athena.last_applied_loadout]));
                     
                        if(SandBoxLoadout != null)
                        {
                            var ItemsCount = SandBoxLoadout.attributes.locker_slots_data.slots.itemwrap.items.Count();
                            string[] ReplacedItems = Enumerable.Repeat(Body.itemToSlot.ToLower(), ItemsCount).ToArray();

                            UpdatedData.Add($"athena.items.0.{AccountDataParsed.athena.last_applied_loadout}.attributes.locker_slots_data.slots.{Body.category.ToLower()}.items", ReplacedItems);
                        }
                    }
                    else
                    {
                        // emotes
                        if (Body.itemToSlot == "")
                        {
                            UpdatedData.Add($"athena.items.0.{AccountDataParsed.athena.last_applied_loadout}.attributes.locker_slots_data.slots.{Body.category.ToLower()}.items.{Body.slotIndex}", "");
                        }else
                        {
                            UpdatedData.Add($"athena.items.0.{AccountDataParsed.athena.last_applied_loadout}.attributes.locker_slots_data.slots.{Body.category.ToLower()}.items.{Body.slotIndex}", Body.itemToSlot.ToLower());
                        }
                    }
                }else
                {
                    if(Body.itemToSlot == "")
                    {
                        UpdatedData.Add($"athena.items.0.{AccountDataParsed.athena.last_applied_loadout}.attributes.locker_slots_data.slots.{Body.category.ToLower()}.items", new List<string> {
                            ""
                        });
                    }
                    else
                    {
                        UpdatedData.Add($"athena.items.0.{AccountDataParsed.athena.last_applied_loadout}.attributes.locker_slots_data.slots.{Body.category.ToLower()}.items", new List<string> { 
                            Body.itemToSlot.ToLower() 
                        });
                    }
                }

                UpdatedData.Add($"athena.RVN", AccountDataParsed.athena.RVN + 1);
                UpdatedData.Add($"athena.CommandRevision", AccountDataParsed.athena.CommandRevision + 1);
                await Handlers.UpdateOne<Account>("accountId", AccountId, UpdatedData);

                return new Mcp()
                {
                    profileRevision = AccountDataParsed.athena.RVN + 1,
                    profileId = "athena",
                    profileChangesBaseRevision = BaseRev + 1,
                    profileChanges = new List<object>()
                    {
                        new
                        {
                            changeType = "statModified",
                            name = $"favorite_{Body.category.ToLower()}",
                            value = Body.itemToSlot
                        }
                    },
                    profileCommandRevision = AccountDataParsed.athena.CommandRevision + 1,
                    serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                    responseVersion = 1
                };
            }

            return new Mcp();
        }
    }
}
