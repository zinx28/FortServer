using FortLibrary.MongoDB.Module;
using FortLibrary.Dynamics;
using Newtonsoft.Json;
using System.Net.Http.Json;
using FortBackend.src.App.Utilities.Constants;

namespace FortBackend.src.App.Utilities.Quests
{
    public class DailyQuestsManager
    {
        public static List<DailyQuestsJson> DailyQuestsObjects = new List<DailyQuestsJson>();
        public static async Task<DailyQuestsJson> GrabRandomQuest(SeasonClass seasonClass)
        {
            Random random = new Random();
            if (DailyQuestsObjects.Count() > 0)
            {
                try 
                { 
                    DailyQuestsJson dailyQuestsJson = DailyQuestsObjects[random.Next(DailyQuestsObjects.Count())];

                    // We check if its already a thing.. if so run again
                    if (!CheckUsedQuest(dailyQuestsJson, seasonClass))
                    {
                        return dailyQuestsJson;
                    }

                    return await GrabRandomQuest(seasonClass);
                }
                catch (Exception ex) { Logger.Error(ex.Message, "Grab Random Quest!"); };
            }
            else
            {
                Logger.Error("DAILY QUEST FOLDER IS EMPTY!", "DailyQuestManager");
            }

            return new DailyQuestsJson();
        }

        public static bool CheckUsedQuest(DailyQuestsJson dailyQuestJson, SeasonClass seasonClass)
        {
            if (seasonClass.DailyQuests.Daily_Quests.Any(e => e.Key == dailyQuestJson.Name))
            {
                return true;
            }

            return false;
        }

        public static DailyQuestsJson ReturnQuestInfo(string DailyQuestName)
        {
            if (DailyQuestsObjects.Count() > 0)
            {
                DailyQuestsJson DailyResponse = DailyQuestsObjects.FirstOrDefault(e => e.Name == DailyQuestName)!;   
                if(DailyResponse != null)
                {
                    return DailyResponse;
                }
            }

            return new DailyQuestsJson();
        }

        public static void LoadDailyQuests()
        {
            var DailyQuestsFolder = Path.Combine(PathConstants.BaseDir, "src/Resources/Json/Quests/DailyQuests");
            if(Path.Exists(DailyQuestsFolder))
            {
                string[] DailyQuestsFiles = Directory.GetFiles(Path.Combine(PathConstants.BaseDir, "src/Resources/Json/Quests/DailyQuests"), "*.json");

                if (DailyQuestsFiles.Count() > 0)
                {
                    foreach (string item in DailyQuestsFiles)
                    {
                        string jsonContent = File.ReadAllText(item);
                        if (jsonContent != null)
                        {
                            DailyQuestsJson dailyQuestJson = JsonConvert.DeserializeObject<DailyQuestsJson>(jsonContent)!;

                            if (dailyQuestJson != null)
                            {
                                DailyQuestsObjects.Add(dailyQuestJson);
                            }
                        }
                    }

                    var a = DailyQuestsObjects.Count();
                    var b = DailyQuestsFiles.Count();

                    Logger.Log($"Loaded Daily Quests: {a}/{b}", "DailyQuestManager");
                }
                else
                {
                    // throw error? or let them find out they are skunekd
                    Logger.Error("DAILY QUEST FOLDER IS EMPTY!", "DailyQuestManager");
                }
            }
        }
    }
}
