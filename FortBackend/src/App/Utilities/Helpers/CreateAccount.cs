using FortBackend.src.App.Utilities;
using FortLibrary.Encoders;
using FortBackend.src.App.Utilities.Helpers.UserManagement;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.EpicResponses.Profile.Query.Items;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using static FortLibrary.DiscordAuth;
using FortBackend.src.App.Utilities.MongoDB.Management;
using FortLibrary.MongoDB;
using FortLibrary;

namespace FortBackend.src.App.Utilities.Helpers
{
    public class CreateAccount
    {
        public CreateAccount() { }
        public class SandboxLoadoutAttributes1
        {
            public object locker_slots_data { get; set; } = new object();
            public string banner_color_template { get; set; } = string.Empty;
            public string banner_icon_template { get; set; } = string.Empty;
            public string locker_name { get; set; } = string.Empty;

            public int use_count { get; set; }
            public bool item_seen { get; set; }
            public bool favorite { get; set; }
        }

        public class ProfileItem1
        {
            public string templateId { get; set; } = string.Empty;
            public SandboxLoadoutAttributes1 attributes { get; set; } = new SandboxLoadoutAttributes1();
            public int quantity { get; set; }
        }
        public static async Task<string> Init(HttpContext httpContext, IMongoDatabase _database, UserInfo responseData1, bool Global = false)
        {
            try
            {

                var Ip = "";
                if (Saved.Saved.DeserializeConfig.Cloudflare)
                    Ip = httpContext.Request.Headers["CF-Connecting-IP"];
                else
                    Ip = httpContext.Connection.RemoteIpAddress!.ToString();

                var username = responseData1.username;
                var GlobalName = responseData1.global_name;
                var DiscordId = responseData1.id;

                IMongoCollection<User> Usercollection = _database.GetCollection<User>("User");
                IMongoCollection<UserFriends> UserFriendscollection = _database.GetCollection<UserFriends>("UserFriends");
                IMongoCollection<Account> Accountcollection = _database.GetCollection<Account>("Account");
                IMongoCollection<StatsInfo> Statscollection = _database.GetCollection<StatsInfo>("StatsInfo");


                string AccountId = Guid.NewGuid().ToString("N").Replace("-", "").Substring(0, 12) + DiscordId;
                string NewAccessToken = JWT.GenerateRandomJwtToken(15, Saved.Saved.DeserializeConfig.JWTKEY);
                string[] UserIp = new string[] { Ip };

                bool BanUser = false;

                if (Saved.Saved.DeserializeConfig.EnableDetections)
                {
                    IMongoCollection<StoreInfo> StoreInfocollection = _database.GetCollection<StoreInfo>("StoreInfo");
                    var filter = Builders<StoreInfo>.Filter.AnyEq(b => b.UserIps, Ip);
                    var count = await StoreInfocollection.CountDocumentsAsync(filter);
                    if (count > 0)
                    {
                        BanUser = true;

                        var update = Builders<StoreInfo>.Update
                            .PushEach(e => e.UserIps, new[] { Ip })
                            .PushEach(e => e.UserIds, new[] { AccountId });

                        var updateResult = await StoreInfocollection.UpdateManyAsync(filter, update);

                        if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                        {
                            if (Saved.Saved.DeserializeConfig.DetectedWebhookUrl != null)
                            {
                                await BanAndWebHooks.Init(Saved.Saved.DeserializeConfig, responseData1);
                            }
                        }
                    }
                }

                await MongoDBCreateAccount.Init(new CreateAccountArg
                {
                    AccountID = AccountId,
                    DiscordId = DiscordId,
                    DisplayName = Global ? GlobalName : username,
                    Email = Generate.RandomString(10) + "@fortbackend.com",
                    Password = Generate.RandomString(15),
                    UserIps = UserIp,
                    banned = BanUser,
                    NewAccessToken = NewAccessToken,
                });

                return NewAccessToken;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CreateAccount");
            }
            return "ERROR";
        }
    }
}
