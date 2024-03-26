using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using System.Threading;

namespace FortBackend.src.App.Utilities.Helpers.Middleware
{
    public class ProfileCacheEntry
    {
        public string AccountId { get; set; } = string.Empty; // accountid
        public Account AccountData { get; set; } = new Account();
        public User UserData { get; set; } = new User();
        public UserFriends UserFriends { get; set; } = new UserFriends();
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }


    public class CacheMiddleware : IHostedService
    {
        public static Dictionary<string, ProfileCacheEntry> GlobalCacheProfiles = new Dictionary<string, ProfileCacheEntry>();
        private CancellationTokenSource _cancellationTokenSource;

        // private readonly IMemoryCache _cache;
        //private readonly IMongoCollection<Account> _mongoAccountCollection;
        //private readonly IMongoCollection<StoreInfo> _mongoStoreInfoCollection;
        //private readonly IMongoCollection<User> _mongoUserCollection;
        //private readonly IMongoCollection<UserFriends> _mongoUserFriendsCollection;

        //private readonly RequestDelegate _next;
        //public CacheMiddleware() {
        //    Logger.Log("Starting", "CacheMiddleware");
        //    Task.Run(CleanupCache);
        //}

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => CleanupCache(_cancellationTokenSource.Token), _cancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
        }

        public static async Task ShutDown()
        {
            Console.WriteLine("Saving Data Before Shutdown");

            var now = DateTime.Now;

            foreach (var kvp in GlobalCacheProfiles)
            {
                var profileId = kvp.Key;
                var profile = kvp.Value;

                //if (profile.LastUpdated < now)
                //{
                Console.WriteLine("i would love to update data but no temp data");
                GlobalCacheProfiles.Remove(profileId);
                //}
            }

            Console.WriteLine("Waiting ");
            await Task.Delay(TimeSpan.FromSeconds(10));


        }

        public static async Task CleanupCache(CancellationToken cancellationToken)
        {
            Logger.Log("Caching.... :)", "CacheMiddleware");
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(10));

                var now = DateTime.Now;

                foreach (var kvp in GlobalCacheProfiles)
                {
                    var profileId = kvp.Key;
                    var profile = kvp.Value;

                    if (profile.LastUpdated < now)
                    {
                        Console.WriteLine("i would love to update data but no temp data");
                        GlobalCacheProfiles.Remove(profileId);
                    }
                }

                Logger.Log("Rechecking", "CacheMiddleware");
            }
        }
    }
}
