namespace FortBackend.src.App.Utilities.Classes.EpicResponses.LeaderBoard
{
    // This is data that caches
    public class LeaderBoardData
    {
        public List<LeaderBoardStats> Solo { get; set; } = new List<LeaderBoardStats>();
        public List<LeaderBoardStats> Duos { get; set; } = new List<LeaderBoardStats>();
        public List<LeaderBoardStats> Trios { get; set; } = new List<LeaderBoardStats>();
        public List<LeaderBoardStats> Squads { get; set; } = new List<LeaderBoardStats>();
        public List<LeaderBoardStats> Ltms { get; set; } = new List<LeaderBoardStats>();
    }

    public class LeaderBoardStats
    {
        public string AccountId { get; set; } = string.Empty;
        public int Wins { get; set; } = 0; // stats
    }
}
