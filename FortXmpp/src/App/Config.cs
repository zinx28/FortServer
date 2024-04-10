using System.Text.Json.Serialization;

namespace FortXmpp.src.App
{
    public class Saved
    {
        public static Config DeserializeConfig = new Config();
    }
    public class Config
    {
        [JsonPropertyName("//")] // just bc yeah hahah
        public string FortXmpp { get; set; } = "";

        // BACKEND WILL AUTO CHANGE THE PROTOCOL
        public string DefaultProtocol { get; set; } = "http://";

        public int BackendPort { get; set; } = 1111; // default backend config
        // NEW XMPP
        public int XmppPort { get; set; } = 443; // 443 default ig

        // MongoDBConnectionString is set to the default local host port for mongodbserver you may change this to your needs!
        // Make sure that the string has read/write perms (owner or what)
        public string MongoDBConnectionString { get; set; } = "mongodb://localhost:27017/?retryWrites=true&w=majority";

        // MongoDBConnectionName by default set to FortBackend this just creates the database "FortBackend"
        public string MongoDBConnectionName { get; set; } = "FortBackend"; // KEEP AS YOUR BACKEND CONNECTION NAME


        // THIS IS FOR LUNA TESTING ONLY.... ENABLING WILL BREAK YOUR THE BACKEND
        public bool LunaPROD { get; set; } = false;
    }
}
