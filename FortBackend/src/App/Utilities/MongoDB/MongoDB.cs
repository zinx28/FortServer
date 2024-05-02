using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using FortLibrary.ConfigHelpers;


namespace FortBackend.src.App.Utilities.MongoDB
{
    public class MongoDBStart
    {
        public static IMongoDatabase? Database { get; private set; }

        public static void Initialize(IServiceCollection services, IConfiguration Configuration)
        {
            Logger.Log("Initializing MongoDB", "MongoDB");
            Config DeserializeConfig = Saved.Saved.DeserializeConfig;
            string connectionString = DeserializeConfig.MongoDBConnectionString;
            string connectionName = DeserializeConfig.MongoDBConnectionName;


            MongoClient MongoDBStartup = new MongoClient(connectionString);

            IMongoDatabase database = MongoDBStartup.GetDatabase(connectionName);
            Database = database;
            Handlers.LaunchDataBase(database); // legit helps and cleans so much stuff up

            services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

            services.AddScoped<IMongoDatabase>(serviceProvider =>
            {
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();

                var conventionPack = new ConventionPack
                    {
                        new IgnoreIfDefaultConvention(true),
                        new IgnoreExtraElementsConvention(true)
                    };
                ConventionRegistry.Register("IgnoreConventions", conventionPack, t => true);

                return mongoClient.GetDatabase(connectionName);
            });

            Logger.Log("Attempting Blank Files", "MongoDB");

            CreateBlank.Module<User>(database);
            CreateBlank.Module<UserFriends>(database);
            CreateBlank.Module<Account>(database);
            CreateBlank.Module<StatsInfo>(database);
            CreateBlank.Module<StoreInfo>(database);

            Logger.Log("MongoDB has fully loaded", "MongoDB");
        }
    }
}
