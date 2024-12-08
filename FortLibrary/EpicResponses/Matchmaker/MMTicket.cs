using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Matchmaker
{
    public class MMTicket
    {
        [JsonPropertyName("id")]
        public string? SessionId { get; set; }

        [JsonPropertyName("ownerId")]
        public string? OwnerId { get; set; }

        [JsonPropertyName("ownerName")]
        public string OwnerName { get; set; } = "FortBackend";

        [JsonPropertyName("serverName")]
        public string ServerName { get; set; } = "FortBackend";

        [JsonPropertyName("serverAddress")]
        public string? ServerAddress { get; set; }

        [JsonPropertyName("serverPort")]
        public int ServerPort { get; set; }


        [JsonPropertyName("maxPublicPlayers")]
        public int MaxPublicPlayers { get; set; } = 100;

        [JsonPropertyName("openPublicPlayers")]
        public int OpenPublicPlayers { get; set; } = 100;

        [JsonPropertyName("maxPrivatePlayers")]
        public int MaxPrivatePlayers { get; set; }

        [JsonPropertyName("openPrivatePlayers")]
        public int OpenPrivatePlayers { get; set; }

        [JsonPropertyName("attributes")]
        public MMTicketAttributes Attributes { get; set; } = new MMTicketAttributes();

        [JsonPropertyName("publicPlayers")]
        public List<string>? PublicPlayers { get; set; } = new List<string>();

        [JsonPropertyName("privatePlayers")]
        public List<string>? PrivatePlayers { get; set; } = new List<string>();

        [JsonPropertyName("totalPlayers")]
        public int TotalPlayers { get; set; } = 0;

        [JsonPropertyName("allowJoinInProgress")]
        public bool AllowJoinInProgress { get; set; }

        [JsonPropertyName("shouldAdvertise")]
        public bool ShouldAdvertise { get; set; }
     
        [JsonPropertyName("isDedicated")]
        public bool IsDedicated { get; set; }

        [JsonPropertyName("usesStats")]
        public bool UsesStats { get; set; }

        [JsonPropertyName("allowInvites")]
        public bool AllowInvites { get; set; }

        [JsonPropertyName("usesPresence")]
        public bool UsesPresence { get; set; }

        [JsonPropertyName("allowJoinViaPresence")]
        public bool AllowJoinViaPresence { get; set; } = true;

        [JsonPropertyName("allowJoinViaPresenceFriendsOnly")]
        public bool AllowJoinViaPresenceFriendsOnly { get; set; }

        [JsonPropertyName("buildUniqueId")]
        public string? BuildUniqueId { get; set; }

        [JsonPropertyName("lastUpdated")]
        public string? LastUpdated { get; set; }

        [JsonPropertyName("started")]
        public bool Started { get; set; }


    }

    public class MMTicketAttributes
    {
        [JsonPropertyName("REGION_s")]
        public string? Region { get; set; } = "EU";

        [JsonPropertyName("GAMEMODE_s")]
        public string? GameMode { get; set; } = "FORTATHENA";

        [JsonPropertyName("ALLOWBROADCASTING_b")]
        public bool AllowBroadcasting { get; set; } = true;


        [JsonPropertyName("SUBREGION_s")]
        public string? SubRegion { get; set; } = "GB";

        [JsonPropertyName("DCID_s")]
        public string? DCID_s { get; set; } = "FORTNITE-LIVEEUGCEC1C2E30UBRCORE0A-49459394";


        [JsonPropertyName("tenant_s")]
        public string? Tenant { get; set; } = "Fortnite";

        [JsonPropertyName("TENANT_s")]
        public string? TenantU { get; set; } = "Fortnite";

        // these are strings (Any, Desktop, PS4, XboxOne, Mobile, Test, Switch, Console, All)
        [JsonPropertyName("MATCHMAKINGPOOL_s")]
        public string? MatchmakingPool { get; set; } = "Any";

        // 0 - 2 (3 is max) 0 is NonSSD
        [JsonPropertyName("STORMSHIELDDEFENSETYPE_i")]
        public int StormShieldDefenseType { get; set; } = 0;

        [JsonPropertyName("HOTFIXVERSION_i")]
        public int HotFixVersion { get; set; } = 0;

        // should be set to the game mode your trying to play
        [JsonPropertyName("PLAYLISTNAME_s")]
        public string? PlaylistName { get; set; } = "Playlist_DefaultSolo";

        [JsonPropertyName("SESSIONKEY_s")]
        public string? SessionKey { get; set; }

        [JsonPropertyName("BEACONPORT_i")]
        public int? BeaconPort { get; set; } = 15009;
    }
}
