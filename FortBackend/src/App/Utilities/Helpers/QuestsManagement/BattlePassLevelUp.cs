using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.XMPP.Data;
using FortBackend.src.XMPP.SERVER.Send;
using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB.Module;
using FortLibrary.XMPP;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Utilities.Helpers.QuestsManagement
{
    public class BattlePassLevelUp
    {
        public static async Task<(ProfileCacheEntry profileCacheEntry, List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore)> Init(VersionClass Season, SeasonClass FoundSeason, ProfileCacheEntry profileCacheEntry, List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore)
        {
            int BookLevelOG = FoundSeason.BookLevel;
            bool NeedItems = false;

            (FoundSeason, NeedItems) = await LevelUpdater.Init(Season.Season, FoundSeason, NeedItems);

            var currencyItem = profileCacheEntry.AccountData.commoncore.Items["Currency"];
            if (currencyItem != null)
            {


                // BATTLE PASS SYSTEM

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
                                    List<NotificationsItemsClassOG> unlessfunc = new List<NotificationsItemsClassOG>();
                                    foreach (var BattlePass in FreeTier)
                                    {
                                        if (!NeedItems) break;
                                        if (BattlePass.Level <= BookLevelOG) continue;
                                        if (BattlePass.Level > FoundSeason.BookLevel) break;

                                        // List<NotificationsItemsClassOG> unlessfunc;
                                        (profileCacheEntry, FoundSeason, MultiUpdates, currencyItem, NeedItems, unlessfunc) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, FoundSeason, MultiUpdates, currencyItem, NeedItems, unlessfunc);
                                    }

                                    foreach (var BattlePass in PaidTier)
                                    {
                                        if (!NeedItems) break;
                                        if (BattlePass.Level <= BookLevelOG) continue;
                                        if (BattlePass.Level > FoundSeason.BookLevel) break;

                                        // List<NotificationsItemsClassOG> unlessfunc;
                                        (profileCacheEntry, FoundSeason, MultiUpdates, currencyItem, NeedItems, unlessfunc) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, FoundSeason, MultiUpdates, currencyItem, NeedItems, unlessfunc);
                                    }


                                    if (NeedItems)
                                    {
                                        var RandomOfferId = Guid.NewGuid().ToString();

                                        profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                                        {
                                            templateId = "GiftBox:gb_battlepass",
                                            attributes = new GiftCommonCoreItemAttributes
                                            {
                                                lootList = unlessfunc
                                            },
                                            quantity = 1
                                        });

                                        MultiUpdatesForCommonCore.Add(new ApplyProfileChangesClassV2
                                        {
                                            changeType = "itemAdded",
                                            itemId = RandomOfferId,
                                            item = new
                                            {
                                                templateId = "GiftBox:gb_battlepass",
                                                attributes = new
                                                {
                                                    max_level_bonus = 0,
                                                    fromAccountId = "",
                                                    lootList = unlessfunc
                                                },
                                                quantity = 1
                                            }
                                        });


                                        await XmppGift.NotifyUser(profileCacheEntry.AccountId);
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
                        else
                        {
                            // season 1 only free tier
                            foreach (var BattlePass in FreeTier)
                            {
                                if (!NeedItems) break;
                                if (BattlePass.Level <= BookLevelOG) continue;
                                if (BattlePass.Level > FoundSeason.Level) break;

                                List<NotificationsItemsClassOG> unlessfunc;
                                (profileCacheEntry, FoundSeason, MultiUpdates, currencyItem, NeedItems, unlessfunc) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, FoundSeason, MultiUpdates, currencyItem, NeedItems);
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




                MultiUpdates.Add(new
                {
                    changeType = "itemQuantityChanged",
                    name = "Currency",
                    value = currencyItem.quantity
                });
            }

            return (profileCacheEntry, MultiUpdates, MultiUpdatesForCommonCore);
        }
    }
}
