namespace FortLauncher.Services.Utils
{
    public class LoginResponse
    {
        public bool banned { get; set; } = false;
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string character { get; set; } = string.Empty;
        public int vbucks { get; set; } = -1;
        public LoginStats stats { get; set; } = new LoginStats();
        public string created { get; set; } = "so many years ago";
        public string DiscordId { get; set; } = string.Empty;

    }

    public class LoginStats
    {
        public int Wins { get; set; } = 0;
        public int MatchesPlayed { get; set; } = 0;
        public int Kills { get; set; } = 0;
    }

}