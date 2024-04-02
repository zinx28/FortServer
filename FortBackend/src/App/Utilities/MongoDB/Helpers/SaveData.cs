using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Module;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.MongoDB.Helpers
{
    public class MongoSaveData
    {
        public static async Task SaveToDB(string AccountId)
        {
            var GrabData = CacheMiddleware.GlobalCacheProfiles.FirstOrDefault(e => e.Key == AccountId);

            if (!GrabData.Equals(default(KeyValuePair<string, ProfileCacheEntry>)))
            {
                var filter1 = Builders<User>.Filter.Eq(x => x.AccountId, AccountId);
                var filter2 = Builders<Account>.Filter.Eq(x => x.AccountId, AccountId);
                var filter3 = Builders<StatsInfo>.Filter.Eq(x => x.AccountId, AccountId);
                var filter4 = Builders<UserFriends>.Filter.Eq(x => x.AccountId, AccountId);

                var collection = MongoDBStart.Database?.GetCollection<User>("User");
                var collection2 = MongoDBStart.Database?.GetCollection<Account>("Account");
                var collection3 = MongoDBStart.Database?.GetCollection<StatsInfo>("StatsInfo");
                var collection4 = MongoDBStart.Database?.GetCollection<UserFriends>("UserFriends");

                if (collection != null && collection2 != null && collection3 != null && collection4 != null)
                {
                    await collection.ReplaceOneAsync(filter1, GrabData.Value.UserData);
                    await collection2.ReplaceOneAsync(filter2, GrabData.Value.AccountData);
                    await collection3.ReplaceOneAsync(filter3, GrabData.Value.StatsData);
                    await collection4.ReplaceOneAsync(filter4, GrabData.Value.UserFriends);
                }
            }

            return;
        }
    }
}
