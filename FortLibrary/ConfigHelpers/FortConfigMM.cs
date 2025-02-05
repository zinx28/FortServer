using FortLibrary.EpicResponses.Profile.Query.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FortLibrary.ConfigHelpers
{
    // Since we parse the config this basicalyl needs to be 1:1 else it be dynamic huh

    // I will work on something in the future to show how to set stuff up
    public class FortConfigMM
    {
        [JsonPropertyName("//")]
        [JsonProperty("//")]
        public string FortBackend { get; set; } = "";


        // ** //

        public string ProjectName { get; set; } = "FortBackend"; // clientsettings, and many other things that include fortbackend

        [JsonPropertyName("JWT-KEY")]
        [JsonProperty("JWT-KEY")]
        public string JWTKEY { get; set; } = "MyVerySecretFortBackendKey@!!!!!!!!";
        public int BackendPort { get; set; } = 1111;
        public int MatchmakerPort { get; set; } = 2121;
        public int AdminPort { get; set; } = 2134;
        public string MatchmakerIP { get; set; } = "127.0.0.1"; // Change if your hosting the matchmaker on a different ip

        // MongoDBConnectionString is set to the default local host port for mongodbserver you may change this to your needs!
        // Make sure that the string has read/write perms (owner or what)
        public string MongoDBConnectionString { get; set; } = "mongodb://localhost:27017/?retryWrites=true&w=majority";

        // MongoDBConnectionName by default set to FortBackend this just creates the database "FortBackend"
        public string MongoDBConnectionName { get; set; } = "FortBackend"; // 

        // - //

        public bool HTTPS { get; set; } = false;

        // ah

        public string FortHosterKey { get; set; } = "FortHosterSecretKey!!@!@!";
    }
}
