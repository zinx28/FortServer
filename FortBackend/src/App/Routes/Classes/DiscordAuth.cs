namespace FortBackend.src.App.Routes.Classes
{
    public class DiscordAuth
    {
        
        public class Server
        {
            public string id { get; set; }
            public string name { get; set; }
            public string icon { get; set; }
            public bool owner { get; set; }
            public ulong permissions { get; set; }
            public string permissions_new { get; set; }
            public string[] features { get; set; }

        }

        public class UserInfo
        {
            public string id { get; set; }
            public string username { get; set; }
            public string avatar { get; set; }

            // other stuff

            public string global_name { get; set; }
            public string email { get; set; }
        }
    }
}
