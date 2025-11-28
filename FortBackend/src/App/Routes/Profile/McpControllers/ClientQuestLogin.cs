using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Quests;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Quests;
using Newtonsoft.Json;
using System.Collections.Generic;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary;
using System.Net.Http.Json;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.App.Utilities.Constants;
using FortLibrary.EpicResponses.Profile.Query;
using MongoDB.Bson.IO;
using FortLibrary.Shop;
using Discord;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using FortBackend.src.App.Utilities.Helpers.QuestsManagement;
using FortBackend.src.App.Utilities.Saved;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class ClientQuestLogin
    {
        // This IS TEMP code ~ i'll rewrite mcp at some point (equiping is fine)
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry)
        {
            string currentDate = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                // adds stats shocked
                 
                //pc_m0_p2 ~ solos
                //pc_m0_p10 ~ duos
                //pc_m0_p9 ~ squads
                foreach (var item in UpdateLeaderBoard.GetStatNames())
                {
                    if (!profileCacheEntry.StatsData.stats.Keys.Contains(item))
                    {
                        profileCacheEntry.StatsData.stats.Add(item, 0);
                    }
                }

                // Daily Quests WIP

                if (profileCacheEntry.AccountData.commoncore.Seasons.Any(x => x.SeasonNumber == Season.Season))
                {
                    // Response Data ~ DONT CHANGE
                    List<object> MultiUpdates = new List<object>();
                    List<object> MultiUpdatesForCommonCore = new List<object>();
                    int BaseRev = profileCacheEntry.AccountData.athena.RVN;

                    if (Season.Season == 0)
                    {
                        if (BaseRev != RVN)
                        {
                            Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                            MultiUpdates = test.profileChanges;
                        }

                        return new Mcp
                        {
                            profileRevision = profileCacheEntry.AccountData.athena.RVN,
                            profileId = ProfileId,
                            profileChangesBaseRevision = BaseRev,
                            profileChanges = MultiUpdates,
                            profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                            serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            responseVersion = 1
                        };
                    }

                    
                    // Temp data
                    //int AddedXP = 0; // xp added to the users account
                    //int AddedLevel = 0; // level added to the users account

                    // Season Data
                    SeasonClass FoundSeason = profileCacheEntry.AccountData.commoncore?.Seasons.FirstOrDefault(x => x.SeasonNumber == Season.Season)!;

                        

                    if(FoundSeason.DailyQuests.Interval != currentDate && !(FoundSeason.DailyQuests.Daily_Quests.Count > 3))
                    {
                        FoundSeason.DailyQuests.Interval = currentDate;
                        FoundSeason.DailyQuests.Rerolls += 1; // give 1 reroll everyday
                             
                        MultiUpdates.Add(new MultiUpdateClassV2
                        {
                            changeType = "statModified",
                            name = "quest_manager",
                            value = new
                            {
                                dailyLoginInterval = currentDate,
                                dailyQuestRerolls = FoundSeason.DailyQuests.Rerolls
                            }
                        });

                        if (!(FoundSeason.DailyQuests.Daily_Quests.Count > 3) || FoundSeason.SeasonNumber == 13 && !(FoundSeason.DailyQuests.Daily_Quests.Count > 5))
                        {
                            var DailyCount = FoundSeason.SeasonNumber != 13 ? 3 - FoundSeason.DailyQuests.Daily_Quests.Count : 5 - FoundSeason.DailyQuests.Daily_Quests.Count;
                               
                            for (int i = 0; i < DailyCount; i++)
                            {
                                try
                                {
                                    DailyQuestsJson dailyQuests = await DailyQuestsManager.GrabRandomQuest(FoundSeason);

                                    if (!string.IsNullOrEmpty(dailyQuests.Name))
                                    {
                                        if(dailyQuests.Properties.Objectives.Count > 1)
                                        {
                                            Logger.Error("FEATURE NOT IMPLEMENTED");
                                        }
                                        else
                                        {
                                            DailyQuestsData dailyQuestsData = new DailyQuestsData
                                            {
                                                templateId = $"Quest:{dailyQuests.Name}",
                                                attributes = new DailyQuestsDataDB
                                                {
                                                    sent_new_notification = false,
                                                    ObjectiveState = new List<DailyQuestsObjectiveStates>
                                                    {
                                                        new DailyQuestsObjectiveStates
                                                        {
                                                            Name = $"completion_{dailyQuests.Properties.Objectives[0].BackendName}",
                                                            Value = 0
                                                        }
                                                    }
                                                },
                                                quantity = 1
                                            };

                                            // so skunked but should wokr
                                            MultiUpdates.Add(new MultiUpdateClass
                                            {
                                                changeType = "itemAdded",
                                                itemId = dailyQuests.Name,
                                                item = new
                                                {
                                                    templateId = $"Quest:{dailyQuests.Name}",
                                                    attributes = new Dictionary<string, object>
                                                    {
                                                        { "creation_time", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                        { "level", -1 },
                                                        { "item_seen", false },
                                                        { "playlists", new List<object>() },
                                                        { "sent_new_notification", true },
                                                        { "challenge_bundle_id", "" },
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
                                                        { $"completion_{dailyQuests.Properties.Objectives[0].BackendName}", 0 }
                                                    },
                                                    quantity = 1
                                                }
                                            });

                                            FoundSeason.DailyQuests.Daily_Quests.Add(dailyQuests.Name, dailyQuestsData);
                                        }
                                            
                                    }
                                    else
                                    {
                                        Logger.Error("GRABBED EMPTY STUFF FOR DAUKY QUESTS?", "DAILY QUESTS!");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex.Message, "DAILY QUESTS!");
                                }
                            }
                            // 
                        } // else how?
                    }

                    // CLAIMING QUEST SYSTEM
                    // Theres a whole different endpoint for claiming but in here we remove and give xp
                    // since i only worked on season 1 quests hasn't been implemented on other seasons until i work on season 2

                    foreach (var item in FoundSeason.DailyQuests.Daily_Quests)
                    {
                        if(item.Value != null) // shoudn't ever be null
                        {
                            DailyQuestsData DailyQuestsObject = item.Value;
                            if (DailyQuestsObject == null) continue;

                            if(DailyQuestsObject.attributes.quest_state == "Claimed")
                            {
                                // AddedXP += DailyQuestsObject.attributes.
                                DailyQuestsJson dailyQuestsJson = DailyQuestsManager.ReturnQuestInfo(item.Key, Season.Season);
                                if (string.IsNullOrEmpty(dailyQuestsJson.Name))
                                {
                                    FoundSeason.BookXP += dailyQuestsJson.Properties.SeasonXP;
                                    FoundSeason.DailyQuests.Daily_Quests.Remove(item.Key); // removes the quest
                                }
                            }
                        }
                    }

                    // END OF CLAIMING QUEST SYTEM

                    // START OF BATTLE PASS QUESTS SYSTEM

                    // Until i work on a season that changes this (season 10 omg) i need to redo this
                    (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry) = await QuestsDealer.Init(FoundSeason, MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry);

                    // END OF BATTLE PASS QUESTS SYSTEM

                    // START OF SPECIAL ITEMS
                    // Granted per second, you may want to put skins of other things in this
                    if (FoundSeason.special_items == null)
                        FoundSeason.special_items = new();

                    List<string> SeasonSpecialIg = BattlepassManager.SeasonSpecialItems.FirstOrDefault(e => e.Key == FoundSeason.SeasonNumber).Value;

                    if (SeasonSpecialIg != null && SeasonSpecialIg.Count > 0)
                    {
                        foreach (var item in SeasonSpecialIg)
                        {
                            if (!FoundSeason.special_items.ContainsKey(item))
                            {
                                FoundSeason.special_items.Add(item, new AthenaItem
                                {
                                    templateId = item,
                                    quantity = 1
                                });

                                MultiUpdates.Add(new MultiUpdateClass
                                {
                                    changeType = "itemAdded",
                                    itemId = item,
                                    item = new AthenaItem
                                    {
                                        templateId = item,
                                        attributes = new AthenaItemAttributes
                                        {
                                            item_seen = false,
                                        },
                                        quantity = 1
                                    }
                                });
                            }
                        }
                    }

                    // END OF SPECIAL ITEMS

                    //

                    // LEVEL SYSTEM & XP

                    List<SeasonXP> SeasonXpIg = BattlepassManager.SeasonBattlePassXPItems.FirstOrDefault(e => e.Key == FoundSeason.SeasonNumber).Value;
                    // unsupported seasons will not go though.. so it doesn't break stuff
                    // I will try to add it for most seasons when i'm not doing other things
                    if (SeasonXpIg != null)
                    {
                        // Levels Up & Grant Items Live (if your proper) ~ i'm not
                        (profileCacheEntry, MultiUpdates, MultiUpdatesForCommonCore) = await BattlePassLevelUp.Init(Season, FoundSeason, profileCacheEntry, MultiUpdates, MultiUpdatesForCommonCore);

                        (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry) = await QuestClaimer.Init(SeasonXpIg, FoundSeason, MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry);

                        // END OF BATTLE PASS SYSTEM

                        /// TO-DO ONLY USE THIS IF THE STAT IS CHANGING

                        MultiUpdates.Add(new
                        {
                            changeType = "statModified",
                            name = "level",
                            value = FoundSeason.Level
                        });

                        MultiUpdates.Add(new
                        {
                            changeType = "statModified",
                            name = "book_level",
                            value = FoundSeason.BookLevel
                        });

                        //book_xp


                        MultiUpdates.Add(new
                        {
                            changeType = "statModified",
                            name = "book_xp",
                            value = FoundSeason.BookXP
                        });

                        MultiUpdates.Add(new
                        {
                            changeType = "statModified",
                            name = "xp",
                            value = FoundSeason.SeasonXP
                        });

                       
                    }

                    // END OF LEVEL SYTEM & XP

                    // START OF AUTO MFA CLAIM

                    if (Saved.DeserializeGameConfig.MfaClaim)
                    {
                        if (!profileCacheEntry.AccountData.commoncore.mfa_enabled)
                        {
                            profileCacheEntry.AccountData.commoncore.mfa_enabled = true;

                            if (!profileCacheEntry.AccountData.athena.Items.ContainsKey("AthenaDance:EID_BoogieDown"))
                            {

                                profileCacheEntry.AccountData.athena.Items.Add("AthenaDance:EID_BoogieDown", new AthenaItem
                                {
                                    templateId = "AthenaDance:EID_BoogieDown",
                                    attributes = new AthenaItemAttributes
                                    {
                                        item_seen = false
                                    },
                                    quantity = 1,
                                });

                                MultiUpdates.Add(new MultiUpdateClass
                                {
                                    changeType = "itemAdded",
                                    itemId = "AthenaDance:EID_BoogieDown",
                                    item = new AthenaItem
                                    {
                                        templateId = "AthenaDance:EID_BoogieDown",
                                        attributes = new AthenaItemAttributes
                                        {
                                            item_seen = false,
                                        },
                                        quantity = 1
                                    }
                                });

                                var RandomOfferId = Guid.NewGuid().ToString();

                                profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                                {
                                    templateId = "GiftBox:gb_mfareward",
                                    attributes = new GiftCommonCoreItemAttributes
                                    {
                                        lootList = new List<NotificationsItemsClassOG>
                                    {
                                        new NotificationsItemsClassOG
                                        {
                                            itemType = "AthenaDance:EID_BoogieDown",
                                            itemGuid = "AthenaDance:EID_BoogieDown",
                                            quantity = 1
                                        }
                                    }
                                    },
                                    quantity = 1
                                });

                                MultiUpdatesForCommonCore.Add(new ApplyProfileChangesClassV2
                                {
                                    changeType = "itemAdded",
                                    itemId = RandomOfferId,
                                    item = new
                                    {
                                        templateId = "GiftBox:gb_mfareward",
                                        attributes = new
                                        {
                                            max_level_bonus = 0,
                                            fromAccountId = "",
                                            lootList = new List<NotificationsItemsClassOG>
                                            {
                                                new NotificationsItemsClassOG
                                                {
                                                    itemType = "AthenaDance:EID_BoogieDown",
                                                    itemGuid = "AthenaDance:EID_BoogieDown",
                                                    quantity = 1
                                                }
                                            }
                                        },
                                        quantity = 1
                                    }
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
                                              new XElement(clientNs + "body", Newtonsoft.Json.JsonConvert.SerializeObject(new
                                              {
                                                  payload = new { },
                                                  type = "com.epicgames.gift.received",
                                                  timestamp = DateTime.UtcNow.ToString("o")
                                              }))
                                            );

                                            xmlMessage = message.ToString();
                                            buffer = Encoding.UTF8.GetBytes(xmlMessage);

                                            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                        }

                                    }
                                }

                                MultiUpdatesForCommonCore.Add(new
                                {
                                    changeType = "statModified",
                                    name = "mfa_enabled",
                                    value = true
                                });
                            }
                        }
                    }

                    // END OF AUTO MFA CLAIM

                    if (MultiUpdates.Count > 0)
                    {
                        profileCacheEntry.LastUpdated = DateTime.UtcNow;
                        profileCacheEntry.AccountData.athena.RVN += 1;
                        profileCacheEntry.AccountData.athena.CommandRevision += 1;
                    }

                    List<object> MultiCommonCoreUpdate = new List<object>();
                    if(MultiUpdatesForCommonCore.Count > 0)
                    {
                        var BeofreUpdate = profileCacheEntry.AccountData.commoncore.RVN;
                        profileCacheEntry.LastUpdated = DateTime.UtcNow;
                        profileCacheEntry.AccountData.commoncore.RVN += 1;
                        profileCacheEntry.AccountData.commoncore.CommandRevision += 1;

                        MultiCommonCoreUpdate.Add(new
                        {
                            profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                            profileId = "common_core",
                            profileChangesBaseRevision = BeofreUpdate,
                            profileChanges = MultiUpdatesForCommonCore,
                            profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                        });
                    }


                    if (BaseRev != RVN)
                    {
                        Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                        MultiUpdates = test.profileChanges;
                    }

                    //Console.WriteLine(JsonConvert.SerializeObject(new Mcp
                    //{
                    //    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    //    profileId = ProfileId,
                    //    profileChangesBaseRevision = BaseRev,
                    //    profileChanges = MultiUpdates,
                    //    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    //    serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                    //    responseVersion = 1
                    //}));

                    //string mcpJson = Newtonsoft.Json.JsonConvert.SerializeObject(new Mcp
                    //{
                    //    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    //    profileId = ProfileId,
                    //    profileChangesBaseRevision = BaseRev,
                    //    profileChanges = MultiUpdates,
                    //    multiUpdate = MultiCommonCoreUpdate,
                    //    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    //    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    //    responseVersion = 1
                    //}, Formatting.Indented);
                    //Console.WriteLine(mcpJson);


                    return new Mcp
                    {
                        profileRevision = profileCacheEntry.AccountData.athena.RVN,
                        profileId = ProfileId,
                        profileChangesBaseRevision = BaseRev,
                        profileChanges = MultiUpdates,
                        multiUpdate = MultiUpdatesForCommonCore,
                        profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                        serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        responseVersion = 1
                    };
                }
                //var DailyQuests =
                Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);

                return response;
                ////Console.WriteLine("fas");
                //List<AthenaItem> contentconfig = JsonConvert.DeserializeObject<List<AthenaItem>>(jsonData); //dynamicbackgrounds.news
                ////Console.WriteLine("GR");

                //ProfileChange test1 = response.profileChanges[0] as ProfileChange;
                //foreach (AthenaItem test in contentconfig)
                //{
                //    //Console.WriteLine("TET");
                //    test1.Profile.items.Add(test.templateId, test);
                //}
                //return response;
            }
            else
            {
                Logger.Error("ClientQuestLogin might not function well");
            }
       

            return new Mcp();
        }
    }
}
