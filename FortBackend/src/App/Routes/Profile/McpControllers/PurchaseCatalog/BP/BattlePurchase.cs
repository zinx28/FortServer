using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.App.Utilities.Quests;
using FortBackend.src.XMPP.Data;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.XMPP;
using FortLibrary;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using FortBackend.src.App.Utilities;
using FortLibrary.MongoDB.Module;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using Newtonsoft.Json;

namespace FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog.BP
{
    public class BattlePurchase
    {
        public static async Task<(SeasonClass seasonObject, CommonCoreItem currencyItem, List<object> ApplyProfileChanges, List<object> MultiUpdates, ProfileCacheEntry profileCacheEntry)> Init(SeasonClass seasonObject, VersionClass Season, CommonCoreItem currencyItem, int Price, List<object> ApplyProfileChanges, List<object> MultiUpdates, ProfileCacheEntry profileCacheEntry, bool NeedItems)
        {
           // bool NeedItems = true;
            //int BookLevelOG = seasonObject.BookLevel;
            List<Battlepass> FreeTier = BattlepassManager.FreeBattlePassItems.FirstOrDefault(e => e.Key == Season.Season).Value;

            if (FreeTier != null)
            {
                if (FreeTier.Count > 0)
                {
                    if (Season.Season > 1)
                    {
                        List<Battlepass> PaidTier = BattlepassManager.PaidBattlePassItems.FirstOrDefault(e => e.Key == Season.Season).Value;

                        if (PaidTier != null)
                        {
                            if (PaidTier.Count > 0)
                            {

                                currencyItem.quantity -= Price;


                                seasonObject.BookPurchased = true;

                                ApplyProfileChanges.Add(new
                                {
                                    changeType = "statModified",
                                    name = "book_purchased",
                                    value = true
                                });

                                if (WeeklyQuestManager.WeeklyQuestsSeasonAboveDictionary.TryGetValue($"Season{seasonObject.SeasonNumber}", out List<WeeklyQuestsJson> WeeklyQuestsArray))
                                {
                                    if (WeeklyQuestsArray.Count > 0)
                                    {
                                        List<string> ResponseIgIdrk = new List<string>();
                                        var ResponseId = "";
                                        foreach (var kvp in WeeklyQuestsArray)
                                        {
                                            ResponseIgIdrk.Add($"ChallengeBundle:{kvp.BundleId}");
                                            ResponseId = $"ChallengeBundleSchedule:{kvp.BundleSchedule}";

                                            List<string> QuestTestResponse = new List<string>();
                                            foreach (var Bundles in kvp.BundlesObject)
                                            {
                                                if (Bundles.quest_data.ExtraQuests) continue;

                                                QuestTestResponse.Add(Bundles.templateId);

                                                DailyQuestsData QuestData = seasonObject.Quests.FirstOrDefault(e => e.Key == Bundles.templateId).Value;
                                                if (QuestData == null)
                                                {
                                                    List<DailyQuestsObjectiveStates> QuestObjectStats = new List<DailyQuestsObjectiveStates>();

                                                    foreach (WeeklyObjectsObjectives ObjectiveItems in Bundles.Objectives)
                                                    {
                                                        //season_xp_gained

                                                        QuestObjectStats.Add(new DailyQuestsObjectiveStates
                                                        {
                                                            Name = $"completion_{ObjectiveItems.BackendName}",
                                                            Value = 0,
                                                            MaxValue = ObjectiveItems.Count
                                                        });
                                                    }

                                                    seasonObject.Quests.Add($"{Bundles.templateId}", new DailyQuestsData
                                                    {
                                                        templateId = $"{Bundles.templateId}",
                                                        attributes = new DailyQuestsDataDB
                                                        {
                                                            challenge_bundle_id = $"ChallengeBundle:{kvp.BundleId}",
                                                            sent_new_notification = false,
                                                            ObjectiveState = QuestObjectStats
                                                        },
                                                        quantity = 1
                                                    });


                                                    var ItemObjectResponse = new
                                                    {
                                                        templateId = $"{Bundles.templateId}",
                                                        attributes = new Dictionary<string, object>
                                                        {
                                                            { "creation_time", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                            { "level", -1 },
                                                            { "item_seen", false },
                                                            { "playlists", new List<object>() },
                                                            { "sent_new_notification", true },
                                                            { "challenge_bundle_id", $"ChallengeBundle:{kvp.BundleId}" },
                                                            { "xp_reward_scalar", 1 },
                                                            { "challenge_linked_quest_given", "" },
                                                            { "quest_pool", "" },
                                                            { "quest_state", "Active" },
                                                            { "bucket", "" },
                                                            { "last_state_change_time", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                            { "challenge_linked_quest_parent", "" },
                                                            { "max_level_bonus", 0 },
                                                            { "xp", 0 },
                                                            { "quest_rarity", "uncommon" },
                                                            { "favorite", false },
                                                            // { $"completion_{dailyQuests.Properties.Objectives[0].BackendName}", 0 }
                                                        },
                                                        quantity = 1
                                                    };

                                                    foreach (DailyQuestsObjectiveStates yklist in QuestObjectStats)
                                                    {
                                                        ItemObjectResponse.attributes.Add(yklist.Name, yklist.Value);
                                                    }

                                                    ApplyProfileChanges.Add(new MultiUpdateClass
                                                    {
                                                        changeType = "itemAdded",
                                                        itemId = $"{Bundles.templateId}",
                                                        item = ItemObjectResponse
                                                    });
                                                }
                                            }

                                            var AthenaItemChallengeBundle = new AthenaItemDynamic
                                            {
                                                templateId = $"ChallengeBundle:{kvp.BundleId}",
                                                attributes = new Dictionary<string, object>
                                                {
                                                    { "has_unlock_by_completion", false },
                                                    { "num_quests_completed", 0 },
                                                    { "level", 0 },
                                                    { "grantedquestinstanceids", QuestTestResponse.ToArray() },
                                                    { "item_seen",  true },
                                                    { "max_allowed_bundle_level", 0 },
                                                    { "num_granted_bundle_quests", QuestTestResponse.Count() },
                                                    { "max_level_bonus", 0 },
                                                    { "challenge_bundle_schedule_id", ResponseId },
                                                    { "num_progress_quests_completed", 0 },
                                                    { "xp", 0 },
                                                    { "favorite", false }
                                                },
                                                quantity = 1,
                                            };

                                            ApplyProfileChanges.Add(new
                                            {
                                                changeType = "itemRemoved",
                                                itemId = $"ChallengeBundle:{kvp.BundleId}"
                                            });

                                            ApplyProfileChanges.Add(new MultiUpdateClass
                                            {
                                                changeType = "itemAdded",
                                                itemId = $"ChallengeBundle:{kvp.BundleId}",
                                                item = AthenaItemChallengeBundle
                                            });
                                        }

                                        var ItemTestObjectResponse = new
                                        {
                                            templateId = ResponseId,
                                            attributes = new Dictionary<string, object>
                                                                {
                                                                    { "unlock_epoch", DateTime.MinValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                                    { "max_level_bonus", 0 },
                                                                    { "level", 0 },
                                                                    { "item_seen", true },
                                                                    { "xp", 0 },
                                                                    { "favorite", false },
                                                                    { "granted_bundles", ResponseIgIdrk.ToArray() }
                                                                },
                                            quantity = 1,
                                        };

                                        MultiUpdates.Add(new
                                        {
                                            changeType = "itemRemoved",
                                            itemId = ResponseId,
                                        });


                                        MultiUpdates.Add(new MultiUpdateClass
                                        {
                                            changeType = "itemAdded",
                                            itemId = ResponseId,
                                            item = ItemTestObjectResponse
                                        });
                                    }
                                }




                                List<NotificationsItemsClassOG> ItemsGivenToUser = new List<NotificationsItemsClassOG>();
                                foreach (Battlepass BattlePass in FreeTier)
                                {
                                    if (!NeedItems) break;
                                    //We don't need this check on purchase as we "WANT" the user to get them items
                                    //if (BookLevelOG <= BattlePass.Level) continue;
                                    if (BattlePass.Level > seasonObject.BookLevel) break;

                                    (profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems, ItemsGivenToUser) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems, ItemsGivenToUser);
                                }
                                foreach (Battlepass BattlePass in PaidTier)
                                {
                                    if (!NeedItems) break;
                                    //if (BookLevelOG <= BattlePass.Level) continue;
                                    if (BattlePass.Level > seasonObject.BookLevel) break;


                                    (profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems, ItemsGivenToUser) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems, ItemsGivenToUser);
                                }

                                // after to get the correct price
                                MultiUpdates.Add(new
                                {
                                    changeType = "itemQuantityChanged",
                                    itemId = "Currency",
                                    quantity = currencyItem.quantity
                                });
                                /*
                                 *   NewItemsGiven.Add(new Dictionary<string, object>
                                    {
                                        { "itemType", FreeRewards["TemplateId"].ToString() },
                                        { "itemGuid", FreeRewards["TemplateId"].ToString() },
                                        { "quantity", int.Parse(FreeRewards["Quantity"].ToString() ?? "1") }
                                    });
                                */
                                var RandomOfferId = Guid.NewGuid().ToString();
                                MultiUpdates.Add(new ApplyProfileChangesClassV2
                                {
                                    changeType = "itemAdded",
                                    itemId = RandomOfferId,
                                    item = new
                                    {
                                        templateId = Season.Season >= 5 ? "GiftBox:gb_battlepasspurchased" : "GiftBox:gb_battlepass",
                                        attributes = new
                                        {
                                            max_level_bonus = 0,
                                            fromAccountId = "",
                                            lootList = ItemsGivenToUser
                                        },
                                        quantity = 1
                                    }
                                });

                                profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                                {
                                    templateId = Season.Season >= 5 ? "GiftBox:gb_battlepasspurchased" : "GiftBox:gb_battlepass",
                                    attributes = new GiftCommonCoreItemAttributes
                                    {
                                        lootList = ItemsGivenToUser
                                    },
                                    quantity = 1
                                });

                                if (!string.IsNullOrEmpty(profileCacheEntry.AccountId))
                                {
                                    Clients Client = GlobalData.Clients.FirstOrDefault(client => client.accountId == profileCacheEntry.AccountId)!;

                                    if (Client != null)
                                    {
                                        string xmlMessage;
                                        byte[] buffer;
                                        WebSocket webSocket = Client.Game_Client;
                                        Logger.PlainLog(webSocket.State);
                                        if (webSocket != null && webSocket.State == WebSocketState.Open)
                                        {
                                            XNamespace clientNs = "jabber:client";

                                            var message = new XElement(clientNs + "message",
                                              new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                              new XAttribute("to", profileCacheEntry.AccountId),
                                              new XElement(clientNs + "body", JsonConvert.SerializeObject(new
                                              {
                                                  payload = new { },
                                                  type = "com.epicgames.gift.received",
                                                  timestamp = DateTime.UtcNow.ToString("o")
                                              }))
                                            );

                                            xmlMessage = message.ToString();
                                            buffer = Encoding.UTF8.GetBytes(xmlMessage);

                                            Logger.PlainLog(xmlMessage);

                                            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                        }

                                    }

                                }
                            }
                            else
                            {
                                Logger.Error("PaidTier file is null [] ? battlepass tiering disabled");
                            }
                        }
                        else
                        {
                            Logger.Log("Unsupported season");
                        }
                    }
                }
                else
                {
                    Logger.Error("FreeTier file is null [] ? battlepass tiering disabled");
                }
            }
            else
            {
                Logger.Error($"This season is *NOT* supported ~ {Season.Season}", "ClientQuestLogin");
            }

            return (seasonObject, currencyItem, ApplyProfileChanges, MultiUpdates, profileCacheEntry);
        }
    }
}
