using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using FortLibrary.ConfigHelpers;
using FortLibrary.MongoDB.Modules;
using FortLibrary;
using MongoDB.Bson;


namespace FortBackend.src.App.Utilities.MongoDB
{
    public class MongoDBStart
    {
        public static IMongoDatabase? Database { get; private set; }

        public static void Initialize(IServiceCollection services, IConfiguration Configuration)
        {
            Logger.Log("Initializing MongoDB", "MongoDB");
      
            FortConfig DeserializeConfig = Saved.Saved.DeserializeConfig;
            string connectionString = DeserializeConfig.MongoDBConnectionString;
            string connectionName = DeserializeConfig.MongoDBConnectionName;

            MongoClient MongoDBStartup = new MongoClient(connectionString);

            IMongoDatabase database;

            // if user doesn't have mongodb server installed / wrong server auth? or smth it wont load
            try
            {
                database = MongoDBStartup.GetDatabase(connectionName);
                if (database == null)
                {
                    Logger.Error("MongoDB is not available. Please check your connection settings.", "MongoDB");
                    throw new Exception("MongoDB is not available. Please check your connection settings.");
                }

                database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();
            }
            catch (Exception ex)
            {
                Logger.Error("Couldn't load MongoDB. Please check the config and ensure MongoDB is configured correctly.", "MongoDB");
                throw new Exception("Couldn't load MongoDB. Please check the config and ensure MongoDB is configured correctly.", ex);
            }


            Database = database;
            Handlers.LaunchDataBase(database); // legit helps and cleans so much stuff up
            
            services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

            var conventionPack = new ConventionPack
            {
                new IgnoreIfDefaultConvention(true),
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("IgnoreConventions", conventionPack, t => true);

            services.AddScoped<IMongoDatabase>(serviceProvider =>
            {
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();

                return mongoClient.GetDatabase(connectionName);
            });

            Logger.Log("Attempting Blank Files", "MongoDB");

            CreateBlank.Module<User>(database);
            CreateBlank.Module<UserFriends>(database);
            CreateBlank.Module<Account>(database);
            CreateBlank.Module<StatsInfo>(database);
            CreateBlank.Module<StoreInfo>(database);
            CreateBlank.Module<AdminInfo>(database);

            Logger.Log("MongoDB has fully loaded", "MongoDB");
        }
    }
}
