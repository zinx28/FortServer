using Discord;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Shop.Helpers.Class;
using FortBackend.src.App.Utilities.Shop.Helpers.Data;
using Newtonsoft.Json;

namespace FortBackend.src.App.Routes.APIS.Profile.McpControllers.PurchaseCatalog
{
    public class PurchaseItem
    {
        public static async Task<Mcp> Init(PurchaseCatalogEntryRequest Body, Account AccountDataParsed)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "json", "shop", "shop.json");
            string json = System.IO.File.ReadAllText(filePath);

            if(string.IsNullOrEmpty(json))
            {
                throw new BaseError()
                {
                    errorCode = "errors.com.epicgames.modules.catalog",
                    errorMessage = "Server Sided Issue",
                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                    numericErrorCode = 12801,
                    originatingService = "any",
                    intent = "prod",
                    error_description = "Server Sided Issue",
                };
            }
            ShopJson shopData = JsonConvert.DeserializeObject<ShopJson>(json);

            string[] SplitOfferId = Body.offerId.Split(":/");
            string SecondSplitOfferId = SplitOfferId[1];
            ItemsSaved ShopContent = new ItemsSaved();
            var NotificationsItems = new List<object>();
            var MultiUpdates = new List<object>();

            foreach (ItemsSaved storefront in shopData.ShopItems.Daily)
            {
                if(storefront.id == SecondSplitOfferId)
                {
                    ShopContent = storefront;
                }
            }

            foreach (ItemsSaved storefront in shopData.ShopItems.Weekly)
            {
                if (storefront.id == SecondSplitOfferId)
                {
                    ShopContent = storefront;
                }
            }

            if(!string.IsNullOrEmpty(ShopContent.id)) {
                bool HasUserHaveItem = AccountDataParsed.athena.Items.Any(item => item.ContainsKey(ShopContent.id));

                if(HasUserHaveItem)
                {
                    throw new BaseError()
                    {
                        errorCode = "errors.com.epicgames.modules.catalog",
                        errorMessage = "You already own the item",
                        messageVars = new List<string> { "PurchaseCatalogEntry" },
                        numericErrorCode = 12801,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "You already own the item",
                    };
                }

                NotificationsItems.Add(new
                {
                    itemType = ShopContent.id,
                    itemGuid = ShopContent.id,
                    itemProfile = "athena",
                    quantity = 1
                });

                //MultiUpdates.Add(new
                //{
                //    changeType = "itemAdded",
                //    itemId = ShopContent.item,
                //    item = new
                //    {
                //        templateId = ShopContent.item,
                //        attribute = new
                //        {

                //        }
                //    }
                //});

            
            }

            return new Mcp();
        }
    }
}
