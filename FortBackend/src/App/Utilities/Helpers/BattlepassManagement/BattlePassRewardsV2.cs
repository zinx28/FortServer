using Discord;
using FortBackend.src.App.Utilities.Discord.Helpers.command;
using FortBackend.src.App.Utilities.Quests;
using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;
using FortLibrary.Shop;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FortBackend.src.App.Utilities.Helpers.BattlepassManagement
{
    // this is for season 17 and above due to claiming worked differently
    public class BattlePassRewardsV2
    {
        public static async Task<(ProfileCacheEntry profileCacheEntry, SeasonClass FoundSeason, List<object> MultiUpdates, 
            int currency, List<NotificationsItemsClassOG> applyProfileChanges)> Init(SeasonBPPurchase ItemToClaim, ProfileCacheEntry profileCacheEntry, SeasonClass FoundSeason, List<object> MultiUpdates, int currency, List<NotificationsItemsClassOG> applyProfileChanges = null)
        {
          
            if (!profileCacheEntry.AccountData.athena.Items.ContainsKey(ItemToClaim.templateId))
            {
                if (!profileCacheEntry.AccountData.commoncore.Items.ContainsKey(ItemToClaim.templateId))
                {
                    if (ItemToClaim.templateId.Contains("HomebaseBannerIcon"))
                    {
                        profileCacheEntry.AccountData.commoncore.Items.Add(ItemToClaim.templateId, new CommonCoreItem
                        {
                            templateId = ItemToClaim.templateId,
                            attributes = new CommonCoreItemAttributes
                            {
                                item_seen = false
                            },
                            quantity = ItemToClaim.Quantity,
                        });

                        MultiUpdates.Add(new MultiUpdateClass
                        {
                            changeType = "itemAdded",
                            itemId = ItemToClaim.templateId,
                            item = new AthenaItem
                            {
                                templateId = ItemToClaim.templateId,
                                attributes = new AthenaItemAttributes
                                {
                                    item_seen = false,
                                },
                                quantity = 1
                            }
                        });
                    }

                    else if (ItemToClaim.templateId.Contains("Athena"))
                    {

                        MultiUpdates.Add(new MultiUpdateClass
                        {
                            changeType = "itemAdded",
                            itemId = ItemToClaim.templateId,
                            item = new AthenaItem
                            {
                                templateId = ItemToClaim.templateId,
                                attributes = new AthenaItemAttributes
                                {
                                    item_seen = false,
                                },
                                quantity = 1
                            }
                        });

                        profileCacheEntry.AccountData.athena.Items.Add(ItemToClaim.templateId, new AthenaItem
                        {
                            templateId = ItemToClaim.templateId,
                            attributes = new AthenaItemAttributes
                            {
                                favorite = false,
                                item_seen = false,
                                level = 1,
                                max_level_bonus = 0,
                                rnd_sel_cnt = 0,
                                variants = ItemToClaim.variants,
                                xp = 0
                            },
                            quantity = ItemToClaim.Quantity,
                        });
                    }
                    else if (ItemToClaim.templateId.Contains("CosmeticVariantToken:"))
                    {
                        // not the best way
                        var variantsByItem = ItemToClaim.new_variants
                          .Where(v => !string.IsNullOrEmpty(v.connectedItem))
                          .GroupBy(v => v.connectedItem);

                        foreach (var group in variantsByItem)
                        {
                            var itemId = group.Key;

                            if (!profileCacheEntry.AccountData.athena.Items.TryGetValue(itemId, out var athenaItem))
                            {
                                Logger.Error(itemId, "You don't own the connected template?");
                                continue;
                            }

                            var itemVariants = athenaItem.attributes.variants;

                            foreach (var variant in group)
                            {
                                var existingT = itemVariants
                                    .FirstOrDefault(v => v.channel == variant.channel);

                                if (existingT != null)
                                {
                                    existingT.owned.AddRange(variant.added);
                                }
                                else
                                {
                                    itemVariants.Add(new AthenaItemVariants
                                    {
                                        channel = variant.channel,
                                        active = variant.added.First(),
                                        owned = variant.added
                                    });
                                }
                            }

                            profileCacheEntry.AccountData.athena.Items[itemId].attributes.variants = itemVariants;

                            MultiUpdates.Add(new
                            {
                                changeType = "itemAttrChanged",
                                itemId = itemId,
                                attributeName = "variants",
                                attributeValue = itemVariants
                            });

                        }
                    }
                    else if (ItemToClaim.templateId.Contains("Currency:"))
                    {
                        currency += ItemToClaim.Quantity;
                    }


                    var existing = applyProfileChanges.FirstOrDefault(e => e.itemType == ItemToClaim.templateId);
                    if (existing != null)
                        existing.quantity += ItemToClaim.Quantity;
                    else
                    {
                        applyProfileChanges.Add(new NotificationsItemsClassOG
                        {
                            itemType = ItemToClaim.templateId,
                            itemGuid = ItemToClaim.templateId,
                            quantity = ItemToClaim.Quantity
                        });
                    }
                }
            }
            

            return (profileCacheEntry, FoundSeason, MultiUpdates, currency, applyProfileChanges);
        }
    }
}