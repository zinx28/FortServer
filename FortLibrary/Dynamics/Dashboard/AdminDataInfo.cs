using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Dynamics.Dashboard
{
    public class AdminDataInfo
    {
        public string AccountId { get; set; } = string.Empty;

        public string DiscordId { get; set; } = string.Empty;
        public int Role { get; set; } = AdminDashboardRoles.Moderator;
    }
}
