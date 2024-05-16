using FortLibrary.MongoDB.Module;
using FortLibrary.Dynamics;
using Newtonsoft.Json;
using System.Net.Http.Json;
using FortBackend.src.App.Utilities.Constants;

namespace FortBackend.src.App.Utilities.Quests
{
    public class DailyQuestsManager
    {
        public static Dictionary<string, List<DailyQuestsJson>> DailyQuestsSeasonAboveDictionary = new Dictionary<string, List<DailyQuestsJson>>();
        
        //public static List<DailyQuestsJson> DailyQuestsSeason1Objects = new List<DailyQuestsJson>();
        //public static List<DailyQuestsJson> DailyQuestsSeasonAboveObjects = new List<DailyQuestsJson>();
        public static async Task<DailyQuestsJson> GrabRandomQuest(SeasonClass seasonClass)
        {
            Random random = new Random();
            if (seasonClass.SeasonNumber <= 10 || seasonClass.SeasonNumber == 13) // eh
            {
                var FindKey = "SeasonsAbove";
                if (seasonClass.SeasonNumber == 1)
                {
                    FindKey = "Season1";
                }
                else if (seasonClass.SeasonNumber == 13)
                {
                    FindKey = "Season13";
                }

                if (DailyQuestsSeasonAboveDictionary.Count() > 0)
                {
                    try
                    {
                        List<DailyQuestsJson> SomeList = DailyQuestsSeasonAboveDictionary.FirstOrDefault(e => e.Key == FindKey).Value;
                        
                        DailyQuestsJson dailyQuestsJson = SomeList[random.Next(SomeList.Count())];

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

            }
       
            //if (seasonClass.SeasonNumber == 1)
            //{
            //    if (DailyQuestsSeason1Objects.Count() > 0)
            //    {
            //        try
            //        {
            //            DailyQuestsJson dailyQuestsJson = DailyQuestsSeason1Objects[random.Next(DailyQuestsSeason1Objects.Count())];

            //            // We check if its already a thing.. if so run again
            //            if (!CheckUsedQuest(dailyQuestsJson, seasonClass))
            //            {
            //                return dailyQuestsJson;
            //            }

            //            return await GrabRandomQuest(seasonClass);
            //        }
            //        catch (Exception ex) { Logger.Error(ex.Message, "Grab Random Quest!"); };
            //    }
            //    else
            //    {
            //        Logger.Error("DAILY QUEST FOLDER IS EMPTY!", "DailyQuestManager");
            //    }
            //}
            //else if(seasonClass.SeasonNumber > 1 && seasonClass.SeasonNumber <= 10)
            //{
            //    if (DailyQuestsSeasonAboveObjects.Count() > 0)
            //    {
            //        try
            //        {
            //            DailyQuestsJson dailyQuestsJson = DailyQuestsSeasonAboveObjects[random.Next(DailyQuestsSeasonAboveObjects.Count())];

            //            // We check if its already a thing.. if so run again
            //            if (!CheckUsedQuest(dailyQuestsJson, seasonClass))
            //            {
            //                return dailyQuestsJson;
            //            }

            //            return await GrabRandomQuest(seasonClass);
            //        }
            //        catch (Exception ex) { Logger.Error(ex.Message, "Grab Random Quest!"); };
            //    }
            //    else
            //    {
            //        Logger.Error("DAILY QUEST FOLDER IS EMPTY!", "DailyQuestManager");
            //    }
            //}
            //else
            //{
            //    if(seasonClass.SeasonNumber == 0)
            //    {
            //        return new DailyQuestsJson();
            //    }

            //    // REMOVE IF YOU ARE USING A SEASON HIGHER THEN CHAPTER 1 OR THIS WILL SPAM
            //    Logger.Error("UNSUPPORTED SEASON -> " + seasonClass.SeasonNumber, "QUESTS");
            //}
           

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

        public static DailyQuestsJson ReturnQuestInfo(string DailyQuestName, int season)
        {
            if(season <= 10) // eh
            {
                var FindKey = "SeasonsAbove";
                if(season == 1)
                {
                    FindKey = "Season1";
                }
                if (DailyQuestsSeasonAboveDictionary.Count() > 0)
                {
                    DailyQuestsJson DailyResponse = DailyQuestsSeasonAboveDictionary.FirstOrDefault(e => e.Key == FindKey).Value.FirstOrDefault(e => e.Name == DailyQuestName)!;
                    if (DailyResponse != null)
                    {
                        return DailyResponse;
                    }
                }
            }
            //else if(season > 1 && season <= 10)
            //{
            //    if (DailyQuestsSeasonAboveObjects.Count() > 0)
            //    {
            //        DailyQuestsJson DailyResponse = DailyQuestsSeasonAboveObjects.FirstOrDefault(e => e.Name == DailyQuestName)!;
            //        if (DailyResponse != null)
            //        {
            //            return DailyResponse;
            //        }
            //    }
            //}
            //else
            //{
            //    // ifk
            //}
        

            return new DailyQuestsJson();
        }

        public static void LoadDailyQuests()
        {
            var DailyQuestsFolder = Path.Combine(PathConstants.BaseDir, "Json/Quests/DailyQuests");
            if(Path.Exists(DailyQuestsFolder))
            {
                string[] FolderDir = Directory.GetDirectories(Path.Combine(PathConstants.BaseDir, "Json/Quests/DailyQuests"));
                if (FolderDir.Count() > 0)
                {
                    
                    foreach(string folder in FolderDir)
                    {
                        string[] QuestsLoader = Directory.GetDirectories(Path.Combine(PathConstants.BaseDir, "Json/Quests/DailyQuests", Path.GetFileName(folder)));
                        foreach (string Quests in QuestsLoader)
                        {
                            string[] DailyQuestsFiles = new string[0];
                            /// if (Quests == "Season 1")
                            //{
                            // Console.WriteLine(Quests);
                           // Console.WriteLine(Quests);
                                DailyQuestsFiles = Directory.GetFiles(Path.Combine(PathConstants.BaseDir, "Json/Quests/DailyQuests", Path.GetFileName(folder), Path.GetFileName(Quests)), "*.json");

                                if (DailyQuestsFiles.Count() > 0)
                                {
                                    List<DailyQuestsJson> DailyQuestsList = new List<DailyQuestsJson>();
                                    
                                    try
                                    {
                                        foreach (string item in DailyQuestsFiles)
                                        {
                                            string jsonContent = File.ReadAllText(item);
                                            if (jsonContent != null)
                                            {
                                                //Console.WriteLine(jsonContent);
                                                DailyQuestsJson dailyQuestJson = JsonConvert.DeserializeObject<DailyQuestsJson>(jsonContent)!;

                                                if (dailyQuestJson != null)
                                                {
                                                    DailyQuestsList.Add(dailyQuestJson);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex.Message, "DAILYQUESTMANAGER");
                                    }
                                   

                                    DailyQuestsSeasonAboveDictionary.Add(Path.GetFileName(Quests), DailyQuestsList);

                                    var a = DailyQuestsList.Count();
                                    var b = DailyQuestsFiles.Count();

                                    Logger.Log($"Loaded Daily {Path.GetFileName(Quests)} Quests: {a}/{b}", "DailyQuestManager");
                                }
                                else
                                {
                                    // throw error? or let them find out they are skunekd
                                    Logger.Error("DAILY QUEST FOLDER IS EMPTY!", "DailyQuestManager");
                                }
                            //}else
                            //{
                            //    Logger.Error("folder different.. weird?", "DAILYQUESTSMANAGER");
                           // }
                        }
                    }
                }             
            }
        }
    }
}
