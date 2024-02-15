using FortBackend.src.App.Routes.APIS.Profile.McpControllers.AthenaResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FortBackend.src.App.Routes.APIS.Profile.McpControllers
{
    public class PurchaseCatalogEntry
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, int Season, int RVN, Account AccountDataParsed, PurchaseCatalogEntryRequest Body)
        {
            Console.WriteLine(ProfileId);
            if (ProfileId == "common_core")
            {
                Season[] Seasons = AccountDataParsed.commoncore.Seasons;

                if (AccountDataParsed.commoncore.Seasons != null)
                {
                    Season seasonObject = AccountDataParsed.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season);

                    if (seasonObject != null)
                    {
                        int PurchaseQuantity = Body.purchaseQuantity;

                        if(PurchaseQuantity < 1 )
                        {
                            //throw new BaseError
                            //{

                            //};
                        }
                    }
                }
                //Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, AccountDataParsed);
                //return response;
            }

            return new Mcp();
        }
    }
}
