using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace FortBackend.src.App.Utilities.MongoDB.Helpers
{
    public class Handlers
    {
        private static IMongoDatabase _database;

        public static void LaunchDataBase(IMongoDatabase database)
        {
            _database = database;
        }

        public async static Task<string> FindOne<T>(string FindData, object valueData)
        {
            try
            {
                if(string.IsNullOrEmpty(FindData) || string.IsNullOrEmpty(valueData.ToString())) 
                {
                    Logger.Error("FindOne Blank Data");
                    return "Error";
                }

                var collection = _database.GetCollection<T>(typeof(T).Name);
                var filterBuilder = Builders<T>.Filter;
                var exactValue = Regex.Escape(valueData.ToString());
                var regexPattern = $"^{exactValue}$";
                var filter = filterBuilder.Regex(FindData, new BsonRegularExpression(regexPattern));
                var result = await collection.Find(filter).ToListAsync();

                if (result != null && result.Any()) return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                Logger.Error("FindOne -> " + ex.Message);
            }
            return "Error";
        }
    }
}
