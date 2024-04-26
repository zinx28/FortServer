
using Amazon.Runtime.Internal.Transform;
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
        public static void Init()
        {
            string[] seasonFolders = Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src/Resources/json/Season"));

            foreach (string seasonFolder in seasonFolders)
            {
                if (!seasonFolder.Contains("Season")) continue;
                //Console.WriteLine(seasonFolder);
                int season = int.Parse(seasonFolder.Split("\\Season")[1]);
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
                            Logger.Error($"Failed To Load {season}", "BattlepassManager");
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

                var SeasonFreeBattlePassFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\Json\\Season\\Season{season}\\SeasonFreeBattlepass.json");
              
                if (File.Exists(SeasonFreeBattlePassFolder))
                {
                    var FreeBattlePass = File.ReadAllText(SeasonFreeBattlePassFolder);
                    if (FreeBattlePass != null)
                    {
                        List<Battlepass> FreeTier = JsonConvert.DeserializeObject<List<Battlepass>>(FreeBattlePass)!;

                        if (FreeTier.Count > 0)
                        {
                            FreeBattlePassItems.Add(season, FreeTier);

                            var SeasonPaidBattlePassFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\Json\\Season\\Season{season}\\SeasonPaidBattlepass.json");

                            if (File.Exists(SeasonPaidBattlePassFolder))
                            {
                                var PaidBattlePass = File.ReadAllText(SeasonFreeBattlePassFolder);
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

            Logger.Log($"Loaded BattlePasses [{BattlePasses.Count}/{seasonFolders.Count()}]", "BattlpassManager");
            Logger.Log($"Loaded FreeBattlePasses [{FreeBattlePassItems.Count}/{seasonFolders.Count()}]", "BattlpassManager");
            Logger.Log($"Loaded PaidBattlePasses [{PaidBattlePassItems.Count}/{seasonFolders.Count()}]", "BattlpassManager");
        }
    }
}
