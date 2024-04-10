using FortLibrary.MongoDB.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary
{
    public class ProfileCacheEntry
    {
        public string AccountId { get; set; } = string.Empty; // accountid
        public Account AccountData { get; set; } = new Account();
        public User UserData { get; set; } = new User();
        public StatsInfo StatsData { get; set; } = new StatsInfo();
        public UserFriends UserFriends { get; set; } = new UserFriends();
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
