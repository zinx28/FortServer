using FortBackend.src.App.Routes.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Helpers.Cached
{
    public class CupCache
    {
        public static List<CacheCupsData> cacheCupsDatas = new();
        public static void Init()
        {
            var CachePath = PathConstants.CacheData("CupCache.json");
            if (!File.Exists(CachePath))
            {
                File.WriteAllText(CachePath, "[]");
            }

            Logger.Log("Loading CupCache", "CupCache");

            var jsonData = System.IO.File.ReadAllText(CachePath);
            List<CacheCupsData> CacheDataRS = JsonConvert.DeserializeObject<List<CacheCupsData>>(jsonData) ?? new();

            if(CacheDataRS != null)
            {
                cacheCupsDatas = CacheDataRS; // frfr
            }
        }

        // yeah nothing special ahh
        public static void Update()
        {
            if (cacheCupsDatas != null)
            {
                System.IO.File.WriteAllText(PathConstants.CacheData("CupCache.json"), JsonConvert.SerializeObject(cacheCupsDatas));
                Logger.Log("RE-LOADED CupCache");
            }
        }
    }
}
