namespace FortLibrary.EpicResponses.Friends
{
    public class PatchPartyMeta
    {
        public List<string> delete { get; set; } = new List<string>();
        public Dictionary<string, object> update { get; set; } = new Dictionary<string, object>();
        public int revision { get; set; }
    }
}
