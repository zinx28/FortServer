using FortLibrary.Default;
using FortLibrary.Encoders;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB;
using FortLibrary.MongoDB.Module;
using MongoDB.Driver;

namespace FortBackend.src.App.Utilities.MongoDB.Management
{
    public class MongoDBCreateAccount
    {
        public static async Task Init(CreateAccountArg CreateAccArg)
        {
            if (MongoDBStart.Database is null) return;
            IMongoCollection<User> Usercollection = MongoDBStart.Database.GetCollection<User>("User");
            IMongoCollection<UserFriends> UserFriendscollection = MongoDBStart.Database.GetCollection<UserFriends>("UserFriends");
            IMongoCollection<Account> Accountcollection = MongoDBStart.Database.GetCollection<Account>("Account");
            IMongoCollection<StatsInfo> Statscollection = MongoDBStart.Database.GetCollection<StatsInfo>("StatsInfo");
            
            if(string.IsNullOrEmpty(CreateAccArg.AccountID))
                CreateAccArg.AccountID = Guid.NewGuid().ToString("N").Replace("-", "").Substring(0, 12) + CreateAccArg.DiscordId;

            string NewAccessToken = JWT.GenerateRandomJwtToken(15, Saved.Saved.DeserializeConfig.JWTKEY);

            User UserData = new User
            {
                AccountId = CreateAccArg.AccountID,
                DiscordId = CreateAccArg.DiscordId,
                Username = CreateAccArg.DisplayName,
                Email = CreateAccArg.Email,
                accesstoken = NewAccessToken,
                UserIps = new string[0],
                banned = false,
                Password = CryptoGen.HashPassword(CreateAccArg.Password),
            };

            UserFriends UserFriendsData = new UserFriends
            {
                AccountId = CreateAccArg.AccountID,
                DiscordId = CreateAccArg.DiscordId
            };

            Account AccountData = DefaultAccount.Init(new AccountJsonFor
            {
                AccountID = CreateAccArg.AccountID,
                DiscordId = CreateAccArg.DiscordId,
            });

            StatsInfo statsData = new StatsInfo()
            {
                AccountId = CreateAccArg.AccountID,
                DiscordId = CreateAccArg.DiscordId
            };

            var userBulkOps = new List<WriteModel<User>>()
            {
                new InsertOneModel<User>(UserData)
            };

            var userFriendsBulkOps = new List<WriteModel<UserFriends>>()
            {
                new InsertOneModel<UserFriends>(UserFriendsData)
            };

            var accountBulkOps = new List<WriteModel<Account>>()
            {
                new InsertOneModel<Account>(AccountData)
            };

            var statsBulkOps = new List<WriteModel<StatsInfo>>()
            {
                new InsertOneModel<StatsInfo>(statsData)
            };

            await Task.WhenAll(
                Usercollection.BulkWriteAsync(userBulkOps),
                UserFriendscollection.BulkWriteAsync(userFriendsBulkOps),
                Accountcollection.BulkWriteAsync(accountBulkOps),
                Statscollection.BulkWriteAsync(statsBulkOps)
            );
        }
    }
}
