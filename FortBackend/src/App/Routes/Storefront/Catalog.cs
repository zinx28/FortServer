using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Storefront;
using FortBackend.src.App.Utilities.Shop.Helpers.Class;
using FortBackend.src.App.Utilities.Shop.Helpers.Data;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Storefront
{
    [ApiController]
    [Route("fortnite/api/storefront/v2/catalog")]
    public class CatalogController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Catalog>> Catalog()
        {
            Response.ContentType = "application/json";
            try
            {

                VersionClass season = await SeasonUserAgent(Request);
                //if(season.Season == 1)
                //{
                //    return NotFound(new
                //    {
                //        errorCode = "errors.com.epicgames.common.not_found",
                //        errorMessage = "Sorry the resource you were trying to find could not be found",
                //        numericErrorCode = 4002,
                //        originatingService = "Fortnite",
                //        intent = "prod"
                //    });
                //    Console.WriteLine("SHOP HAS BEEN DISABLED FOR LOGIN CRASHES!");
                //    return BadRequest(new
                //    {
                //        errorCode = "errors.com.epicgames.common.not_found",
                //        errorMessage = "Sorry the resource you were trying to find could not be found",
                //        numericErrorCode = 0,
                //        originatingService = "Fortnite",
                //        intent = "prod"
                //    }); // as this is just for now!
                //}
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/shop/shop.json");
                string json = System.IO.File.ReadAllText(filePath);

                if (string.IsNullOrEmpty(json))
                {
                    Logger.Error("Catalog is null -> weird issue");
                    return Ok(new
                    {
                        errorCode = "errors.com.epicgames.common.not_found",
                        errorMessage = "Sorry the resource you were trying to find could not be found",
                        numericErrorCode = 0,
                        originatingService = "Fortnite",
                        intent = "prod"
                    });
                }
                ShopJson shopData = JsonConvert.DeserializeObject<ShopJson>(json);

                //string filePath1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/test.json");
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

                if (season.Season == 1)
                {
                    string SeasonShopPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/shop/special/OgShop.json");
                    string OGJson = System.IO.File.ReadAllText(SeasonShopPath);

                    if (string.IsNullOrEmpty(OGJson))
                    {
                        Logger.Error("SEASON SHOP ISNT FOUND PLEASE MAKE SURE YOU DIDNT DELETE IT");
                        return Ok(new
                        {
                            errorCode = "errors.com.epicgames.common.not_found",
                            errorMessage = "Sorry the resource you were trying to find could not be found",
                            numericErrorCode = 0,
                            originatingService = "Fortnite",
                            intent = "prod"
                        });
                    }
                    List<catalogEntrie> OgShopJson = JsonConvert.DeserializeObject<List<catalogEntrie>>(OGJson);

                    ShopObject.storefronts.Add(new
                    {
                        name = "BRSeasonStorefront",
                        catalogEntries = OgShopJson
                    });

                    return Ok(ShopObject);
                }

                    // NEED A RECODE RN
                    int SortPriority = 20;
                int LargeSortPriority = -10;

                foreach (var WeeklyItems in shopData.ShopItems.Weekly)
                {

                    SortPriority += 1;
                    LargeSortPriority += 1;
                    List<CatalogRequirements> requirements = new List<CatalogRequirements>();
                    List<itemGrants> itemGrants = new List<itemGrants>();
                    //Item itemIg = WeeklyItems.items.FirstOrDefault(item => !string.IsNullOrEmpty(item.description));
                    var DisplayAsset = $"DA_Daily_{WeeklyItems.item}";
                    if (!string.IsNullOrEmpty(WeeklyItems.BundlePath))
                    {
                        DisplayAsset = WeeklyItems.BundlePath;
                    }


                    itemGrants.Add(new itemGrants
                    {
                        templateId = WeeklyItems.item,
                        quantity = 1
                    });
                    requirements.Add(new CatalogRequirements
                    {
                        requirementType = "DenyOnItemOwnership",
                        requiredId = WeeklyItems.item,
                        minQuantity = 1,
                    });

                    foreach (dynamic d in WeeklyItems.items)
                    {

                        itemGrants.Add(new itemGrants
                        {
                            templateId = d.item,
                            quantity = 1
                        });
                        requirements.Add(new CatalogRequirements
                        {
                            requirementType = "DenyOnItemOwnership",
                            requiredId = d.item,
                            minQuantity = 1,
                        });
                    }

                    if (!WeeklyItems.name.ToString().ToLower().Contains("bundle"))
                    {
                        var WeeklyItem = new catalogEntrie
                        {
                            devName = $"{WeeklyItems.id}",
                            offerId = $"v2:/{WeeklyItems.id}",
                            categories = WeeklyItems.categories,
                            prices = new List<CatalogPrices>
                            {
                                new CatalogPrices
                                {
                                    currencyType = "MtxCurrency",
                                    currencySubType = "",
                                    regularPrice = WeeklyItems.singleprice,
                                    finalPrice = WeeklyItems.price,
                                    saleExpiration = DateTime.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    basePrice = WeeklyItems.price,
                                }
                            },
                            requirements = requirements,
                            offerType = "StaticPrice",
                            giftInfo = new
                            {
                                bIsEnabled = true,
                                forcedGiftBoxTemplateId = "",
                                purchaseRequirements = new List<dynamic>(),
                                giftRecordIds = new List<dynamic>()
                            },
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
                            catalogGroupPriority = 0,//WeeklyItems.type == "Normal" ? LargeSortPriority : SortPriority
                        };

                        ShopObject.storefronts[1].catalogEntries.Add(WeeklyItem);
                    }
                }

                foreach (var WeeklyItems in shopData.ShopItems.Daily)
                {
                    SortPriority += 1;
                    LargeSortPriority += 1;
                    List<CatalogRequirements> requirements = new List<CatalogRequirements>();
                    List<itemGrants> itemGrants = new List<itemGrants>();
                    //Item itemIg = WeeklyItems.items.FirstOrDefault(item => !string.IsNullOrEmpty(item.description));
                    var DisplayAsset = $"DA_Daily_{WeeklyItems.item}";
                    if (!string.IsNullOrEmpty(WeeklyItems.BundlePath))
                    {
                        DisplayAsset = WeeklyItems.BundlePath;
                    }


                    itemGrants.Add(new itemGrants
                    {
                        templateId = WeeklyItems.item,
                        quantity = 1
                    });
                    requirements.Add(new CatalogRequirements
                    {
                        requirementType = "DenyOnItemOwnership",
                        requiredId = WeeklyItems.item,
                        minQuantity = 1,
                    });

                    foreach (dynamic d in WeeklyItems.items)
                    {
                        itemGrants.Add(new itemGrants
                        {
                            templateId = d.item,
                            quantity = 1
                        });
                        requirements.Add(new CatalogRequirements
                        {
                            requirementType = "DenyOnItemOwnership",
                            requiredId = d.item,
                            minQuantity = 1,
                        });
                    }

                    if (!WeeklyItems.name.ToString().ToLower().Contains("bundle"))
                    {
                        var WeeklyItem = new catalogEntrie
                        {
                            devName = $"{WeeklyItems.id}",
                            offerId = $"v2:/{WeeklyItems.id}",
                            categories = WeeklyItems.categories,
                            prices = new List<CatalogPrices>
                            {
                                new CatalogPrices
                                {
                                    currencyType = "MtxCurrency",
                                    currencySubType = "",
                                    regularPrice = WeeklyItems.singleprice,
                                    finalPrice = WeeklyItems.price,
                                    saleExpiration = DateTime.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    basePrice = WeeklyItems.price,
                                }
                            },
                            requirements = requirements,
                            offerType = "StaticPrice",
                            giftInfo = new
                            {
                                bIsEnabled = true,
                                forcedGiftBoxTemplateId = "",
                                purchaseRequirements = new List<dynamic>(),
                                giftRecordIds = new List<dynamic>()
                            },
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
                            catalogGroupPriority = 0,//WeeklyItems.type == "Normal" ? LargeSortPriority : SortPriority

                        };

                        ShopObject.storefronts[0].catalogEntries.Add(WeeklyItem);
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
