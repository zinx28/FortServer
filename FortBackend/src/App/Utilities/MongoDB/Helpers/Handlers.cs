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

        public static async Task<string> FindSimilar<T>(string FindData, object valueData, int howmany, string CustomRegex = "")
        {
            try
            {
                var collection = _database.GetCollection<T>(typeof(T).Name);
                var filterBuilder = Builders<T>.Filter;
               
                var regexPattern = (BsonRegularExpression)null;
                if (!string.IsNullOrEmpty(CustomRegex))
                {
                    Console.WriteLine("T");
                    regexPattern = new BsonRegularExpression(CustomRegex, "i");
                }
                else
                {
                    var exactValue = Regex.Escape(valueData.ToString());
                    regexPattern = new BsonRegularExpression($"{exactValue}", "i");
                }
                

                var filter = filterBuilder.Regex(FindData, regexPattern);
                var result = await collection.Find(filter).Limit(howmany).ToListAsync();

                if (result != null && result.Any())
                {
                    return JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FindSimilar -> " + ex.Message);
            }

            return "Error";
        }

        public static async Task<string> UpdateOne<T>(string FindValue, object valueData, Dictionary<string, object> updateFields, bool PullFromArray = false)
        {
            try
            {
                var collection = _database.GetCollection<T>(typeof(T).Name);
                var filter = Builders<T>.Filter.Eq(FindValue, valueData);

                var updateDefinitions = new List<UpdateDefinition<T>>();

                foreach (var field in updateFields)
                {
                    var fieldName = field.Key;
                    var fieldValue = field.Value;
                    var updateDefinition = Builders<T>.Update.Set(fieldName, BsonValue.Create(fieldValue));

                    updateDefinitions.Add(updateDefinition);
                }

                var combinedUpdate = Builders<T>.Update.Combine(updateDefinitions);


                if (combinedUpdate == null)
                {
                    return "Error";
                }
                else
                {
                    await collection.UpdateOneAsync(filter, combinedUpdate);
                    return "Updated";
                }

            }
            catch (Exception ex)
            {
                Logger.Error("UpdateOne -> " + ex.Message);
                return "Error";
            }
        }


        public static async Task<string> PullFromArray<T>(string FindData, string valueData, string arrayField, string pullField, string pullValue)
        {
            try
            {
                var collection = _database.GetCollection<T>(typeof(T).Name);
                var filter = Builders<T>.Filter.Eq(FindData, valueData);


                var updateResult = await collection.UpdateOneAsync(
                   filter,
                   Builders<T>.Update.PullFilter(arrayField, Builders<BsonDocument>.Filter.Eq(pullField, pullValue))
                );
                if (updateResult.ModifiedCount > 0)
                {
                    return "Updated";
                }
                else
                {
                    return "No documents matched the filter criteria.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("[MongoDB:PullFromArray] -> " + ex.Message);
                return "Error";
            }
        }

        public static async Task<string> PushOne<T>(string FindValue, object valueData, Dictionary<string, object> updateFields, bool ForceThingy = true)
        {
            try
            {
                var collection = _database.GetCollection<T>(typeof(T).Name);
                var filter = Builders<T>.Filter.Eq(FindValue, valueData);
                var updateDefinitions = new List<UpdateDefinition<T>>();

                foreach (var field in updateFields)
                {
                    var fieldName = field.Key;
                    var fieldValue = field.Value;
                    if (ForceThingy)
                    {
                        if (fieldValue is List<Dictionary<string, object>> fieldValueList)
                        {
                            var bsonList = fieldValueList.Select(dictionary => BsonValue.Create(dictionary)).ToList();
                            var updateDefinition = Builders<T>.Update.PushEach(fieldName, bsonList);
                            updateDefinitions.Add(updateDefinition);
                        }
                        else
                        {
                            var updateDefinition = Builders<T>.Update.Push(fieldName, BsonValue.Create(fieldValue));
                            updateDefinitions.Add(updateDefinition);
                        }
                    }
                    else
                    {
                        if (fieldValue is List<Dictionary<string, object>> fieldValueList)
                        {
                            var bsonList = fieldValueList.Select(dictionary => dictionary).ToList();
                            var updateDefinition = Builders<T>.Update.PushEach(fieldName, bsonList);
                            updateDefinitions.Add(updateDefinition);
                        }
                        else
                        {
                            var updateDefinition = Builders<T>.Update.Push(fieldName, fieldValue);
                            updateDefinitions.Add(updateDefinition);
                        }
                    }
                }

                var combinedUpdate = Builders<T>.Update.Combine(updateDefinitions);

                if (combinedUpdate == null)
                {
                    return "Error";
                }
                else
                {
                    await collection.UpdateOneAsync(filter, combinedUpdate);

                    return "Updated";
                }

            }
            catch (Exception ex)
            {
                Logger.Error("PushOne -> " + ex.Message);
                return "Error";
            }
        }
}
}
