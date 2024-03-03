using FortBackend.src.App.Utilities.MongoDB.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FortBackend.src.App.Utilities.MongoDB.Module
{
    [BsonCollectionName("Friends")]
    [BsonIgnoreExtraElements]
    public class UserFriends
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("DiscordId")]
        public string DiscordId { get; set; }

        [BsonElement("accountId")]
        public string AccountId { get; set; }


        [BsonElement("incoming")]
        [BsonIgnoreIfNull]
        public List<FriendsObject> Incoming { get; set; } = new List<FriendsObject>();

        [BsonElement("outgoing")]
        [BsonIgnoreIfNull]
        public List<FriendsObject> Outgoing { get; set; } = new List<FriendsObject>();

        [BsonElement("accepted")]
        [BsonIgnoreIfNull]
        public List<FriendsObject> Accepted { get; set; } = new List<FriendsObject>();

        [BsonElement("blocked")]
        [BsonIgnoreIfNull]
        public List<FriendsObject> Blocked { get; set; } = new List<FriendsObject>();
    }
    public class FriendsObject
    {
        [BsonRepresentation(BsonType.String)]
        public string accountId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public string alias { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public DateTime created { get; set; }
    }
}
