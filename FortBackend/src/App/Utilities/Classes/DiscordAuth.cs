namespace FortBackend.src.App.Utilities.Classes
{
    public class DiscordAuth
    {

        public class Server
        {
            public ulong id { get; set; }
            public string name { get; set; } = string.Empty;
            public string icon { get; set; } = string.Empty;
            public bool owner { get; set; }
            public ulong permissions { get; set; }
            public string permissions_new { get; set; } = string.Empty;
            public string[] features { get; set; } = new string[0];

        }

        public class UserInfo
        {
            public string id { get; set; } = string.Empty;
            public string username { get; set; } = string.Empty;
            public string avatar { get; set; } = string.Empty;

            // other stuff

            public string global_name { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
        }
    }
}
