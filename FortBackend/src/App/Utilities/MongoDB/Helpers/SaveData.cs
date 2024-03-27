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


                foreach (var item in GrabData.Value.AccountData.athena.Items)
                {
                    foreach (var kvp in item)
                    {
                        Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                    }
                }
                /*
                 * System.AggregateException: 'One or more errors occurred. (An error occurred while serializing the athena property of class FortBackend.src.App.Utilities.MongoDB.Module.Account: An error occurred while serializing the Items property of class FortBackend.src.App.Utilities.MongoDB.Module.Athena: Type Newtonsoft.Json.Linq.JObject is not configured as a type that is allowed to be serialized for this instance of ObjectSerializer.)'

                */

                collection.ReplaceOne(filter1, GrabData.Value.UserData);
                collection2.ReplaceOne(filter2, GrabData.Value.AccountData);
                collection3.ReplaceOne(filter3, GrabData.Value.UserFriends);
            }

            return;
        }
    }
}
