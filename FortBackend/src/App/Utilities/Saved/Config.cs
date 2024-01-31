using System.Text.Json.Serialization;

namespace FortBackend.src.App.Utilities.Saved
{

    // Since we parse the config this basicalyl needs to be 1:1 else it be dynamic huh
    public class Config
    {
        [JsonPropertyName("//")] // just bc yeah hahah
        public string Uh { get; set; }

        public int BackendPort { get; set; } = 1111; // 1111 default ig
        
        // more stuff in the future when i add ofc
    }
}
