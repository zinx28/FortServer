using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FortHoster.src.Classes
{
    public class ConfigC
    {
        [JsonPropertyName("//")]
        [JsonProperty("//")]
        public string FortBackend { get; set; } = "";

        //EU, NAE.... ykyk
        public string Region { get; set; } = "EU";
        // best way for these kids who uses reboot for everything ;(
        public string Playlist { get; set; } = "Playlist_DefaultSolo";

        // this should be random
        public string Key { get; set; } = "FortHosterSecretKey!!@!@!";

        public bool HTTPS { get; set; } = false;
        public bool Headless { get; set; } = true;
        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 2121;
        public bool GameLogs { get; set; } = false; // i dont recommend if you use two servers
        public string Email { get; set; } = string.Empty;   
        public string Password { get; set; } = string.Empty;
        public double Season { get; set; } = 10.40; // this be usefull in the future
        public string GamePath { get; set; } = "";
        public string RedirectDLL { get; set; } = "";
        public string GameServerDLL { get; set; } = "";
        public string GameServerIP { get; set; } = "127.0.0.1";
        public int GameServerPort { get; set; } = 7777;
    }

    public class Saved
    {
        public static ConfigC ConfigC = new ConfigC();
        public static List<Servers> servers = new();
    }
}
