using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;


namespace FortBackend.src.App.Utilities.MongoDB
{
    public class MongoDBStart
    {
        public static IMongoDatabase Database { get; private set; }

        public static void Initialize(IServiceCollection services, IConfiguration Configuration)
        {
            Logger.Log("Initializing MongoDB", "MongoDB");
            Config DeserializeConfig = Saved.Saved.DeserializeConfig;
            string connectionString = DeserializeConfig.MongoDBConnectionString;
            string connectionName = DeserializeConfig.MongoDBConnectionName;


            MongoClient MongoDBStartup = new MongoClient(connectionString);

            IMongoDatabase database = MongoDBStartup.GetDatabase(connectionName);
            Database = database;

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

            Logger.Log("MongoDB has fully loaded", "MongoDB");
        }
    }
}
