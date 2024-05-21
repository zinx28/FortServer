using System.Text.Json.Serialization;
using FortLibrary.ConfigHelpers;
using FortLibrary.EpicResponses.Profile.Query.Items;

namespace FortMatchmaker.src.App.Utilities
{
    public class Saved
    {
        public static FortConfig DeserializeConfig = new FortConfig();
        public static CachedDataClass BackendCachedData = new CachedDataClass();
    }

    public class CachedDataClass
    {
        // AUTO CHANGES DURING RUN TIME
        public string DefaultProtocol { get; set; } = "http://";
    }
}
