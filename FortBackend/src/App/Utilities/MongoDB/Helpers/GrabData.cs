using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.MongoDB.Helpers
{
    public class GrabData
    {
        public static async Task<ProfileCacheEntry> Profile(string AccountId)
        {
            try
            {
                Console.WriteLine("GRABBING DATA!");
                var GrabData = CacheMiddleware.GlobalCacheProfiles.FirstOrDefault(e => e.Key == AccountId);
                if (GrabData.Equals(default(KeyValuePair<string, ProfileCacheEntry>)))
                {
                    var AccountData = await Handlers.FindOne<Account>("accountId", AccountId);
                    var UserData = await Handlers.FindOne<User>("accountId", AccountId);
                    var FriendsData = await Handlers.FindOne<UserFriends>("accountId", AccountId);
                    Console.WriteLine("GRABBING DATA");
                    if (AccountData != "Error" && UserData != "Error" && FriendsData != "Error")
                    {
                        Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)?[0];
                        User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)?[0];
                        UserFriends FriendsDataParsed = JsonConvert.DeserializeObject<UserFriends[]>(FriendsData)?[0];
                        Console.WriteLine("GRABBING DATA3");
                        if (AccountDataParsed != null && UserDataParsed != null && FriendsDataParsed != null)
                        {
                            Console.WriteLine("GRABBING DATA4");
                            ProfileCacheEntry ProfileCacheEntry = new ProfileCacheEntry
                            {
                                AccountId = AccountId,
                                AccountData = AccountDataParsed,
                                UserData = UserDataParsed,
                                UserFriends = FriendsDataParsed,
                                LastUpdated = DateTime.Now,
                            };
                            CacheMiddleware.GlobalCacheProfiles.Add(AccountId, ProfileCacheEntry);

                            return ProfileCacheEntry;
                        }
                    }
                }
                else
                {
                    return GrabData.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "GrabData");
            }
           
            return new ProfileCacheEntry();
        }
    }
}
