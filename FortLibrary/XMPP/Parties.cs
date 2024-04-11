namespace FortLibrary.XMPP
{
    public class Parties
    {
        public string id { get; set; } = string.Empty;
        public string privacy { get; set; } = "PUBLIC";
        public string created_at { get; set; } = string.Empty;
        public string updated_at { get; set; } = string.Empty;
        public object config { get; set; } = new object();
        public List<Members> members { get; set; } = new List<Members>();
        public List<object> applicants { get; set; } = new List<object>();
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
        public List<object> invites { get; set; } = new List<object>();
        public int revision { get; set; }
        public List<object> intentions { get; set; } = new List<object>();
    }
}
