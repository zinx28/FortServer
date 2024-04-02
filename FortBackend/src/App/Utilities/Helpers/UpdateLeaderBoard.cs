using FortBackend.src.App.Utilities.Classes.EpicResponses.LeaderBoard;
using FortBackend.src.App.Utilities.MongoDB;
using FortBackend.src.App.Utilities.MongoDB.Module;
using MongoDB.Driver;
using System.Collections.Generic;

namespace FortBackend.src.App.Utilities.Helpers
{

    public class UpdateLeaderBoard
    {
        static IMongoCollection<StatsInfo> Collection { get; set; }
        public static LeaderBoardData LeaderboardCached { get; set; } = new LeaderBoardData();
        public static async Task<List<LeaderBoardStats>> GrabTop100()
        {
            var TempData = new List<LeaderBoardStats>();

            var data = await Collection.Find(FilterDefinition<StatsInfo>.Empty).ToListAsync();

            foreach (var statName in GetStatNames())
            {
                var ListStatsData = new Dictionary<string, int>();

                var top100 = data
                .OrderByDescending(x => x.stats.TryGetValue(statName, out int value) ? value : int.MinValue)
                .Take(100)
                .ToList();


                foreach (var stats in top100)
                {
                    Console.WriteLine(stats.AccountId);
                    Console.WriteLine(statName);
                    int statsValue = -1;
                    if (stats.stats.TryGetValue(statName, out int value69))
                    {
                        statsValue = value69;
                    }

                    ListStatsData.Add(stats.AccountId, statsValue);
                }

                TempData.Add(new LeaderBoardStats
                {
                    statName = statName,
                    stat = ListStatsData
                });

            }
            
            return TempData;
        }
        public static async Task GrabLatest()
        {
            //LeaderBoardData
            Collection = MongoDBStart.Database.GetCollection<StatsInfo>("StatsInfo");

            LeaderboardCached.Data = await GrabTop100();
            //LeaderboardCached.Duos = await GrabTop100("duos");
            //LeaderboardCached.Trios = await GrabTop100("trios");
            //LeaderboardCached.Squads = await GrabTop100("squad");
            //LeaderboardCached.Ltms = await GrabTop100("ltm");
        }

        public static async Task LeaderboardLoop()
        {
            while (true)
            {
                await GrabLatest();
                Logger.Log("Grabbed Latest Leaderboard", "[Leaderboard]");

                await Task.Delay(900000);
            }
        }

        //pc_m0_p2 ~ solos
        //pc_m0_p10 ~ duos
        //pc_m0_p9 ~ squads
        public static IEnumerable<string> GetStatNames()
        {
            return new List<string>
            {
                "br_placetop1_pc_m0_p2",
                "br_placetop10_pc_m0_p2",
                "br_placetop25_pc_m0_p2",
                "br_placetop1_pc_m0_p10",
                "br_placetop5_pc_m0_p10",
                "br_placetop12_pc_m0_p10",
                "br_placetop1_pc_m0_p9",
                "br_placetop3_pc_m0_p9",
                "br_placetop6_pc_m0_p9"
            };
        }
    }
}
