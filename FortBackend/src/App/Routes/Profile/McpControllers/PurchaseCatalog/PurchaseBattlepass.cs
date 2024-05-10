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
using Newtonsoft.Json;
using Discord;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog
{
    public class PurchaseBattlepass
    {
        public static async Task<Mcp> Init(VersionClass Season, string ProfileId, PurchaseCatalogEntryRequest Body, ProfileCacheEntry profileCacheEntry, StoreBattlepassPages battlepass)
        {
            string OfferId = Body.offerId;
            int Price = 0;

            List<object> MultiUpdates = new List<object>();
            List<object> ApplyProfileChanges = new List<object>();
            var NotificationsItems = new List<NotificationsItemsClass>();

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

                                                    currencyItem.quantity -= Price;
                                                  

                                                    seasonObject.BookPurchased = true;

                                                    ApplyProfileChanges.Add(new
                                                    {
                                                        changeType = "statModified",
                                                        name = "book_purchased",
                                                        value = true
                                                    });

                                                   
                                                    List<NotificationsItemsClassOG> ItemsGivenToUser = new List<NotificationsItemsClassOG>();

                                                    foreach (Battlepass BattlePass in FreeTier)
                                                    {
                                                        if (!NeedItems) break;
                                                        //We don't need this check on purchase as we "WANT" the user to get them items
                                                        //if (BookLevelOG <= BattlePass.Level) continue;
                                                        if (BattlePass.Level > seasonObject.Level) break;
                                                        
                                                        (profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems, ItemsGivenToUser) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems, ItemsGivenToUser);
                                                    }

                                                    foreach (Battlepass BattlePass in PaidTier)
                                                    {
                                                        if (!NeedItems) break;
                                                        //if (BookLevelOG <= BattlePass.Level) continue;
                                                        if (BattlePass.Level > seasonObject.Level) break;
                                                       

                                                        (profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems, ItemsGivenToUser) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems, ItemsGivenToUser);
                                                    }

                                                    // after to get the correct price
                                                    MultiUpdates.Add(new
                                                    {
                                                        changeType = "itemQuantityChanged",
                                                        itemId = "Currency",
                                                        quantity = currencyItem.quantity
                                                    });
                                                    /*
                                                     *   NewItemsGiven.Add(new Dictionary<string, object>
                                                        {
                                                            { "itemType", FreeRewards["TemplateId"].ToString() },
                                                            { "itemGuid", FreeRewards["TemplateId"].ToString() },
                                                            { "quantity", int.Parse(FreeRewards["Quantity"].ToString() ?? "1") }
                                                        });
                                                    */
                                                    var RandomOfferId = Guid.NewGuid().ToString();
                                                    MultiUpdates.Add(new ApplyProfileChangesClassV2
                                                    {
                                                        changeType = "itemAdded",
                                                        itemId = RandomOfferId,
                                                        item = new
                                                        {
                                                            templateId = Season.Season >= 5 ? "GiftBox:gb_battlepasspurchased" : "GiftBox:gb_battlepass",
                                                            attributes = new
                                                            {
                                                                max_level_bonus = 0,
                                                                fromAccountId = "",
                                                                lootList = ItemsGivenToUser
                                                            },
                                                            quantity = 1
                                                        }
                                                    });

                                                    profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                                                    {
                                                        templateId = Season.Season >= 5 ? "GiftBox:gb_battlepasspurchased" : "GiftBox:gb_battlepass",
                                                        attributes = new GiftCommonCoreItemAttributes
                                                        {
                                                            lootList = ItemsGivenToUser
                                                        },
                                                        quantity = 1
                                                    });

                                                    if(!string.IsNullOrEmpty(profileCacheEntry.AccountId))
                                                    {
                                                        Clients Client = GlobalData.Clients.FirstOrDefault(client => client.accountId == profileCacheEntry.AccountId)!;

                                                        if (Client != null)
                                                        {
                                                            string xmlMessage;
                                                            byte[] buffer;
                                                            WebSocket webSocket = Client.Game_Client;
                                                            Console.WriteLine(webSocket.State);
                                                            if(webSocket != null && webSocket.State == WebSocketState.Open)
                                                            {
                                                                XNamespace clientNs = "jabber:client";

                                                                var message = new XElement(clientNs + "message",
                                                                  new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                                  new XAttribute("to", profileCacheEntry.AccountId),
                                                                  new XElement(clientNs + "body", JsonConvert.SerializeObject(new
                                                                  {
                                                                      payload = new { },
                                                                      type = "com.epicgames.gift.received",
                                                                      timestamp = DateTime.UtcNow.ToString("o")
                                                                  }))
                                                                );

                                                                xmlMessage = message.ToString();
                                                                buffer = Encoding.UTF8.GetBytes(xmlMessage);

                                                                Console.WriteLine(xmlMessage);

                                                                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                                            }
                                                            
                                                        }

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


                        // Response

                        if (MultiUpdates.Count > 0)
                        {
                            profileCacheEntry.AccountData.athena.RVN += 1;
                            profileCacheEntry.AccountData.athena.CommandRevision += 1;
                        }

                        if (ApplyProfileChanges.Count > 0)
                        {
                            profileCacheEntry.AccountData.commoncore.RVN += 1;
                            profileCacheEntry.AccountData.commoncore.CommandRevision += 1;
                        }


                        Mcp mcp = new Mcp()
                        {
                            profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                            profileId = ProfileId,
                            profileChangesBaseRevision = BaseRev,
                            profileChanges = MultiUpdates,
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
                            profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                            serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            multiUpdate = new List<object>()
                            {
                                new
                                {
                                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                                    profileId = "athena",
                                    profileChangesBaseRevision = BaseRev2,
                                    profileChanges = ApplyProfileChanges,
                                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                                }
                            },
                            responseVersion = 1
                        };

                        string mcpJson = JsonConvert.SerializeObject(mcp, Formatting.Indented);
                        Console.WriteLine(mcpJson);

                        return mcp;
                    }
                }
            }

            return new Mcp();
        }
    }
}
