using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Storefront;
using FortBackend.src.App.Utilities.Shop.Helpers.Class;
using FortBackend.src.App.Utilities.Shop.Helpers.Data;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FortBackend.src.App.Routes.APIS.Storefront
{
    [ApiController]
    [Route("fortnite/api/storefront/v2/catalog")]
    public class CatalogController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Catalog> Catalog()
        {
            Response.ContentType = "application/json";
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/shop/shop.json");
                string json = System.IO.File.ReadAllText(filePath);

                if(string.IsNullOrEmpty(json))
                {
                    Logger.Error("Catalog is null -> weird issue");
                    return Ok(new Catalog());
                }
                ShopJson shopData = JsonConvert.DeserializeObject<ShopJson>(json);

                //string filePath1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/temp.json");
                //string json1 = System.IO.File.ReadAllText(filePath1);
                //Catalog shopData1 = JsonConvert.DeserializeObject<Catalog>(json1);
                //return Content(json1);    
                if (shopData == null)
                {
                    Logger.Error("shopData is null -> weird issue");
                    return Ok(new Catalog());
                }

                Catalog ShopObject = new Catalog
                {
                    refreshIntervalHrs = 1,
                    dailyPurchaseHrs = 24,
                    expiration = $"{shopData.expiration}",
                    storefronts = new List<dynamic> {
                        new
                        {
                            name = "BRDailyStorefront",
                            catalogEntries = new List<dynamic>()
                        },
                        new
                        {
                            name = "BRWeeklyStorefront",
                            catalogEntries = new List<dynamic>()
                        }
                    }
                };
               
                // NEED A RECODE RN
                int SortPriority = 20;
                int LargeSortPriority = -10;

                foreach (var WeeklyItems in shopData.ShopItems.Weekly)
                {
                  
                    SortPriority += 1;
                    LargeSortPriority += 1;
                    List<object> requirements = new List<object>();
                    List<object> itemGrants = new List<object>();
                    var DisplayAsset = $"DA_Featured_{WeeklyItems.name}";
                    if(!string.IsNullOrEmpty(WeeklyItems.BundlePath))
                    {
                        DisplayAsset = WeeklyItems.BundlePath;
                    }
                    if (WeeklyItems.item == null || WeeklyItems.item == "")
                    {
                        if (WeeklyItems.name.ToString().ToLower().Contains("bundle"))
                        {
                            foreach (dynamic d in WeeklyItems.items)
                            {
                                itemGrants.Add(new
                                {
                                    templateId = d.item,
                                    quantity = 1
                                });
                                requirements.Add(new
                                {
                                    requirementType = "DenyOnItemOwnership",
                                    requiredId = d.item,
                                    minQuantity = 1,
                                });
                            }
                        }
                        else
                        {
                            Console.WriteLine("ADDED");
                        }
                    }
                    else
                    {

                        itemGrants.Add(new
                        {
                            templateId = $"{WeeklyItems.item}",
                            quantity = 1
                        });
                        requirements.Add(new
                        {
                            requirementType = "DenyOnItemOwnership",
                            requiredId = WeeklyItems.item,
                            minQuantity = 1,
                        });

                        foreach (dynamic d in WeeklyItems.items)
                        {
                            itemGrants.Add(new
                            {
                                templateId = d.item,
                                quantity = 1
                            });
                            requirements.Add(new
                            {
                                requirementType = "DenyOnItemOwnership",
                                requiredId = d.item,
                                minQuantity = 1,
                            });
                        }

                        var shockedwow = new
                        {
                            devName = $"{WeeklyItems.item}",
                            offerId = $"v2:/{WeeklyItems.id}",
                            fulfillmentIds = new List<string>(),
                            dailyLimit = -1,
                            weeklyLimit = -1,
                            monthlyLimit = -1,
                            categories = WeeklyItems.categories,
                            prices = new List<dynamic>
                            {
                                new
                                {
                                    currencyType = "MtxCurrency",
                                    currencySubType = "",
                                    regularPrice = int.Parse(WeeklyItems.normalprice.ToString() ?? "4343434343"),
                                    finalPrice = int.Parse(WeeklyItems.price.ToString() ?? "4343434343"),
                                    saleExpiration = DateTime.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    basePrice = int.Parse(WeeklyItems.price.ToString() ?? "4343434343"),
                                }
                            },
                            matchFilter = "",
                            filterWeight = 0,
                            appStoreId = new List<string>(),
                            requirements = requirements,
                            offerType = "StaticPrice",
                            giftInfo = new
                            {
                                bIsEnabled = true,
                                forcedGiftBoxTemplateId = "",
                                purchaseRequirements = new List<dynamic>(),
                                giftRecordIds = new List<dynamic>()
                            },
                            refundable = true,
                            metaInfo = new List<object>()
                            {
                                new
                                {
                                    key = "SectionId",
                                    value = "Featured"
                                },
                                new
                                {
                                    key = "TileSize",
                                    value = WeeklyItems.type
                                },
                                new
                                {
                                    key = "sectionPriority",
                                    value = WeeklyItems.type == "Normal" ? LargeSortPriority.ToString() : SortPriority.ToString()
                                }
                            },
                            displayAssetPath = $"/Game/Catalog/DisplayAssets/{DisplayAsset}.{DisplayAsset}",
                            itemGrants = itemGrants,
                            sortPriority = WeeklyItems.type == "Normal" ? LargeSortPriority : SortPriority,
                            // catalogGroup = "",
                            catalogGroupPriority = 0,//WeeklyItems.type == "Normal" ? LargeSortPriority : SortPriority
                        };
                        //Console.WriteLine("TEST");

                        ShopObject.storefronts[1].catalogEntries.Add(shockedwow);
                        //((List<dynamic>)ShopObject.storefronts[1].catalogEntries).Add(shockedwow);
                    }
                }

                foreach (var WeeklyItems in shopData.ShopItems.Daily)
                {
                    SortPriority += 1;
                    LargeSortPriority += 1;
                    List<object> requirements = new List<object>();
                    List<object> itemGrants = new List<object>();

                    var DisplayAsset = $"DA_Daily_{WeeklyItems.name}";
                    if (!string.IsNullOrEmpty(WeeklyItems.BundlePath))
                    {
                        DisplayAsset = WeeklyItems.BundlePath;
                    }

                    if (WeeklyItems.item == null || WeeklyItems.item == "")
                    {
                        if (WeeklyItems.name.ToString().ToLower().Contains("bundle"))
                        {
                            foreach (dynamic d in WeeklyItems.items)
                            {
                                itemGrants.Add(new
                                {
                                    templateId = d.item,
                                    quantity = 1
                                });
                                requirements.Add(new
                                {
                                    requirementType = "DenyOnItemOwnership",
                                    requiredId = d.item,
                                    minQuantity = 1,
                                });
                            }
                        }
                        else
                        {
                            Console.WriteLine("ADDED");
                        }
                    }
                    else
                    {

                        itemGrants.Add(new
                        {
                            templateId = $"{WeeklyItems.item}",
                            quantity = 1
                        });
                        requirements.Add(new
                        {
                            requirementType = "DenyOnItemOwnership",
                            requiredId = WeeklyItems.item,
                            minQuantity = 1,
                        });

                        foreach (dynamic d in WeeklyItems.items)
                        {
                            itemGrants.Add(new
                            {
                                templateId = d.item,
                                quantity = 1
                            });
                            requirements.Add(new
                            {
                                requirementType = "DenyOnItemOwnership",
                                requiredId = d.item,
                                minQuantity = 1,
                            });
                        }

                        var shockedwow = new
                        {
                            devName = $"{WeeklyItems.item}",
                            offerId = $"v2:/{WeeklyItems.id}",
                            fulfillmentIds = new List<string>(),
                            dailyLimit = -1,
                            weeklyLimit = -1,
                            monthlyLimit = -1,
                            categories = WeeklyItems.categories,
                            prices = new List<dynamic>
                            {
                                new
                                {
                                    currencyType = "MtxCurrency",
                                    currencySubType = "",
                                    regularPrice = int.Parse(WeeklyItems.normalprice.ToString() ?? "4343434343"),
                                    finalPrice = int.Parse(WeeklyItems.price.ToString() ?? "4343434343"),
                                    saleExpiration = DateTime.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    basePrice = int.Parse(WeeklyItems.price.ToString() ?? "4343434343"),
                                }
                            },
                            matchFilter = "",
                            filterWeight = 0,
                            appStoreId = new List<string>(),
                            requirements = requirements,
                            offerType = "StaticPrice",
                            giftInfo = new
                            {
                                bIsEnabled = true,
                                forcedGiftBoxTemplateId = "",
                                purchaseRequirements = new List<dynamic>(),
                                giftRecordIds = new List<dynamic>()
                            },
                            refundable = true,
                            metaInfo = new List<object>()
                            {
                                new
                                {
                                    key = "SectionId",
                                    value = "Daily"
                                },
                                new
                                {
                                    key = "TileSize",
                                    value = WeeklyItems.type
                                },
                                new
                                {
                                    key = "sectionPriority",
                                    value = WeeklyItems.type == "Normal" ? LargeSortPriority.ToString() : SortPriority.ToString()
                                }
                            },
                            displayAssetPath = $"/Game/Catalog/DisplayAssets/{DisplayAsset}.{DisplayAsset}",
                            itemGrants = itemGrants,
                            sortPriority = WeeklyItems.type == "Normal" ? LargeSortPriority : SortPriority,
                            // catalogGroup = "",
                            catalogGroupPriority = 0,//WeeklyItems.type == "Normal" ? LargeSortPriority : SortPriority

                        };

                        ShopObject.storefronts[0].catalogEntries.Add(shockedwow);
                    }
                }

                return Ok(ShopObject);
            }
            catch (Exception ex)
            {
                Logger.Error($"CatalogShop -> {ex.Message}");
            }

            return Ok(new Catalog());
        }
    }
}
