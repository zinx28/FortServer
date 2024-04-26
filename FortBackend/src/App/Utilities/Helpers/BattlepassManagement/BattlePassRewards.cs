using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB.Module;

namespace FortBackend.src.App.Utilities.Helpers.BattlepassManagement
{
    public class BattlePassRewards
    {
        public static async Task<(ProfileCacheEntry profileCacheEntry, SeasonClass FoundSeason, List<object> MultiUpdates, CommonCoreItem currencyItem, bool NeedItems, List<NotificationsItemsClassOG> applyProfileChanges)> Init(List<ItemInfo> Rewards, ProfileCacheEntry profileCacheEntry, SeasonClass FoundSeason, List<object> MultiUpdates, CommonCoreItem currencyItem, bool NeedItems, List<NotificationsItemsClassOG> applyProfileChanges = null)
        {
            foreach (ItemInfo iteminfo in Rewards)
            {
                if (!profileCacheEntry.AccountData.athena.Items.ContainsKey(iteminfo.TemplateId))
                {
                    if (!profileCacheEntry.AccountData.commoncore.Items.ContainsKey(iteminfo.TemplateId))
                    {
                        if (iteminfo.TemplateId.Contains("HomebaseBannerIcon"))
                        {
                            profileCacheEntry.AccountData.commoncore.Items.Add(iteminfo.TemplateId, new CommonCoreItem
                            {
                                templateId = iteminfo.TemplateId,
                                attributes = new CommonCoreItemAttributes
                                {
                                    item_seen = false
                                },
                                quantity = iteminfo.Quantity,
                            });
                        }
                        else if (iteminfo.TemplateId.Contains("Athena"))
                        {
                            profileCacheEntry.AccountData.athena.Items.Add(iteminfo.TemplateId, new AthenaItem
                            {
                                templateId = iteminfo.TemplateId,
                                attributes = new AthenaItemAttributes
                                {
                                    favorite = false,
                                    item_seen = false,
                                    level = 1,
                                    max_level_bonus = 0,
                                    rnd_sel_cnt = 0,
                                    variants = new List<AthenaItemVariants>(),
                                    xp = 0
                                },
                                quantity = iteminfo.Quantity,
                            });
                        }
                        else if (iteminfo.TemplateId.Contains("Token:"))
                        {
                            if (iteminfo.TemplateId.Contains("athenaseasonfriendxpboost"))
                            {
                                FoundSeason.season_friend_match_boost += iteminfo.Quantity;

                                MultiUpdates.Add(new
                                {
                                    changeType = "statModified",
                                    name = "season_match_boost",
                                    value = FoundSeason.season_friend_match_boost
                                });
                            }
                            else if (iteminfo.TemplateId.Contains("athenaseasonxpboost"))
                            {
                                //  season_match_boost
                                FoundSeason.season_match_boost += iteminfo.Quantity;

                                MultiUpdates.Add(new
                                {
                                    changeType = "statModified",
                                    name = "season_friend_match_boost",
                                    value = FoundSeason.season_friend_match_boost
                                });
                            }
                            else
                            {
                                Logger.Log($"{iteminfo.TemplateId} is not supported", "ClientQuestLogin");
                            }
                        }
                        else if (iteminfo.TemplateId.Contains("Currency:"))
                        {
                            currencyItem.quantity += iteminfo.Quantity;
                        }
                        else if (iteminfo.TemplateId.Contains("AccountResource:"))
                        {
                            FoundSeason.SeasonXP += iteminfo.Quantity;

                            (FoundSeason, NeedItems) = await LevelUpdater.Init(FoundSeason.SeasonNumber, FoundSeason, NeedItems);
                            NeedItems = true; // force it as we don't want this to become false here
                        }
                        else
                        {
                            Logger.Log($"{iteminfo.TemplateId} is not supported", "ClientQuestLogin");
                        }

                        applyProfileChanges.Add(new NotificationsItemsClassOG
                        {
                            itemType = iteminfo.TemplateId,
                            itemGuid = iteminfo.TemplateId,
                            quantity = iteminfo.Quantity,
                        });
                    }
                }
            }

            return (profileCacheEntry, FoundSeason, MultiUpdates, currencyItem, NeedItems,  applyProfileChanges);
        }
    }
}
