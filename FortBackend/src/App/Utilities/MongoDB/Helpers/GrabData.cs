using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.MongoDB.Module;
using Newtonsoft.Json;
using FortLibrary;
using MongoDB.Driver;
using FortLibrary.MongoDB.Modules;

namespace FortBackend.src.App.Utilities.MongoDB.Helpers
{
    public class GrabData
    {

        // gonna use cached data for this as it's the best way !

        public static async Task<ProfileCacheEntry> ProfileDiscord(string DiscordId, string SearchKey = "DiscordId")
        {

            try
            {
                var GrabData = default(KeyValuePair<string, ProfileCacheEntry>);
                if (CacheMiddleware.GlobalCacheProfiles.Any())
                {
                    GrabData = CacheMiddleware.GlobalCacheProfiles.FirstOrDefault(e => e.Value.UserData.DiscordId == DiscordId);
                }
                if (GrabData.Value == null)
                {
                    var UserData = await Handlers.FindOne<User>(SearchKey, DiscordId);
                    if (UserData != "Error")
                    {
                        User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)![0];
                        if (UserDataParsed != null)
                        {
                            return await FindCache(UserDataParsed);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Bypass? Invaild Account");
                    }
                }
                else
                {
                    if (GrabData.Value != null)
                    {
                        //Console.WriteLine("RETURNING CORRET DATA <3");
                        return GrabData.Value;
                    }
                    Console.WriteLine("what? how");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "GrabDataDiscord");
            }

            return new ProfileCacheEntry();
        }
        public static async Task<ProfileCacheEntry> ProfileEmail(string UsersEmail)
        {

            try
            {
                var GrabData = default(KeyValuePair<string, ProfileCacheEntry>);
                if (CacheMiddleware.GlobalCacheProfiles.Any())
                {
                    GrabData = CacheMiddleware.GlobalCacheProfiles.FirstOrDefault(e => e.Value.UserData.Email == UsersEmail);
                }
                if (GrabData.Value == null)
                {
                    var UserData = await Handlers.FindOne<User>("email", UsersEmail);
                    if (UserData != "Error")
                    {
                        User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)![0];
                        if (UserDataParsed != null)
                        {
                            return await FindCache(UserDataParsed);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Bypass? Invaild Account");
                    }
                }
                else
                {
                    if (GrabData.Value != null)
                    {
                        //Console.WriteLine("RETURNING CORRET DATA <3");
                        return GrabData.Value;
                    }
                    Console.WriteLine("what? how");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "GrabDataEmail");
            }

            return new ProfileCacheEntry();
        }
        public static async Task<ProfileCacheEntry> Profile(string AccountId, bool Auth = false, string AuthToken = "N"/*, bool Find11 = false*/)
        {
            try
            {
                var GrabData = default(KeyValuePair<string, ProfileCacheEntry>);
                if (CacheMiddleware.GlobalCacheProfiles.Any())
                {
                    if (Auth)
                    {
                        GrabData = CacheMiddleware.GlobalCacheProfiles.FirstOrDefault(e => e.Value.UserData.accesstoken == AuthToken);
                    }
                    else
                    {
                        GrabData = CacheMiddleware.GlobalCacheProfiles.FirstOrDefault(e => e.Key == AccountId);
                    }
                }
                if (GrabData.Value == null)
                {
                    var UserData = await Handlers.FindOne<User>(Auth ? "accesstoken" : "accountId", Auth ? AuthToken : AccountId);
                    if (UserData != "Error")
                    {
                        User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)![0];
                        if (UserDataParsed != null)
                        {
                            // we need to check again just in case
                            if (Auth)
                            {
                                GrabData = CacheMiddleware.GlobalCacheProfiles.FirstOrDefault(e => e.Key == UserDataParsed.AccountId);
                                if (GrabData.Value != null)
                                {
                                    return GrabData.Value;
                                }
                            }
                                


                            return await FindCache(UserDataParsed);
                        }
                    }
                    else
                    {
                        Logger.Error("Bypass? Invaild Account", string.IsNullOrEmpty(AccountId) ? "ERROR" : AccountId);
                    }
                }
                else
                {
                    if (GrabData.Value != null)
                    {
                        //Console.WriteLine("RETURNING CORRET DATA <3");
                        return GrabData.Value;
                    }
                    Console.WriteLine("what? how");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "GrabDataProfile");
            }
           
            return new ProfileCacheEntry();
        }


        public static async Task<ProfileCacheEntry> FindCache(User UserDataParsed)
        {


            var AccountData = await Handlers.FindOne<Account>("accountId", UserDataParsed.AccountId);
            var AccStatsData = await Handlers.FindOne<StatsInfo>("accountId", UserDataParsed.AccountId);
            var FriendsData = await Handlers.FindOne<UserFriends>("accountId", UserDataParsed.AccountId);

            if (AccountData != "Error" && FriendsData != "Error")
            {
                Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)![0];
                StatsInfo AccStatsDataParsed = JsonConvert.DeserializeObject<StatsInfo[]>(AccStatsData)![0];
                UserFriends FriendsDataParsed = JsonConvert.DeserializeObject<UserFriends[]>(FriendsData)![0];

                if (AccountDataParsed != null && UserDataParsed != null && FriendsDataParsed != null)
                {
                    ProfileCacheEntry ProfileCacheEntry = new ProfileCacheEntry
                    {
                        AccountId = UserDataParsed.AccountId,
                        AccountData = AccountDataParsed,
                        StatsData = AccStatsDataParsed,
                        UserData = UserDataParsed,
                        UserFriends = FriendsDataParsed,
                        LastUpdated = DateTime.Now,
                    };

                    CacheMiddleware.GlobalCacheProfiles.Add(UserDataParsed.AccountId, ProfileCacheEntry);

                    return ProfileCacheEntry;
                }
            }

            return new ProfileCacheEntry();
        }
    }
}
