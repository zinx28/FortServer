using FortBackend.src.App.Utilities.MongoDB.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FortBackend.src.App.Utilities.MongoDB.Module
{
    //[BsonCollectionName("User")]
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("accountId")]
        public string AccountId { get; set; }

        [BsonElement("DiscordId")]
        public string DiscordId { get; set; }

        [BsonElement("accesstoken")]
        [BsonIgnoreIfNull]
        public string accesstoken { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        // This will be auto randomly generated when the user creates a account
        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("banned")]
        public bool banned { get; set; } = false; // idk this might not be used if theres a better system

    }
}
