using FortLibrary.ConfigHelpers;
using FortLibrary.EpicResponses.Profile.Query.Items;
using System.Text.Json.Serialization;

namespace FortBackend.src.App.Utilities.Saved
{
    public class Saved
    {
        public static FortConfig DeserializeConfig = new FortConfig();
        public static FortGameConfig DeserializeGameConfig = new FortGameConfig();
    }

    // Moved Config to the library.. works the same though- i just feel like it's better
    // FortLibrary/ConfigHelpers/FortConfig.cs
}
