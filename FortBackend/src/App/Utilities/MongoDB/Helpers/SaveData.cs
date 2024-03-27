using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Module;
using MongoDB.Driver;

namespace FortBackend.src.App.Utilities.MongoDB.Helpers
{
    public class MongoSaveData
    {
        public static async Task SaveToDB(string AccountId)
        {
            var GrabData = CacheMiddleware.GlobalCacheProfiles.FirstOrDefault(e => e.Key == AccountId);
            if (!GrabData.Equals(default(KeyValuePair<string, ProfileCacheEntry>)))
            {
                Console.WriteLine("SAVING DATA");
                var filter1 = Builders<User>.Filter.Eq(x => x.AccountId, AccountId);
                var filter2 = Builders<Account>.Filter.Eq(x => x.AccountId, AccountId);
                var filter3 = Builders<UserFriends>.Filter.Eq(x => x.AccountId, AccountId);

                var collection = MongoDBStart.Database.GetCollection<User>("User");
                var collection2 = MongoDBStart.Database.GetCollection<Account>("Account");
                var collection3 = MongoDBStart.Database.GetCollection<UserFriends>("UserFriends");

                Console.WriteLine(GrabData.Value.AccountData.athena.Items);


                collection.ReplaceOne(filter1, GrabData.Value.UserData);
                collection2.ReplaceOne(filter2, GrabData.Value.AccountData);
                collection3.ReplaceOne(filter3, GrabData.Value.UserFriends);
            }

            return;
        }
    }
}
