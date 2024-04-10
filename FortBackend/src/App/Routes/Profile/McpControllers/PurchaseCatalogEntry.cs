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

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class PurchaseCatalogEntry
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, PurchaseCatalogEntryRequest Body)
        {
            Console.WriteLine(ProfileId);
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

                                if (OfferId.Contains(":/"))
                                {
                                    Mcp mcp = await PurchaseItem.Init(Season, ProfileId, Body, profileCacheEntry);
                                    return mcp;
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
                    }else
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
