using FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.MongoDB.Module;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Purchases;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary;
using FortLibrary.Dynamics;
using Newtonsoft.Json;
using FortLibrary.EpicResponses.Storefront;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class PurchaseCatalogEntry
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, PurchaseCatalogEntryRequest Body)
        {
            if (ProfileId == "common_core" || ProfileId == "profile0")
            {
                List<SeasonClass> Seasons = profileCacheEntry.AccountData.commoncore.Seasons;

                if (profileCacheEntry.AccountData.commoncore.Seasons != null)
                {
                    SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;

                    if (seasonObject != null)
                    {
                        int PurchaseQuantity = Body.purchaseQuantity;

                        if (PurchaseQuantity < 1)
                        {
                            throw new BaseError
                            {
                                errorCode = "errors.com.epicgames.modules.catalog",
                                errorMessage = "Catalog Limit is at least 1!",
                                messageVars = new List<string> { "PurchaseCatalogEntry" },
                                numericErrorCode = 12801,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Catalog Limit is at least 1!",
                            };
                        }
                     
                        if (Body.currency == "MtxCurrency")
                        {
                            if (Body.offerId != null)
                            {
                                string OfferId = Body.offerId;

                                if (string.IsNullOrEmpty(OfferId))
                                {
                                    throw new BaseError
                                    {
                                        errorCode = "errors.com.epicgames.modules.catalog",
                                        errorMessage = "Couldn't find battlepass offer: " + OfferId,
                                        messageVars = new List<string> { "PurchaseCatalogEntry" },
                                        numericErrorCode = 12801,
                                        originatingService = "any",
                                        intent = "prod",
                                        error_description = "Catalog Limit is at least 1!",
                                    };
                                }


                                if (OfferId.Contains(":/"))
                                {
                                    Mcp mcp = await PurchaseItem.Init(Season, ProfileId, Body, profileCacheEntry);
                                    return mcp;
                                }
                                else
                                {
                                    // should be battlepass but we can verify that
                                    StoreBattlepassPages battlepass = BattlepassManager.BattlePasses.FirstOrDefault(e => e.Key == Season.Season).Value;

                                    if (battlepass == null)
                                    {
                                        Logger.Error("Couldn't find bp items for this season", "PruchaseCatlaogEntry");
                                        throw new BaseError
                                        {
                                            errorCode = "errors.com.epicgames.modules.catalog",
                                            errorMessage = "Couldn't find battlepass offer: " + OfferId,
                                            messageVars = new List<string> { "PurchaseCatalogEntry" },
                                            numericErrorCode = 12801,
                                            originatingService = "any",
                                            intent = "prod",
                                            error_description = "Catalog Limit is at least 1!",
                                        };
                                    }

                                    var SingleTierOffer = battlepass.catalogEntries.FirstOrDefault(e => e.offerId == OfferId && e.devName.Contains("SingleTier"));

                                    if (battlepass != null)
                                    {
                                        bool FoundId = false;
                                        foreach (catalogEntrieStore a in battlepass.catalogEntries)
                                        {
                                            if(a.offerId == OfferId)
                                            {
                                                FoundId = true;
                                                break;
                                            }
                                        }

                                        if (FoundId)
                                        {
                                            Mcp mcp = await PurchaseBattlepass.Init(Season, ProfileId, Body, profileCacheEntry, battlepass);
                                            return mcp;
                                        }
                                        else
                                        {
                                            throw new BaseError
                                            {
                                                errorCode = "errors.com.epicgames.modules.catalog",
                                                errorMessage = "Couldn't find battlepass offer: " + OfferId,
                                                messageVars = new List<string> { "PurchaseCatalogEntry" },
                                                numericErrorCode = 12801,
                                                originatingService = "any",
                                                intent = "prod",
                                                error_description = "Catalog Limit is at least 1!",
                                            };
                                        }
                                    }
                                }
                            }
                        }

                        throw new BaseError
                        {
                            errorCode = "errors.com.epicgames.modules.catalog",
                            errorMessage = "Error trying to purchase item",
                            messageVars = new List<string> { "PurchaseCatalogEntry" },
                            numericErrorCode = 12801,
                            originatingService = "any",
                            intent = "prod",
                            error_description = "Error trying to purchase item",
                        };
                    }
                    else
                    {
                        throw new BaseError
                        {
                            errorCode = "errors.com.epicgames.modules.catalog",
                            errorMessage = "Backend Issue",
                            messageVars = new List<string> { "PurchaseCatalogEntry" },
                            numericErrorCode = 12801,
                            originatingService = "any",
                            intent = "prod",
                            error_description = "Backend Issue",
                        };
                    }
                }
                //Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, AccountDataParsed);
                //return response;
            }

            return new Mcp();
        }
    }
}
