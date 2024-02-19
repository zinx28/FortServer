using Discord;
using FortBackend.src.App.Routes.APIS.Profile.McpControllers.AthenaResponses;
using FortBackend.src.App.Routes.APIS.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Purchases;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Shop.Helpers.Class;
using FortBackend.src.App.Utilities.Shop.Helpers.Data;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.APIS.Profile.McpControllers.PurchaseCatalog
{
    public class PurchaseItem
    {
        public static async Task<Mcp> Init(VersionClass Season, string ProfileId, PurchaseCatalogEntryRequest Body, Account AccountDataParsed)
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
            var NotificationsItems = new List<NotificationsItemsClass>();
            var MultiUpdates = new List<object>();
            var ApplyProfileChanges = new List<object>();
            Dictionary<string, object> UpdatedData = new Dictionary<string, object>();
            List<Dictionary<string, object>> itemList = new List<Dictionary<string, object>>();
            int BaseRev = AccountDataParsed.commoncore.RVN;

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

                Console.WriteLine(HasUserHaveItem);

                NotificationsItems.Add(new NotificationsItemsClass
                {
                    itemType = ShopContent.item,
                    itemGuid = ShopContent.id,
                    itemProfile = "athena"
                });

                MultiUpdates.Add(new MultiUpdateClass
                {
                    changeType = "itemAdded",
                    itemId = ShopContent.item,
                    item = new AthenaItem
                    {
                        templateId = ShopContent.item,
                        attributes = new AthenaItemAttributes
                        {
                            item_seen = false,
                            variants = ShopContent.variants,
                        },
                        quantity = 1
                    }
                });

                foreach (Item ItemShopItem in ShopContent.items)
                {
                    NotificationsItems.Add(new NotificationsItemsClass
                    {
                        itemType = ItemShopItem.item,
                        itemGuid = ItemShopItem.item,
                        itemProfile = "athena"
                    });

                    MultiUpdates.Add(new MultiUpdateClass
                    {
                        changeType = "itemAdded",
                        itemId = ItemShopItem.item,
                        item = new AthenaItem
                        {
                            templateId = ItemShopItem.item,
                            attributes = new AthenaItemAttributes
                            {
                                item_seen = false,
                                variants = ItemShopItem.variants,
                            },
                            quantity = 1
                        }
                    });
                }
                //AthenaItem test = AccountDataParsed.commoncore.Items.FirstOrDefault(e => e.ContainsKey("Currency"))["Currency"] as AthenaItem;

                // I need to work on this

                int GrabPlacement = -1;
                GrabPlacement = AccountDataParsed.commoncore.Items.SelectMany((item, index) => new List<(Dictionary<string, object> Item, int Index)> { (Item: item, Index: index) })
                .TakeWhile(pair => !pair.Item.ContainsKey("Currency"))
                .Count();
                //Console.WriteLine(JsonConvert.SerializeObject(AccountDataParsed, Formatting.Indented));
                var currencyItem = AccountDataParsed.commoncore.Items[GrabPlacement]["Currency"] as dynamic;

                try
                {
                   // AthenaItem currencyItem2 = currencyItem as AthenaItem;
                    Console.WriteLine(currencyItem);
                   // Console.WriteLine(currencyItem2.quantity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //Console.WriteLine(AccountDataParsed.commoncore.Items[GrabPlacement]["Currency"]);
                if (currencyItem.quantity == 0)
                {
                    throw new BaseError()
                    {
                        errorCode = "errors.com.epicgames.modules.catalog",
                        errorMessage = "Your Poor",
                        messageVars = new List<string> { "PurchaseCatalogEntry" },
                        numericErrorCode = 12801,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "Your Poor",
                    };
                }

                Console.WriteLine($"Currency Item Quantity: {currencyItem.quantity}");

                if (ShopContent.price > int.Parse(currencyItem.quantity.ToString()))
                {
                    throw new BaseError()
                    {
                        errorCode = "errors.com.epicgames.modules.catalog",
                        errorMessage = "Item is higher price",
                        messageVars = new List<string> { "PurchaseCatalogEntry" },
                        numericErrorCode = 12801,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "Item is higher price",
                    };
                }

                int Price = currencyItem.quantity - ShopContent.price;

                ApplyProfileChanges.Add(new ApplyProfileChangesClass
                { 
                    changeType = "itemQuantityChanged",
                    itemId = "Currency",
                    quantity = Price
                });
                

                var newItem699 = new Dictionary<string, object>
                {
                    {
                        $"{ShopContent.item}", new Dictionary<string, object>
                        {
                            { "TemplateId", $"{ShopContent.item}" },
                            {
                                "Attributes", new Dictionary<string, object>
                                {
                                    { "favorite", false },
                                    { "item_seen", false },
                                    { "level", 1 },
                                    { "max_level_bonus", 0 },
                                    { "rnd_sel_cnt", 0 },
                                    { "variants", ShopContent.variants },
                                    { "xp", 0 }
                                }
                            },
                            {  "Quantity", 1 }
                        }
                    }
                };

                itemList.Add(newItem699);

                foreach (Item ItemInItems in ShopContent.items)
                {
                    itemList.Add(new Dictionary<string, object>
                    {
                        {
                            $"{ItemInItems.item}", new Dictionary<string, object>
                            {
                                { "TemplateId", $"{ItemInItems.item}" },
                                {
                                    "Attributes", new Dictionary<string, object>
                                    {
                                        { "favorite", false },
                                        { "item_seen", false },
                                        { "level", 1 },
                                        { "max_level_bonus", 0 },
                                        { "rnd_sel_cnt", 0 },
                                        { "variants", ItemInItems.variants },
                                        { "xp", 0 }
                                    }
                                },
                                {  "Quantity", 1 }
                            }
                        }
                    });
                }

                UpdatedData.Add($"commoncore.items.{GrabPlacement}.Currency.quantity", Price);
                //if (MultiUpdates.Count > 0)
                //{
                //    UpdatedData.Add($"athena.RVN", AccountDataParsed.athena.RVN + 1);
                //    UpdatedData.Add($"athena.CommandRevision", AccountDataParsed.athena.CommandRevision + 1);
                //}
                if (ApplyProfileChanges.Count > 0)
                {
                    UpdatedData.Add($"commoncore.RVN", AccountDataParsed.commoncore.RVN + 1);
                    UpdatedData.Add($"commoncore.CommandRevision", AccountDataParsed.commoncore.CommandRevision + 1);
                }
              
                await Handlers.UpdateOne<Account>("accountId", AccountDataParsed.AccountId, UpdatedData);

                await Handlers.PushOne<Account>("accountId", AccountDataParsed.AccountId, new Dictionary<string, object>
                {
                    {
                        $"athena.items", itemList
                    }
                });

                var AthenaNew = await Handlers.FindOne<Account>("accountId", AccountDataParsed.AccountId);

                AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AthenaNew)?[0];

                Console.WriteLine("TEST");

                List<dynamic> BigA = new List<dynamic>();
                if (Season.SeasonFull >= 12.20)
                {
                    Mcp test = await CommonCoreResponse.Grab(AccountDataParsed.AccountId, ProfileId, Season, AccountDataParsed.commoncore.RVN, AccountDataParsed);
                    ApplyProfileChanges = test.profileChanges;
                }

                Mcp mcp = new Mcp()
                {
                    profileRevision = AccountDataParsed.commoncore.RVN + 1,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev + 1,
                    profileChanges = ApplyProfileChanges,
                    notifications = new List<McpNotifications>()
                    {
                        new McpNotifications
                        {
                            type = "CatalogPurchase",
                            primary =  true,
                            lootResult = new LootResultClass
                            {
                                items = NotificationsItems
                            }
                        }
                    },
                    profileCommandRevision = AccountDataParsed.commoncore.CommandRevision + 1,
                    serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                    multiUpdate = new List<object>()
                    {
                        new
                        {
                            profileRevision = AccountDataParsed.athena.RVN + 1,
                            profileId = "athena",
                            profileChangesBaseRevision = BaseRev + 1,
                            profileChanges = MultiUpdates,
                            profileCommandRevision = AccountDataParsed.athena.CommandRevision + 1,
                        }
                    },
                    responseVersion = 1
                };
                string mcpJson = JsonConvert.SerializeObject(mcp, Formatting.Indented);
                Console.WriteLine(mcpJson);
                return mcp;
            };

            return new Mcp();
        }
    }
}
