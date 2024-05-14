using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.Helpers.UserManagement;
using FortBackend.src.App.Utilities.MongoDB;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.XMPP.Data;
using FortLibrary.MongoDB.Module;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Diagnostics.Eventing.Reader;
using static FortLibrary.DiscordAuth;

namespace FortBackend.src.App.Utilities.Helpers
{
    public class CheckIP
    {
        public static async Task<string> Init(string Ip, User UserData)
        {
            IMongoCollection<StoreInfo> StoreInfocollection = MongoDBStart.Database.GetCollection<StoreInfo>("StoreInfo");
            var filter = Builders<StoreInfo>.Filter.AnyEq(b => b.UserIps, Ip);
            var count = await StoreInfocollection.CountDocumentsAsync(filter);
            if (count > 0)
            {
                bool hasUpdates = false;
                var update = Builders<StoreInfo>.Update.Combine();

                var existingIPs = await StoreInfocollection.Find(filter).Project(b => b.UserIps).FirstOrDefaultAsync();
                var newIps = UserData.UserIps.Except(existingIPs).ToArray();
                if (newIps.Count() > 0)
                {
                    update = update.PushEach(b => b.UserIps, newIps);
                    hasUpdates = true;
                }

                var existingIds = await StoreInfocollection.Find(filter).Project(b => b.UserIds).FirstOrDefaultAsync();
                string[] SoReal = new string[] { UserData.AccountId };
                var newIds = SoReal.Except(existingIds).ToArray();

                if (newIds.Count() > 0)
                {
                    update = update.PushEach(b => b.UserIds, newIds);
                    hasUpdates = true;
                }


                if (hasUpdates)
                {
                    await StoreInfocollection.UpdateManyAsync(filter, update);
                }
                try
                {
                    try
                    {
                        var FindAccessToken = GlobalData.AccessToken.FirstOrDefault(e => e.accountId == UserData.AccountId);
                        if(FindAccessToken != null)
                        {
                            GlobalData.AccessToken.Remove(FindAccessToken);
                        }
                        
                        await MongoSaveData.SaveToDB(UserData.AccountId);
                        CacheMiddleware.GlobalCacheProfiles.Remove(UserData.AccountId);
                    }
                    catch { } // idfk

                    await BanAndWebHooks.Init(Saved.Saved.DeserializeConfig, new UserInfo
                    {
                        id = UserData.DiscordId,
                        username = UserData.Username
                    });

                    await Handlers.UpdateOne<User>("DiscordId", UserData.DiscordId, new Dictionary<string, object>()
                    {
                        { "banned", true }
                    });

                    return "banned";
                }
                catch (Exception ex)
                {
                    return "unknown";
                }

                //return BadRequest(new { Error = "You are banned!" });

            }
            return "ok";
        }

    }
}
