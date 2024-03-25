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
        public string FortBackend { get; set; } = "";

        // BACKEND WILL AUTO CHANGE THE PROTOCOL
        public string DefaultProtocol { get; set; } = "http://";


        public int BackendPort { get; set; } = 1111; // 1111 default ig

        public int MatchmakerPort { get; set; } = 2121; // 2121 default ig
        public string MatchmakerIP { get; set; } = "127.0.0.1"; // Don't change unless its hosted on a different ip

        // OLD XMPP
        public int TCPXmppPort { get; set; } = 20123;

        // NEW XMPP
        public int XmppPort { get; set; } = 443; // 443 default ig

        // MongoDBConnectionString is set to the default local host port for mongodbserver you may change this to your needs!
        // Make sure that the string has read/write perms (owner or what)
        public string MongoDBConnectionString { get; set; } = "mongodb://localhost:27017/?retryWrites=true&w=majority";

        // MongoDBConnectionName by default set to FortBackend this just creates the database "FortBackend"
        public string MongoDBConnectionName { get; set; } = "FortBackend"; // 


        // ---  YOU MAY ADD THIS TO YOUR CONFIG
        public bool bShowBotMessage { get; set; } = true; // just add this to your config like "bShowBotMessage": false;
        public string DiscordBotMessage { get; set; } = "FortBackend";

        // --- ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        public string DiscordToken { get; set; } = "";
        public ulong ServerID { get; set; } = 0;
        public ulong RoleID { get; set; } = 0; // who ever has this role will be able to use commands
        public string ApplicationClientID { get; set; } = "";
        public string ApplicationSecret { get; set; } = "";
        public string ApplicationURI { get; set; } = "";

        // CURRENTLY SHOP WON'T SEND NOTHING AS ITS A SKUNKY IMAGE THAT ISNT WORKED ON
        public string ShopWebhookUrl { get; set; } = "";

        public string DetectedWebhookUrl { get; set; } = "";
        public bool EnableDetections { get; set; } = true;

        // SEASON YOU'RE HOSTING
        public bool ForceSeason { get; set; } = false;
        public int Season { get; set; } = 0;
    }
}
