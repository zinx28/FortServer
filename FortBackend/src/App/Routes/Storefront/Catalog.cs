using FortBackend.src.App.Utilities;
using FortLibrary.EpicResponses.Storefront;
using FortLibrary.Shop;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary.Dynamics;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.App.Utilities.Constants;
using Microsoft.AspNetCore.Razor.Hosting;
using FortLibrary;
using FortBackend.src.App.Utilities.Saved;

namespace FortBackend.src.App.Routes.Storefront
{
    // The worst code ever needs to be cleaner
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
                var AcceptLanguage = Request.Headers["Accept-Language"].ToString();
                VersionClass season = await SeasonUserAgent(Request);

                //if(Saved.BackendCachedData.CurrentShop.ShopItems.Daily.Count == 0 &&
                //    Saved.BackendCachedData.CurrentShop.ShopItems.Weekly.Count == 0)
                //{
                //    return Ok(new
                //    {
                //        errorCode = "errors.com.epicgames.common.not_found",
                //        errorMessage = "Sorry the resource you were trying to find could not be found",
                //        numericErrorCode = 0,
                //        originatingService = "Fortnite",
                //        intent = "prod"
                //    });
                //}

                if (string.IsNullOrEmpty(AcceptLanguage))
                {
                    AcceptLanguage = "eu"; // weird
                }


                ShopJson shopData = Saved.BackendCachedData.CurrentShop;

                if (shopData == null) // never
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
                    List<ItemsSaved> SeasonShopJson = Saved.BackendCachedData.OGShop;

                    if (SeasonShopJson != null)
                    {
                        List<catalogEntrie> SeasonShopEntrie = new List<catalogEntrie>();

                        foreach (var item in SeasonShopJson) {
                            List<CatalogRequirements> requirements = new List<CatalogRequirements>();
                            List<itemGrants> itemGrants = new List<itemGrants>();

                            itemGrants.Add(new itemGrants
                            {
                                templateId = item.item,
                                quantity = 1
                            });

                            //requirements.Add(new CatalogRequirements
                            //{
                            //    requirementType = "ECatalogRequirementType_MAX",
                            //    requiredId = "level",
                            //    minQuantity = 5,
                            //});

                            SeasonShopEntrie.Add(new catalogEntrie
                            {
                                devName = $"{item.devName}",
                                offerId = $"v2:/{item.id}",
                                categories = item.categories,
                                prices = new List<CatalogPrices>
                                {
                                    new CatalogPrices
                                    {
                                        currencyType = "MtxCurrency",
                                        currencySubType = "",
                                        regularPrice = item.singleprice,
                                        finalPrice = item.price,
                                        saleExpiration = DateTime.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                        basePrice = item.price,
                                    }
                                },
                                requirements = requirements,
                                offerType = "StaticPrice",
                                giftInfo = new {},
                                metaInfo = item.metaInfo,
                                displayAssetPath = item.displayAssetPath,
                                itemGrants = itemGrants,
                                sortPriority = item.sortPriority,
                                catalogGroupPriority = item.catalogGroupPriority,
                            }); 
                            
                        }

                        ShopObject.storefronts.Add(new
                        {
                            name = "BRSeasonStorefront",
                            catalogEntries = SeasonShopEntrie
                        });
                    }

                    return Ok(ShopObject);
                }

                if(season.Season == 2)
                {
               //     return Ok(ShopObject);
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
                    var DisplayAsset = $"DA_Featured_{WeeklyItems.item}";
                    if (!string.IsNullOrEmpty(WeeklyItems.BundlePath))
                    {
                        DisplayAsset = WeeklyItems.BundlePath;
                    }
                    else {
                        var Items = WeeklyItems.item.Split(':');

                        if(Items.Length > 0)
                        {
                            DisplayAsset = $"DA_Featured_{Items[1]}";
                        }
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
                            metaInfo = new List<MetaInfo>()
                            {
                                new MetaInfo
                                {
                                    key = "SectionId",
                                    value = "Featured"
                                },
                                new MetaInfo
                                {
                                    key = "TileSize",
                                    value = WeeklyItems.type
                                },
                                new MetaInfo
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
                            metaInfo = new List<MetaInfo>()
                            {
                                new MetaInfo
                                {
                                    key = "SectionId",
                                    value = "Daily"
                                },
                                new MetaInfo
                                {
                                    key = "TileSize",
                                    value = WeeklyItems.type
                                },
                                new MetaInfo
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

                StoreBattlepassPages battlepass = BattlepassManager.BattlePasses.FirstOrDefault(e => e.Key == season.Season).Value;

                if(battlepass != null)
                {
                    List<object> responseobject = new List<object>();
                    foreach (catalogEntrieStore a in battlepass.catalogEntries)
                    {
                        List<object> array = new List<object>();

                        if (a.prices[0].saleType != null)
                        {
                            array.Add(new
                            {
                                currencyType = a.prices[0].currencyType,
                                currencySubType = a.prices[0].currencySubType,
                                regularPrice = a.prices[0].regularPrice,
                                finalPrice = a.prices[0].finalPrice,
                                saleType = a.prices[0].saleType,
                                saleExpiration = a.prices[0].saleExpiration,
                                basePrice = a.prices[0].basePrice,
                            });
                        }
                        else
                        {

                            array.Add(new
                            {
                                currencyType = a.prices[0].currencyType,
                                currencySubType = a.prices[0].currencySubType,
                                regularPrice = a.prices[0].regularPrice,
                                finalPrice = a.prices[0].finalPrice,
                                //saleType = a.prices[0].saleType,
                                saleExpiration = a.prices[0].saleExpiration,
                                basePrice = a.prices[0].basePrice,
                            });
                        }

                        //Console.WriteLine(a.GetLanguage(a.title, AcceptLanguage));

                        var ResponseItem = new
                        {
                            devName = $"{a.devName}",
                            offerId = $"{a.offerId}",
                            offerType = "StaticPrice",
                            prices = array,
                            categories = a.categories,
                            dailyLimit = a.dailyLimit,
                            weeklyLimit = a.weeklyLimit,
                            monthlyLimit = a.monthlyLimit,
                            refundable = a.refundable,
                            appStoreId = a.appStoreId,
                            requirements = a.requirements,
                            giftInfo = a.giftInfo,
                            metaInfo = a.metaInfo,
                            displayAssetPath = a.displayAssetPath,
                            itemGrants = a.itemGrants,
                            sortPriority = a.sortPriority,
                            catalogGroupPriority = 0,
                            title = a.GetLanguage(a.title, AcceptLanguage),
                            shortDescription = a.GetLanguage(a.shortDescription, AcceptLanguage),
                            description = a.GetLanguage(a.description, AcceptLanguage)
                        };

                        responseobject.Add(ResponseItem);
                    }

                    var SeasonBattlepassinfo = new
                    {
                        name = battlepass.name,
                        catalogEntries = responseobject
                    };
                    ShopObject.storefronts.Add(SeasonBattlepassinfo);
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
