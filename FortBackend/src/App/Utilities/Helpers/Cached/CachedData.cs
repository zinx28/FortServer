using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.App.Utilities.Quests;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.ConfigHelpers;
using FortLibrary.EpicResponses.Profile.Query.Items;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Helpers.Cached
{
    public class CachedData
    {
        public static async Task Init()
        {
            // -- All Paths -- //

            var FortConfigPath = PathConstants.CachedPaths.FortConfig;
            var FortGamePath = PathConstants.CachedPaths.FortGame;
            var FullLockerPath = PathConstants.CachedPaths.FullLocker;
            var DefaultBannerPath = PathConstants.CachedPaths.DefaultBanner;
            var DefaultBannerColorsPath = PathConstants.CachedPaths.DefaultBannerColors;

            // -- //

            // -- Verify -- //

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

            // -- //

            FortConfig DeserializeConfig = Saved.Saved.DeserializeConfig;
            CachedDataClass BackendCachedData = Saved.Saved.BackendCachedData;
            FortGameConfig DeserializeGameConfig = Saved.Saved.DeserializeGameConfig;


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
                if (DeserializeConfig.ForceSeason)
                {
                    Logger.Log($"Force Season Is On [{DeserializeConfig.Season}]", "FortConfig");
                }

                Logger.Log("Loaded Config", "FortConfig");
            }

            var ReadFortGameConfig = File.ReadAllText(FortGamePath);
            if (string.IsNullOrEmpty(ReadFortGameConfig)) { throw new Exception("Error reading ReadFortGameConfig"); }


            DeserializeGameConfig = JsonConvert.DeserializeObject<FortGameConfig>(ReadFortConfig)!;
            if (DeserializeGameConfig == null)
            {
                Logger.Error("Couldn't deserialize config", "FortGameConfig");
                throw new Exception("FortGameConfig: Couldn't deserialize config");
            }
            else
            {
                Logger.Log("Loaded Config", "FortGameConfig");
            }


            string FullLockerJson = File.ReadAllText(FullLockerPath);
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

            string DefaultBanners = File.ReadAllText(DefaultBannerPath);
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


            string DefaultColors = File.ReadAllText(DefaultBannerColorsPath);
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
                BattlepassManager.Init();
            }
            catch (Exception ex) { Logger.Error("Battlepass Data -> " + ex.Message); }

            try
            {
                NewsManager.Init();
            }
            catch (Exception ex) { Logger.Error("NewsManager Data -> " + ex.Message); }

            Saved.Saved.DeserializeConfig = DeserializeConfig;
            Saved.Saved.DeserializeGameConfig = DeserializeGameConfig;
        }
    }
}
