using System.Runtime.CompilerServices;

namespace FortLibrary.EpicResponses.LeaderBoard
{
    // This is data that caches
    public class LeaderBoardData
    {
        public List<LeaderBoardStats> Data { get; set; } = new List<LeaderBoardStats>();
    } 

    public class LeaderBoardStats
    {
        public string statName { get; set; } = string.Empty;
        public Dictionary<string, int> stat { get; set; } = new Dictionary<string, int>();
    }
}
