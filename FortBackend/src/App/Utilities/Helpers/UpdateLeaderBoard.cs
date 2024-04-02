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
        public static async Task<List<LeaderBoardStats>> GrabTop100(string GM)
        {
            var TempData = new List<LeaderBoardStats>();

            var data = await Collection.Find(FilterDefinition<StatsInfo>.Empty).ToListAsync();

            var top100 = data
                .OrderByDescending(x => x.Gamemodes.FirstOrDefault(e => e.Gamemode == GM)?.Stats.Wins ?? 0)
                .Take(100)
                .ToList();

            foreach (var stats in top100) {
                Console.WriteLine(stats.AccountId);
                TempData.Add(new LeaderBoardStats
                {
                    AccountId = stats.AccountId,
                    Wins = stats.Gamemodes.FirstOrDefault(e => e.Gamemode == GM)?.Stats.Wins ?? -1
                });
            }
            
            return TempData;
        }
        public static async Task GrabLatest()
        {
            //LeaderBoardData
            Collection = MongoDBStart.Database.GetCollection<StatsInfo>("StatsInfo");

            LeaderboardCached.Solo = await GrabTop100("solos");
            LeaderboardCached.Duos = await GrabTop100("duos");
            LeaderboardCached.Trios = await GrabTop100("trios");
            LeaderboardCached.Squads = await GrabTop100("squad");
            LeaderboardCached.Ltms = await GrabTop100("ltm");
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
    }
}
