using FortBackend.src.App.Utilities.Constants;
using FortLibrary;
using FortLibrary.Dynamics;
using Newtonsoft.Json;
using System.IO;

namespace FortBackend.src.App.Utilities.Helpers.BattlepassManagement
{
    public class BattlepassManager
    {
        public static Dictionary<int, StoreBattlepassPages> BattlePasses = new Dictionary<int, StoreBattlepassPages>();
        public static Dictionary<int, List<Battlepass>> FreeBattlePassItems = new Dictionary<int, List<Battlepass>>();
        public static Dictionary<int, List<Battlepass>> PaidBattlePassItems = new Dictionary<int, List<Battlepass>>();

        public static Dictionary<int, List<SeasonXP>> SeasonBattlePassXPItems = new Dictionary<int, List<SeasonXP>>();

        public static Dictionary<int, List<SeasonBP>> SeasonBattleStarsItems = new Dictionary<int, List<SeasonBP>>();

        public static void Init()
        {
            string[] seasonFolders = Directory.GetDirectories(Path.Combine(PathConstants.BaseDir, $"json/Season"));

            foreach (string seasonFolder in seasonFolders)
            {
                if (!seasonFolder.Contains("Season")) continue;
                //Console.WriteLine(seasonFolder);
                int season = int.Parse(seasonFolder.Split("\\Season")[1]);


                var SeasonXPFolder = Path.Combine(seasonFolder, "SeasonXP.json");

                if (File.Exists(SeasonXPFolder))
                {
                    string seasonData = File.ReadAllText(SeasonXPFolder);

                    if (!string.IsNullOrEmpty(seasonData))
                    {
                        List<SeasonXP> seasonXp = JsonConvert.DeserializeObject<List<SeasonXP>>(seasonData)!;
                        if (seasonXp != null && seasonXp.Count > 0)
                        {
                            SeasonBattlePassXPItems.Add(season, seasonXp);
                        }
                        else
                        {
                            Logger.Error($"Failed To Load Season {season} ({Path.GetFileName(SeasonXPFolder)})", "BattlepassManager");
                        }
                        //JsonConvert.DeserializeObject<List<SeasonXP>>(SeasonData)
                    }
                }
                // string battlepassFilePath = Path.Combine(seasonFolder, "BattlePass.json");

                var SeasonBattleStarsFolder = Path.Combine(seasonFolder, "SeasonBP.json");

                if (File.Exists(SeasonBattleStarsFolder))
                {
                    string seasonData = File.ReadAllText(SeasonBattleStarsFolder);

                    if (!string.IsNullOrEmpty(seasonData))
                    {
                        List<SeasonBP> seasonXp = JsonConvert.DeserializeObject<List<SeasonBP>>(seasonData)!;
                        if (seasonXp != null && seasonXp.Count > 0)
                        {
                            SeasonBattleStarsItems.Add(season, seasonXp);
                        }
                        else
                        {
                            if(season > 1)
                                Logger.Error($"Failed To Load Season {season} ({Path.GetFileName(SeasonBattleStarsFolder)})", "BattlepassManager");
                        }
                        //JsonConvert.DeserializeObject<List<SeasonXP>>(SeasonData)
                    }
                }


                string battlepassFilePath = Path.Combine(seasonFolder, "BattlePass.json");

                if (File.Exists(battlepassFilePath))
                {
                    string seasonData = File.ReadAllText(battlepassFilePath);

                    if (!string.IsNullOrEmpty(seasonData))
                    {
                        StoreBattlepassPages itemshop = JsonConvert.DeserializeObject<StoreBattlepassPages>(seasonData)!;
                        if(itemshop != null)
                        {
                            BattlePasses.Add(season, itemshop);
                        }
                        else
                        {
                            Logger.Error($"Failed To Load Season {season} (${Path.GetFileName(battlepassFilePath)})", "BattlepassManager");
                        }
                    }
                }
                else
                {
                    if (seasonFolder.Contains("Season1") && !seasonFolder.Contains("Season11"))
                    {
                        BattlePasses.Add(season, new StoreBattlepassPages() { name = "season1doesnthaveabattlepass" }); // else it would say 1/3
                    }
                    else
                    {
                        Logger.Error($"The season {season} doesn't contain a battlepass!", "BattlepassManager");
                    }
                }

                var SeasonFreeBattlePassFolder = Path.Combine(PathConstants.BaseDir, $"Json\\Season\\Season{season}\\SeasonFreeBattlepass.json");
              
                if (File.Exists(SeasonFreeBattlePassFolder))
                {
                    var FreeBattlePass = File.ReadAllText(SeasonFreeBattlePassFolder);
                    if (FreeBattlePass != null)
                    {
                        List<Battlepass> FreeTier = JsonConvert.DeserializeObject<List<Battlepass>>(FreeBattlePass)!;

                        if (FreeTier.Count > 0)
                        {
                            FreeBattlePassItems.Add(season, FreeTier);

                            var SeasonPaidBattlePassFolder = Path.Combine(PathConstants.BaseDir, $"Json\\Season\\Season{season}\\SeasonPaidBattlepass.json");

                            if (File.Exists(SeasonPaidBattlePassFolder))
                            {
                                var PaidBattlePass = File.ReadAllText(SeasonPaidBattlePassFolder);
                                if (PaidBattlePass != null)
                                {
                                    List<Battlepass> PaidTier = JsonConvert.DeserializeObject<List<Battlepass>>(PaidBattlePass)!;

                                    if (PaidTier.Count > 0)
                                    {
                                        PaidBattlePassItems.Add(season, PaidTier);
                                    }
                                    else
                                    {
                                        Logger.Error($"The season {season} have a '[]' empty array paid tier file?", "BattlepassManager");
                                    }
                                }
                            }
                            else
                            {
                                if (seasonFolder.Contains("Season1") && !seasonFolder.Contains("Season11"))
                                {
                                    PaidBattlePassItems.Add(season, new List<Battlepass>()); //s1 no paid tier
                                }
                                else
                                {
                                    Logger.Error($"The season {season} doesnt have a paid tier file?", "BattlepassManager");
                                }
                            }
                        }
                        else
                        {
                            Logger.Error($"The season {season} have a '[]' empty array free tier file?", "BattlepassManager");
                        }
                    }
                }
                else
                {
                    Logger.Error($"The season {season} doesn't contain a SeasonFreeBattlePass!", "BattlepassManager");
                }
            }

            ///JsonConvert.DeserializeObject<List<SeasonXP>>(SeasonData)

            Logger.Log($"Loaded BattlePasses [{BattlePasses.Count}/{seasonFolders.Count()}]", "BattlpassManager");
            Logger.Log($"Loaded FreeBattlePasses [{FreeBattlePassItems.Count}/{seasonFolders.Count()}]", "BattlpassManager");
            Logger.Log($"Loaded PaidBattlePasses [{PaidBattlePassItems.Count}/{seasonFolders.Count()}]", "BattlpassManager");
            Logger.Log($"Loaded SeasonBattlePassXPItems [{SeasonBattlePassXPItems.Count}/{seasonFolders.Count()}]", "BattlpassManager");
            Logger.Log($"Loaded SeasonBattleStarsItems [{SeasonBattleStarsItems.Count}/{seasonFolders.Count()}]", "BattlpassManager");
        }
    }
}
