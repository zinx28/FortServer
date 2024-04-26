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

namespace FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog
{
    public class PurchaseBattlepass
    {
        public static async Task<Mcp> Init(VersionClass Season, string ProfileId, PurchaseCatalogEntryRequest Body, ProfileCacheEntry profileCacheEntry, StoreBattlepassPages battlepass)
        {
            string OfferId = Body.offerId;
            int Price = 0;

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
                        if (seasonObject.BookPurchased)
                        {
                            // not proper response
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
                        // I need to work on this
                        var currencyItem = profileCacheEntry.AccountData.commoncore.Items["Currency"];

                        if (Price > currencyItem.quantity)
                        {
                            throw new BaseError()
                            {
                                errorCode = "errors.com.epicgames.modules.catalog",
                                errorMessage = "Not enough vbucks",
                                messageVars = new List<string> { "PurchaseCatalogEntry" },
                                numericErrorCode = 12801,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Not enough vbucks",
                            };
                        }
                    }
                }
            }

            return new Mcp();
        }
    }
}
