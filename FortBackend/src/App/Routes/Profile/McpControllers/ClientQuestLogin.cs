using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.ConfigHelpers;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Newtonsoft.Json;
using System.Collections.Generic;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class ClientQuestLogin
    {
        // This IS TEMP code
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, Account_Module AccountDataParsed)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                var jsonData = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\Json\\default.json"));
                if (!string.IsNullOrEmpty(jsonData))
                {
                    Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, AccountDataParsed);

                    //Console.WriteLine("fas");
                    List<AthenaItem> contentconfig = JsonConvert.DeserializeObject<List<AthenaItem>>(jsonData); //dynamicbackgrounds.news
                    //Console.WriteLine("GR");
                    ProfileChange test1 = response.profileChanges[0] as ProfileChange;
                    foreach (AthenaItem test in contentconfig)
                    {
                        //Console.WriteLine("TET");
                        test1.Profile.items.Add(test.templateId, test);
                    }
                    return response;
                }
                else
                {
                    Logger.Error("ClientQuestLogin might not function well");
                }
            }

            return new Mcp();
        }
    }
}
