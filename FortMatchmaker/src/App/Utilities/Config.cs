using System.Text.Json.Serialization;

namespace FortMatchmaker.src.App.Utilities
{
    public class Saved
    {
        public static Config DeserializeConfig;
    }
    public class Config
    {
        [JsonPropertyName("//")]
        public string FortMatchmaker { get; set; } = "";

        // BACKEND WILL AUTO CHANGE THE PROTOCOL
        public string DefaultProtocol { get; set; } = "http://";
        public int MatchmakerPort { get; set; } = 2121; // 2121 default ig
        public string MatchmakerIP { get; set; } = "127.0.0.1"; // Don't change unless its hosted on a different ip


        // MongoDBConnectionString is set to the default local host port for mongodbserver you may change this to your needs!
        // Make sure that the string has read/write perms (owner or what)
        public string MongoDBConnectionString { get; set; } = "mongodb://localhost:27017/?retryWrites=true&w=majority";

        // MongoDBConnectionName by default set to FortBackend this just creates the database "FortBackend"
        public string MongoDBConnectionName { get; set; } = "FortBackend";

    }
}
