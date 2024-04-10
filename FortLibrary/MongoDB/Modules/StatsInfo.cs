using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FortLibrary.MongoDB.Module
{
    [BsonIgnoreExtraElements]
    public class StatsInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("accountId")]
        public string AccountId { get; set; } = string.Empty;

        [BsonElement("DiscordId")]
        public string DiscordId { get; set; } = string.Empty;

        [BsonElement("gamemodes")]
        public List<GamemodeStatsData> Gamemodes { get; set; } = new List<GamemodeStatsData>();


        [BsonElement("stats")]
        public Dictionary<string, int> stats { get; set; } = new Dictionary<string, int>();

        [BsonElement("wins")]
        public int Wins { get; set; } = 0;

        [BsonElement("kills")]
        public int Kills { get; set; } = 0;

        [BsonElement("matchplayed")]
        public int MatchesPlayed { get; set; } = 0;
    }

    public class GamemodeStatsData
    {
        [BsonElement("gamemode")]
        public string Gamemode { get; set; } = string.Empty;
        [BsonElement("stats")]
        public StatsData Stats { get; set; } = new StatsData();
    }

    public class StatsData
    {
        [BsonElement("wins")]
        public int Wins { get; set; } = 0;

        [BsonElement("kills")]
        public int Kills { get; set; } = 0;

        [BsonElement("matchplayed")]
        public int MatchesPlayed { get; set; } = 0;
    }


}
