using FortLibrary;

namespace FortBackend.src.App.Utilities.Constants
{
    public class PathConstants
    {
        public static readonly string BaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        public static readonly string LocalAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Saved.Saved.DeserializeConfig.ProjectName);

        public static readonly string CacheDir = Path.Combine(BaseDir, "Cache");
        public static readonly string ImageDir = Path.Combine(BaseDir, "Image");
        public static readonly string IniDir = Path.Combine(BaseDir, "Ini");
        public static readonly string JsonDir = Path.Combine(BaseDir, "Json");
        public static readonly string ClientSettingsDir = Path.Combine(LocalAppData, "ClientSettings");

        public class ShopJson
        {
            public static readonly string Shop = Path.Combine(BaseDir, "Json/shop/shop.json");
            public static readonly string ShopPrices = Path.Combine(BaseDir, "Json/shop/prices.json");
            public static readonly string ShopBundles = Path.Combine(BaseDir, "Json/shop/bundles.json");
            public static readonly string SeasonShop = Path.Combine(BaseDir, "Json/shop/special/SeasonShop.json");

            public static readonly string ShopSkins = Path.Combine(BaseDir, "Json/shop/skins.json");
            public static readonly string ShopEmotes = Path.Combine(BaseDir, "Json/shop/emotes.json");
            public static readonly string ShopGliders = Path.Combine(BaseDir, "Json/shop/gliders.json");
            public static readonly string ShopPickaxe = Path.Combine(BaseDir, "Json/shop/pickaxes.json");
            public static readonly string ShopWrap = Path.Combine(BaseDir, "Json/shop/wrap.json");

            public static readonly string ChristmasItems = Path.Combine(BaseDir, "Json/shop/christmas/christmas.json");
        }

        public class Templates
        {
            public static readonly string Events = Path.Combine(BaseDir, "Json/templates/Events.json");
            public static readonly string Arena = Path.Combine(BaseDir, "Json/templates/Arena.json");
            public static readonly string Score = Path.Combine(BaseDir, "Json/templates/Score.json");
        }

        public class CloudDir
        {
            public static readonly string CloudDire = Path.Combine(BaseDir, "Ini/CloudDir");
          
            public static string chunk(string chunk)
            {
                return Path.Combine(CloudDire, chunk);
            }

          

            public static readonly string FullCloud = Path.Combine(CloudDire, "Full.ini");
        }

        public class CloudStorage
        {
            public static readonly string IniConfig = Path.Combine(IniDir, "IniConfig.json");
            public static readonly string PlaylistData = Path.Combine(IniDir, "PlaylistData.json");
            public static readonly string QosRegionManager = Path.Combine(IniDir, "QosRegionManager.json");
            //QosRegionManager.json
        }

        public class CachedPaths
        {
            public static readonly string FortConfig = Path.Combine(BaseDir, "Config.json");
            public static readonly string FortGame = Path.Combine(BaseDir, "GameConfig.json");
            public static readonly string FullLocker = Path.Combine(BaseDir, "Json/Profiles/FullLocker.json");
            public static readonly string DefaultBanner = Path.Combine(BaseDir, "Json/Profiles/Banners/DefaultBanners.json");
            public static readonly string DefaultBannerColors = Path.Combine(BaseDir, "Json/Profiles/Banners/DefaultColors.json");

        }

        public static readonly string EulaTrackingFN = Path.Combine(JsonDir, "EulaTrackingFN.json");
        public static readonly string Keychain = Path.Combine(JsonDir, "keychain.json");
        public static readonly string FN_PROD = Path.Combine(JsonDir, "FN_PROD.json");
        public static readonly string SdkDefault = Path.Combine(JsonDir, "SdkDefault.json");
        public static readonly string EpicSettings = Path.Combine(JsonDir, "epicsettings.json");
        public static readonly string Content = Path.Combine(JsonDir, "content.json");
        public static readonly string SparkTracks = Path.Combine(JsonDir, "spark-tracks.json");
        public static readonly string Discovery = Path.Combine(JsonDir, "discovery.json");
        public static readonly string DiscoveryAssets = Path.Combine(JsonDir, "discoveryassets.json");
        public static readonly string Timeline = Path.Combine(JsonDir, "Timeline.json");


        public static string CloudSettings(string ClientId)
        {
            if (!Directory.Exists(ClientSettingsDir))
            {
                Directory.CreateDirectory(ClientSettingsDir);
            }

            return Path.Combine(ClientSettingsDir, ClientId);
        }
        public static string CacheData(string filepath)
        {
            if (!Directory.Exists(CacheDir))
            {
                Directory.CreateDirectory(CacheDir);
            }

            return Path.Combine(CacheDir, filepath);
        }


        public static string ReturnImage(string Image)
        {
            var Response = Path.Combine(ImageDir, Image);
            //Logger.Log(Response);
            if (!System.IO.File.Exists(Response))
            {
                Logger.Error("Couldn't find image " + Image, "ReturnImage");
                Response = Path.Combine(ImageDir, "Trans_Boykisser.png"); // i'll change this in the future :3
            }

            return Response;
        }
    }
}
