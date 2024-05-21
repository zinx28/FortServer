using FortLibrary;
using MongoDB.Driver;

namespace FortBackend.src.App.Utilities.MongoDB.Helpers
{
    // This Creates a Blank Module
    public class CreateBlank
    {
        public static void Module<T>(IMongoDatabase database)
        {
            var collectionName = typeof(T).Name;
            try
            {
                if (!CollectionExists(database, collectionName))
                {
                    database.CreateCollection(collectionName);
                    Logger.Log($"Created {collectionName}", "MongoDB");
                }
            }
            catch
            {
                Console.WriteLine($"Failed to create blank collection -> {collectionName}");
            }
        }


        public static bool CollectionExists(IMongoDatabase database, string collectionName)
        {
            var collections = database.ListCollectionNames().ToList();
            return collections.Contains(collectionName);
        }
    }
}
