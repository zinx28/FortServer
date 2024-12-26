using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortLibrary.Dynamics.Dashboard;

namespace FortLibrary.MongoDB.Modules
{
    [BsonIgnoreExtraElements]
    public class AdminInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("accountId")]
        public string AccountId { get; set; } = string.Empty;

        [BsonElement("DiscordId")]
        public string DiscordId { get; set; } = string.Empty;

        [BsonElement("Role")]
        public int Role { get; set; } = AdminDashboardRoles.Moderator;

        //[BsonElement("DisplayName")]
        //public string DisplayName { get; set; } = string.Empty;
    }
}
