using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FortBackend.src.App.Utilities.MongoDB.Module
{
    [BsonIgnoreExtraElements]
    public class StoreInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("UserIds")]
        public string[] UserIds { get; set; } = new string[0];

        [BsonElement("UserIps")]
        public string[] UserIps { get; set; } = new string[0];

        [BsonElement("InitialBanReason")]
        public string InitialBanReason { get; set; } = "none";

        [BsonElement("InitialBanTimestamp")]
        public DateTime InitialBanTimestamp { get; set; } = DateTime.UtcNow;
    }
}
