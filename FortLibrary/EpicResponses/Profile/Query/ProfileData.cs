namespace FortLibrary.EpicResponses.Profile.Query
{
    public class ProfileData
    {
        public string _id { get; set; } = "RANDOM";
       // public string Update { get; set; } = "";
        public string Created { get; set; } = DateTime.Parse(DateTime.UtcNow.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string Updated { get; set; } = DateTime.Parse(DateTime.UtcNow.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public int rvn { get; set; }
        public int WipeNumber { get; set; } = 1;
        public string accountId { get; set; } = "0";
        public string profileId { get; set; } = "notproper";
        public string version { get; set; } = "no_version";
        public Stats stats { get; set; } = new Stats();
        public Dictionary<string, dynamic> items { get; set; } = new Dictionary<string, dynamic>();
        public int commandRevision { get; set; } = 5;
    }
}
