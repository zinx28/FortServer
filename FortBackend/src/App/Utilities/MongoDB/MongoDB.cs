using FortBackend.src.App.Utilities.Saved;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;


namespace FortBackend.src.App.Utilities.MongoDB
{
    public class MongoDBStart
    {
        public static void Initialize(IServiceCollection services, IConfiguration Configuration)
        {
            Logger.Log("Initializing MongoDB", "MongoDB");
            Config DeserializeConfig = Saved.Saved.DeserializeConfig;
            string connectionString = DeserializeConfig.MongoDBConnectionString;
            string connectionName = DeserializeConfig.MongoDBConnectionName;

            MongoClient MongoDBStartup = new MongoClient(connectionString);
            IMongoDatabase database69 = MongoDBStartup.GetDatabase(connectionName);
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

            Logger.Log("Skipped Blank Files", "MongoDB");

            Logger.Log("MongoDB has fully loaded", "MongoDB");
        }
    }
}
