using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.MongoDB.Module;
using MongoDB.Driver;
using Newtonsoft.Json;
using FortLibrary;

namespace FortBackend.src.App.Utilities.MongoDB.Helpers
{
    public class MongoSaveData
    {
        public static async Task SaveToDB(string accountId)
        {
            if (!CacheMiddleware.GlobalCacheProfiles.TryGetValue(accountId, out var profileEntry)) return;

            var db = MongoDBStart.Database;
            if (db == null) return;

            var userCollection = db.GetCollection<User>("User");
            var accountCollection = db.GetCollection<Account>("Account");
            var statsCollection = db.GetCollection<StatsInfo>("StatsInfo");
            var friendsCollection = db.GetCollection<UserFriends>("UserFriends");

            var userFilter = Builders<User>.Filter.Eq(x => x.AccountId, accountId);
            var accountFilter = Builders<Account>.Filter.Eq(x => x.AccountId, accountId);
            var statsFilter = Builders<StatsInfo>.Filter.Eq(x => x.AccountId, accountId);
            var friendsFilter = Builders<UserFriends>.Filter.Eq(x => x.AccountId, accountId);

            var tasks = new Task[]
            {
                userCollection.ReplaceOneAsync(userFilter, profileEntry.UserData),
                accountCollection.ReplaceOneAsync(accountFilter, profileEntry.AccountData),
                statsCollection.ReplaceOneAsync(statsFilter, profileEntry.StatsData),
                friendsCollection.ReplaceOneAsync(friendsFilter, profileEntry.UserFriends)
            };

            await Task.WhenAll(tasks);
        }
    }
}
