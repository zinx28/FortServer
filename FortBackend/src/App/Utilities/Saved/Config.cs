using FortBackend.src.App.Utilities.ADMIN;
using FortLibrary.ConfigHelpers;
using FortLibrary.EpicResponses.Profile.Query.Items;
using System.Text.Json.Serialization;

namespace FortBackend.src.App.Utilities.Saved
{
    public class Saved
    {
        public static CachedAdminData CachedAdminData = new CachedAdminData();
        public static CachedDataClass BackendCachedData = new CachedDataClass();
        public static FortConfig DeserializeConfig = new FortConfig();
        public static FortGameConfig DeserializeGameConfig = new FortGameConfig();
    }

    public class CachedDataClass
    {
        // AUTO CHANGES DURING RUN TIME
        public string DefaultProtocol { get; set; } = "http://";
        public Dictionary<string, AthenaItem> FullLocker_AthenaItems { get; set; } = new Dictionary<string, AthenaItem>();
        public Dictionary<string, CommonCoreItem> DefaultBanners_Items { get; set; } = new Dictionary<string, CommonCoreItem>();


    }
    // Moved Config to the library.. works the same though- i just feel like it's better
    // FortLibrary/ConfigHelpers/FortConfig.cs
}
