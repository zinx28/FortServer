using System.Text.Json.Serialization;

namespace FortBackend.src.App.Utilities.Saved
{
    public class Saved
    {
        public static Config DeserializeConfig;
    }
    // Since we parse the config this basicalyl needs to be 1:1 else it be dynamic huh
    public class Config
    {
        [JsonPropertyName("//")] // just bc yeah hahah
        public string Uh { get; set; } = "";

        public int BackendPort { get; set; } = 1111; // 1111 default ig

        public int XmppPort { get; set; } = 443; // 443 default ig

        // MongoDBConnectionString is set to the default local host port for mongodbserver you may change this to your needs!
        // Make sure that the string has read and write perms (owner or what)
        public string MongoDBConnectionString { get; set; } = "mongodb://localhost:27017/?retryWrites=true&w=majority";

        // MongoDBConnectionName by default set to FortBackend this just creates the database "FortBackend"
        public string MongoDBConnectionName { get; set; } = "FortBackend"; // 

        public string DiscordToken { get; set; } = "";

        public ulong ServerID { get; set; } = 0;

        public string ApplicationClientID = "";
        public string ApplicationSecret = "";
        public string ApplicationURI = "";

        public string ShopWebhookUrl = "";
        // more stuff in the future when i add ofc
    }
}
