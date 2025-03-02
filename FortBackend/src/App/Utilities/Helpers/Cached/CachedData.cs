﻿using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.App.Utilities.Quests;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.Utilities.Shop;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics;
using FortLibrary.Encoders;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.Shop;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Helpers.Cached
{
    public class CachedData
    {
        public static FortConfig DeserializeConfig
        {
            get => Saved.Saved.DeserializeConfig;
            set => Saved.Saved.DeserializeConfig = value;
        }

        public static CachedDataClass BackendCachedData
        {
            get => Saved.Saved.BackendCachedData;
            set => Saved.Saved.BackendCachedData = value;
        }

        public static FortGameConfig DeserializeGameConfig
        {
            get => Saved.Saved.DeserializeGameConfig;
            set => Saved.Saved.DeserializeGameConfig = value;
        }

        public static async Task Init()
        {
            // -- All Paths -- //

            var FortConfigPath = PathConstants.CachedPaths.FortConfig;
            var FortGamePath = PathConstants.CachedPaths.FortGame;
            var FullLockerPath = PathConstants.CachedPaths.FullLocker;
            var DefaultBannerPath = PathConstants.CachedPaths.DefaultBanner;
            var DefaultBannerColorsPath = PathConstants.CachedPaths.DefaultBannerColors;
            var IniConfigPath = PathConstants.CloudStorage.IniConfig;
            var ShopPath = PathConstants.ShopJson.Shop;
            var OgShopPath = PathConstants.ShopJson.SeasonShop;
            var ShopPricesPath = PathConstants.ShopJson.ShopPrices;
            var ShopBundlesPath = PathConstants.ShopJson.ShopBundles;
            var TimelinePath = PathConstants.Timeline;
            // -- //

            // -- Verify -- //

            if (!File.Exists(ShopPath))
            {
                Logger.Error("Couldn't find Shop Path (shop.json)", "Shop-Path");
                throw new Exception("Couldn't find ShopPrices Path (shop.json)");
            }

            if (!File.Exists(OgShopPath))
            {
                Logger.Error("Couldn't find Og Shop Path (SeasonShop.json)", "OgShop-Path");
                throw new Exception("Couldn't find ShopPrices Path (SeasonShop.json)");
            }

            if (!File.Exists(ShopPricesPath))
            {
                Logger.Error("Couldn't find ShopPrices Path (prices.json)", "ShopPrices-Path");
                throw new Exception("Couldn't find ShopPrices Path (prices.json)");
            }

            if (!File.Exists(ShopBundlesPath))
            {
                Logger.Error("Couldn't find ShopBundles Path (bundles.json)", "ShopBundles-Path");
                throw new Exception("Couldn't find ShopBundles Path (bundles.json)");
            }

            if (!File.Exists(IniConfigPath))
            {
                Logger.Error("Couldn't find IniConfig Path (IniConfig.json)", "IniConfig-Path");
                throw new Exception("Couldn't find IniConfig Path (IniConfig.json)");
            }

            if (!File.Exists(FortConfigPath))
            {
                Logger.Error("Couldn't find FortConfig Path (config.json)", "FortConfig-Path");
                throw new Exception("Couldn't find FortConfig Path (config.json)");
            }

            if (!File.Exists(FortGamePath))
            {
                Logger.Error("Couldn't find FortGameConfig Path (gameconfig.json)", "FortGameConfig-Path");
                throw new Exception("Couldn't find FortGameConfig Path (gameconfig.json)");
            }

            if (!File.Exists(FullLockerPath))
            {
                Logger.Error("Couldn't find FullLocker Path (FullLocker.json)", "FullLocker-Path");
                throw new Exception("Couldn't find FullLocker Path (FullLocker.json)");
            }

            if (!File.Exists(DefaultBannerPath))
            {
                Logger.Error("Couldn't find DefaultBanner Path (DefaultBanners.json)", "DefaultBanner-Path");
                throw new Exception("Couldn't find DefaultBanner Path (DefaultBanners.json)");
            }

            if (!File.Exists(DefaultBannerColorsPath))
            {
                Logger.Error("Couldn't find DefaultBannerColors Path (DefaultColors.json)", "DefaultBannerColors-Path");
                throw new Exception("Couldn't find DefaultBannerColors Path (DefaultColors.json)");
            }

            if (!File.Exists(TimelinePath))
            {
                Logger.Error("Couldn't find DefaultBannerColors Path (DefaultColors.json)", "DefaultBannerColors-Path");
                throw new Exception("Couldn't find DefaultBannerColors Path (DefaultColors.json)");
            }

            // -- //
            var ReadFortConfig = File.ReadAllText(FortConfigPath);
            if (string.IsNullOrEmpty(ReadFortConfig)) { throw new Exception("Error reading ReadFortConfig"); } // well should've thrown a different error

            DeserializeConfig = JsonConvert.DeserializeObject<FortConfig>(ReadFortConfig)!;
            if (DeserializeConfig == null)
            {
                Logger.Error("Couldn't deserialize config", "FortConfig");
                throw new Exception("FortConfig: Couldn't deserialize config");
            }
            else
            {
                Logger.Log("Loaded Config", "FortConfig");
            }

            var ReadFortGameConfig = await File.ReadAllTextAsync(FortGamePath);
            if (string.IsNullOrEmpty(ReadFortGameConfig)) { throw new Exception("Error reading ReadFortGameConfig"); }


            DeserializeGameConfig = JsonConvert.DeserializeObject<FortGameConfig>(ReadFortGameConfig)!;
            if (DeserializeGameConfig == null)
            {
                Logger.Error("Couldn't deserialize config", "FortGameConfig");
                throw new Exception("FortGameConfig: Couldn't deserialize config");
            }
            else
            {
                if (DeserializeGameConfig.ForceSeason)
                {
                    Logger.Log($"Force Season Is On [{DeserializeGameConfig.Season}]", "FortConfig");
                }

                Logger.Log("Loaded Config", "FortGameConfig");
            }

            // -- CHECKS -- //

            try
            {
                Logger.Log("Testing JTW KEY", "CHECKING");
                JWT.GenerateRandomJwtToken(24, DeserializeConfig.JWTKEY);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IDX10653"))  // Checking for specific error message content
                {
                    Logger.Error("The JWT KEY is too small. Please ensure the key size is at least 128 bits.", "CHECKING");
                }else
                {
                    Logger.Error(ex.Message, "CHECKING");
                }
            }
               

            // -- //

            string FullLockerJson = await File.ReadAllTextAsync(FullLockerPath);
            if (string.IsNullOrEmpty(FullLockerJson))
            {
                Logger.Error("FULL LOCKER JSON IS NULL", "FullLockerFile");
                throw new Exception("FULL LOCKER JSON IS NULL");
            }

            try
            {
                BackendCachedData.FullLocker_AthenaItems = JsonConvert.DeserializeObject<Dictionary<string, AthenaItem>>(FullLockerJson)!;
                Logger.Log("Full locker is loaded", "Services");
            }
            catch (Exception ex) { Logger.Error("FULL LOCKER -> " + ex.Message); }

            string DefaultBanners = await File.ReadAllTextAsync(DefaultBannerPath);
            if (string.IsNullOrEmpty(DefaultBanners))
            {
                Logger.Error("DefaultBanners JSON IS NULL", "DefaultBannersFile");
                throw new Exception("DefaultBanners JSON IS NULL");
            }

            try
            {
                BackendCachedData.DefaultBanners_Items = JsonConvert.DeserializeObject<Dictionary<string, CommonCoreItem>>(DefaultBanners)!;
                Logger.Log("DefaultBanners is loaded", "Services");
            }
            catch (Exception ex) { Logger.Error("DefaultBanners -> " + ex.Message); }


            string DefaultColors = await File.ReadAllTextAsync(DefaultBannerColorsPath);
            if (string.IsNullOrEmpty(DefaultColors))
            {
                Logger.Error("DefaultColors JSON IS NULL", "DefaultColorsFile");
                throw new Exception("DefaultColors JSON IS NULL");
            }

            try
            {
                Dictionary<string, CommonCoreItem> DefaultColorsData = JsonConvert.DeserializeObject<Dictionary<string, CommonCoreItem>>(DefaultColors)!;
                foreach (var item in DefaultColorsData)
                {
                    BackendCachedData.DefaultBanners_Items.Add(item.Key, item.Value);
                }
                Logger.Log("DefaultColors is loaded", "Services");
            }
            catch (Exception ex) { Logger.Error("DefaultColors -> " + ex.Message); }

            try
            {
                DailyQuestsManager.LoadDailyQuests();
            }
            catch (Exception ex) { Logger.Error("DailyQuests -> " + ex.Message); }

            try
            {
                WeeklyQuestManager.LoadAllWeeklyQuest();
            }
            catch (Exception ex) { Logger.Error("WeeklyQuests -> " + ex.Message); }



            //WeeklyQuestManager

            try
            {
                BattlepassManager.Init();
            }
            catch (Exception ex) { Logger.Error("Battlepass Data -> " + ex.Message); }

            try
            {
                NewsManager.Init();
            }
            catch (Exception ex) { Logger.Error("NewsManager Data -> " + ex.Message); }

            try
            {
                CupCache.Init();
            }
            catch (Exception ex) { Logger.Error("CupCache Data -> " + ex.Message); }

            try
            {
                string filePath = await File.ReadAllTextAsync(IniConfigPath);

                IniManager.IniConfigData = JsonConvert.DeserializeObject<IniConfig>(filePath)!;
            }
            catch (Exception ex) { Logger.Error("IniConfig Data -> " + ex.Message); }


            try
            {
                string filePath = await File.ReadAllTextAsync(TimelinePath);

                BackendCachedData.TimelineData = JsonConvert.DeserializeObject<Timeline>(filePath)!;
            }
            catch (Exception ex) { Logger.Error("Timeline Data -> " + ex.Message); }



            try
            {
                string filePath = await File.ReadAllTextAsync(ShopPricesPath);

                BackendCachedData.ShopPrices = JsonConvert.DeserializeObject<ItemPricing>(filePath)!;
            }
            catch (Exception ex) { Logger.Error("ItemPricing Data -> " + ex.Message); }

            try
            {
                string json = await File.ReadAllTextAsync(ShopPath);
                string Ogjson = await File.ReadAllTextAsync(OgShopPath);

                BackendCachedData.CurrentShop = JsonConvert.DeserializeObject<ShopJson>(json)!;
                BackendCachedData.OGShop = JsonConvert.DeserializeObject<List<ItemsSaved>>(Ogjson)!;
            } catch (Exception ex) { Logger.Error("LOADING SHOP FILES DATA -> " + ex.Message); }

            try
            {
                string filePath = await File.ReadAllTextAsync(ShopBundlesPath);

                BackendCachedData.ShopBundles = JsonConvert.DeserializeObject<List<ShopBundles>>(filePath)!; ;
                BackendCachedData.ShopBundlesFiltered = JsonConvert.DeserializeObject<List<ShopBundles>>(filePath)!; ;

                LoadShopCache.LoadAndFilterShopBundles(DeserializeGameConfig.Season);
                LoadShopCache.LoadItems(DeserializeGameConfig.Season);
            }
            catch (Exception ex) { Logger.Error("ShopBundles Data -> " + ex.Message); }
        }
    }
}
 