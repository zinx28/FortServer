using System.Runtime.CompilerServices;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.LeaderBoard
{
    // This is data that caches
    public class LeaderBoardData
    {
        public List<LeaderBoardStats> Data { get; set; } = new List<LeaderBoardStats>();
        //public List<LeaderBoardStats> Duos { get; set; } = new List<LeaderBoardStats>();
        //public List<LeaderBoardStats> Trios { get; set; } = new List<LeaderBoardStats>();
        //public List<LeaderBoardStats> Squads { get; set; } = new List<LeaderBoardStats>();
        //public List<LeaderBoardStats> Ltms { get; set; } = new List<LeaderBoardStats>();
    } 

    public class LeaderBoardStats
    {
        public string statName { get; set; } = string.Empty;
        public List<statsData> stat { get; set; } = new List<statsData>();

    }

    public class statsData
    {
        public string accountId { get; set; } = string.Empty;
        public int value { get; set; } = 0;
    }
    //public class LeaderBoardStats
    //{
    //    public string AccountId { get; set; } = string.Empty;
    //    public int Wins { get; set; } = 0; // stats
    //}
}
