using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FortBackend.src.App.Utilities.MongoDB.Module
{
    [BsonIgnoreExtraElements]
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("accountId")]
        public string AccountId { get; set; }

        [BsonElement("DiscordId")]
        public string DiscordId { get; set; }






        // last !

        [BsonElement("accessToken")]
        public string[] AccessToken { get; set; } = new string[0];

        [BsonElement("refreshToken")]
        public string[] RefreshToken { get; set; } = new string[0];

        [BsonElement("clientToken")]
        public string[] ClientToken { get; set; } = new string[0];
    }
}
