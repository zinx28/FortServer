//using FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog;
//using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
//using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
//using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Purchases;
//using FortBackend.src.App.Utilities.MongoDB.Module;
//using Microsoft.AspNetCore.Http.HttpResults;
//using static FortBackend.src.App.Utilities.Helpers.Grabber;

//namespace FortBackend.src.App.Routes.Profile.McpControllers
//{
//    public class PurchaseCatalogEntry
//    {
//        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, Account AccountDataParsed, PurchaseCatalogEntryRequest Body)
//        {
//            Console.WriteLine(ProfileId);
//            if (ProfileId == "common_core")
//            {
//                SeasonClass[] Seasons = AccountDataParsed.commoncore.Seasons;

//                if (AccountDataParsed.commoncore.Seasons != null)
//                {
//                    SeasonClass seasonObject = AccountDataParsed.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season);

//                    if (seasonObject != null)
//                    {
//                        int PurchaseQuantity = Body.purchaseQuantity;

//                        if (PurchaseQuantity < 1)
//                        {
//                            throw new BaseError
//                            {
//                                errorCode = "errors.com.epicgames.modules.catalog",
//                                errorMessage = "Catalog Limit is at least 1!",
//                                messageVars = new List<string> { "PurchaseCatalogEntry" },
//                                numericErrorCode = 12801,
//                                originatingService = "any",
//                                intent = "prod",
//                                error_description = "Catalog Limit is at least 1!",
//                            };
//                        }

//                        if (Body.currency == "MtxCurrency")
//                        {
//                            if (Body.offerId != null)
//                            {
//                                string OfferId = Body.offerId;

//                                if (OfferId.Contains(":/"))
//                                {
//                                    Mcp mcp = await PurchaseItem.Init(Season, ProfileId, Body, AccountDataParsed);
//                                    return mcp;
//                                }
//                            }
//                        }

//                        throw new BaseError
//                        {
//                            errorCode = "errors.com.epicgames.modules.catalog",
//                            errorMessage = "Error trying to purchase item",
//                            messageVars = new List<string> { "PurchaseCatalogEntry" },
//                            numericErrorCode = 12801,
//                            originatingService = "any",
//                            intent = "prod",
//                            error_description = "Error trying to purchase item",
//                        };
//                    }
//                }
//                //Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, AccountDataParsed);
//                //return response;
//            }

//            return new Mcp();
//        }
//    }
//}
