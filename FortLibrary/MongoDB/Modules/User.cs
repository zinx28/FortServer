using FortLibrary.MongoDB.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FortLibrary.MongoDB.Module
{
    [BsonCollectionName("User")]
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("accountId")]
        public string AccountId { get; set; } = string.Empty;

        [BsonElement("DiscordId")]
        public string DiscordId { get; set; } = string.Empty;

        [BsonElement("accesstoken")]
        [BsonIgnoreIfNull]
        public string accesstoken { get; set; } = string.Empty;

        [BsonElement("Username")]
        public string Username { get; set; } = string.Empty;

        // This will be auto randomly generated when the user creates a account
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("UserIps")]
        public string[] UserIps { get; set; } = new string[0];

        [BsonElement("banned")]
        public bool banned { get; set; } = false; // idk this might not be used if theres a better system

    }
}
