using FortBackend.src.App.Utilities.Constants;
using FortLibrary.Dynamics;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Quests
{
    public class WeeklyQuestManager
    {
        public static Dictionary<string, List<WeeklyQuestsJson>> WeeklyQuestsSeasonAboveDictionary = new Dictionary<string, List<WeeklyQuestsJson>>();

        public static void LoadAllWeeklyQuest()
        {
            var WeeklyQuestsFolder = Path.Combine(PathConstants.BaseDir, "Json/Quests/Challenges");
            if (Path.Exists(WeeklyQuestsFolder))
            {
                var Season = -1;
                if(Saved.Saved.DeserializeGameConfig.ForceSeason)
                {
                    Season = Saved.Saved.DeserializeGameConfig.Season;
                }

                if(Season == -1)
                {
                    string[] FolderDir = Directory.GetDirectories(Path.Combine(PathConstants.BaseDir, "Json/Quests/Challenges"));
                    if (FolderDir.Count() > 0)
                    {
                        foreach (string folder in FolderDir)
                        {
                            // TO FORCE SEASON!!!
                            if (Saved.Saved.DeserializeGameConfig.ForceSeason)
                            {
                                if (!(Path.GetFileName(folder) == $"Season{Saved.Saved.DeserializeGameConfig.Season}")) continue;
                            }
                                //Path.GetFileName(folder)
                              
                            string[] QuestsLoader = Directory.GetDirectories(Path.Combine(PathConstants.BaseDir, "Json/Quests/Challenges", Path.GetFileName(folder)));

                            foreach (string Quests in QuestsLoader)
                            {

                               // Console.WriteLine(Quests);
                                string[] SeasonQuestsDir = Directory.GetFiles(Path.Combine(PathConstants.BaseDir, "Json/Quests/Challenges", Path.GetFileName(folder), Path.GetFileName(Quests)));
                                
                                List<WeeklyQuestsJson> WeeklyQuestsList = new List<WeeklyQuestsJson>();

                                Logger.Log($"MAX WEEKLY ITEM IS SET TO {Saved.Saved.DeserializeGameConfig.WeeklyQuest}", "WEEKLYQUESTMANAGER");
                                foreach (string SeasonFolder in SeasonQuestsDir)
                                {
                                    Console.WriteLine(SeasonFolder);
                                    if (SeasonFolder.Contains("Weekly"))
                                    {
                                        if (WeeklyQuestsList.Count >= Saved.Saved.DeserializeGameConfig.WeeklyQuest) continue;
                                        string jsonContent = File.ReadAllText(SeasonFolder);
                                        if (jsonContent != null)
                                        {
                                           
                                            WeeklyQuestsJson weeklyQuestJson = JsonConvert.DeserializeObject<WeeklyQuestsJson>(jsonContent)!;

                                            if (weeklyQuestJson != null)
                                            {
                                                WeeklyQuestsList.Add(weeklyQuestJson);
                                            }
                                        }
                                    }
                                }

                                WeeklyQuestsSeasonAboveDictionary.Add(Path.GetFileName(folder), WeeklyQuestsList);

                                var a = WeeklyQuestsList.Count();
                                var b = SeasonQuestsDir.Count();

                                Logger.Log($"Loaded Quests {Path.GetFileName(Quests)} Quests: {a}/{b} ({Path.GetFileName(folder)})", "WEEKLYQUESTMANAGER");
                            }
                        }
                    }
                }
                else
                {
                    Logger.Error("THIS NEEDS TO BE ADDED ELSE I WILL CRY!! or ill just remvoe this check");
                }
            }
        }
    }
}
