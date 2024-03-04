namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Friends
{
    public class PatchPartyMeta
    {
        public List<string> delete { get; set; }
        public Dictionary<string, object> update { get; set; }
        public int revision { get; set; }
    }
}
