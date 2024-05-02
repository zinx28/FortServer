using FortLibrary.ConfigHelpers;
using FortLibrary.EpicResponses.Profile.Query.Items;
using System.Text.Json.Serialization;

namespace FortBackend.src.App.Utilities.Saved
{
    public class Saved
    {
        public static Config DeserializeConfig = new Config();
    }

    // Moved Config to the library.. works the same though- i just feel like it's better
    // FortLibrary/ConfigHelpers/FortConfig.cs
}
