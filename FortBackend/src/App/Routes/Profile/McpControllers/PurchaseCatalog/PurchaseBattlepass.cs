using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile;
using FortLibrary;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary.Dynamics;
using FortLibrary.Shop;
using FortLibrary.EpicResponses.Storefront;
using Microsoft.IdentityModel.Tokens;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;

namespace FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog
{
    public class PurchaseBattlepass
    {
        public static async Task<Mcp> Init(VersionClass Season, string ProfileId, PurchaseCatalogEntryRequest Body, ProfileCacheEntry profileCacheEntry, StoreBattlepassPages battlepass)
        {
            string OfferId = Body.offerId;
            int Price = 0;

            List<object> MultiUpdates = new List<object>();

            int BaseRev = profileCacheEntry.AccountData.commoncore.RVN;
            int BaseRev2 = profileCacheEntry.AccountData.athena.RVN;
            catalogEntrieStore ShopContent = new catalogEntrieStore();

            foreach (catalogEntrieStore storefront in battlepass.catalogEntries)
            {
                if (storefront.offerId == OfferId)
                {
                    ShopContent = storefront;
                    Price = storefront.prices[0].finalPrice;
                    break; // found it
                }
            }

            if (!string.IsNullOrEmpty(ShopContent.offerId))
            {
                List<SeasonClass> Seasons = profileCacheEntry.AccountData.commoncore.Seasons;

                if (profileCacheEntry.AccountData.commoncore.Seasons != null)
                {
                    SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;

                    if (seasonObject != null)
                    {
                        
                        // I need to work on this
                        var currencyItem = profileCacheEntry.AccountData.commoncore.Items["Currency"];

                        if (Price > currencyItem.quantity)
                        {
                            throw new BaseError()
                            {
                                errorCode = "errors.com.epicgames.modules.catalog",
                                errorMessage = "Not enough vbucks/ did you bypass",
                                messageVars = new List<string> { "PurchaseCatalogEntry" },
                                numericErrorCode = 12801,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Not enough vbucks",
                            };
                        }

                        if (ShopContent.devName.ToString().Contains("SingleTier"))
                        {
                            if (!seasonObject.BookPurchased)
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "Required Battlepass",
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "Required Battlepass",
                                };
                            }

                            throw new BaseError()
                            {
                                errorCode = "errors.com.epicgames.modules.catalog",
                                errorMessage = "FortBackend Doesn't Support This At the moment~ SingleTier",
                                messageVars = new List<string> { "PurchaseCatalogEntry" },
                                numericErrorCode = 12801,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "FortBackend Doesn't Support This At the moment~ SingleTier",
                            };
                        }
                        else
                        {
                            // not proper response
                            if (seasonObject.BookPurchased)
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "You already own the bp",
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "You already own the bp",
                                };
                            }

                            if (ShopContent.devName.ToString().Contains("BattleBundle"))
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "FortBackend Doesn't Support This At the moment",
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "FortBackend Doesn't Support This At the moment",
                                };
                            }
                            else if (ShopContent.devName.ToString().Contains("BattlePass"))
                            {
                                currencyItem.quantity -= Price;
                                seasonObject.BookPurchased = true;
                                bool NeedItems = true;
                                int BookLevelOG = seasonObject.BookLevel;
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
                                                    foreach (var BattlePass in FreeTier)
                                                    {
                                                        if (!NeedItems) break;
                                                        //We don't need this check on purchase as we "WANT" the user to get them items
                                                        //if (BookLevelOG <= BattlePass.Level) continue;
                                                        if (BattlePass.Level > seasonObject.Level) break;

                                                        (profileCacheEntry, seasonObject, MultiUpdates, currencyItem, NeedItems) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, seasonObject, MultiUpdates, currencyItem, NeedItems);
                                                    }

                                                    foreach (var BattlePass in PaidTier)
                                                    {
                                                        if (!NeedItems) break;
                                                        //if (BookLevelOG <= BattlePass.Level) continue;
                                                        if (BattlePass.Level > seasonObject.Level) break;


                                                        (profileCacheEntry, seasonObject, MultiUpdates, currencyItem, NeedItems) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, seasonObject, MultiUpdates, currencyItem, NeedItems);
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
                            }
                            else
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "?????",
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "?????",
                                };
                            }
                        }

                       


                    }
                }
            }

            return new Mcp();
        }
    }
}
