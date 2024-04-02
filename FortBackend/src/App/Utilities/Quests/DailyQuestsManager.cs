using FortBackend.src.App.Utilities.Classes.Dynamics;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Quests
{
    public class DailyQuestsManager
    {
        public static string[] DailyQuestsFiles = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/Json/Quests/DailyQuests"), "*.json");
        public static async Task<DailyQuestsJson> GrabRandomQuest(SeasonClass seasonClass)
        {
            Random random = new Random();
            if (DailyQuestsFiles.Count() > 0)
            {
                string randomJsonFile = DailyQuestsFiles[random.Next(DailyQuestsFiles.Length)];

                try
                {
                    string jsonContent = File.ReadAllText(randomJsonFile);
                    if (jsonContent != null)
                    {
                        DailyQuestsJson dailyQuestJson = JsonConvert.DeserializeObject<DailyQuestsJson>(jsonContent)!;

                        // We check if its already a thing.. if so run again
                        if(!CheckUsedQuest(dailyQuestJson, seasonClass))
                        {
                            return dailyQuestJson;
                        }

                        return await GrabRandomQuest(seasonClass);
                    }
                }
                catch { };
            }else
            {
                Logger.Error("DAILY QUESTS FILES ARE EMPTY");
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
    }
}
