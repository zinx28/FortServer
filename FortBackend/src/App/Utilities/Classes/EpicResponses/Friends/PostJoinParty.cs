using FortBackend.src.App.XMPP.Helpers.Resources;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Friends
{
    public class PostJoinParty
    {
        public Connection connection { get; set; } 
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
    }

    public class Connection
    {
        public string id { get; set; }
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
        public bool yield_leadership { get; set; } = false;
    }
}
