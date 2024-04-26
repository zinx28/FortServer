
using Amazon.Runtime.Internal.Transform;
using FortLibrary.Dynamics;
using Newtonsoft.Json;
using System.IO;

namespace FortBackend.src.App.Utilities.Helpers.BattlepassManagement
{
    public class BattlepassManager
    {
        public static Dictionary<int, StoreBattlepassPages> BattlePasses = new Dictionary<int, StoreBattlepassPages>();
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
                    if (!seasonFolder.Contains("Season1"))
                    {
                        Logger.Error($"The season {season} doesn't contain a battlepass!", "BattlepassManager");
                    }else
                    {
                        BattlePasses.Add(season, new StoreBattlepassPages() { name = "season1doesnthaveabattlepass"}); // else it would say 1/3
                    }
                }
            }

            Logger.Log($"Loaded BattlePasses [{BattlePasses.Count}/{seasonFolders.Count()}]", "BattlpassManager");
        }
    }
}
