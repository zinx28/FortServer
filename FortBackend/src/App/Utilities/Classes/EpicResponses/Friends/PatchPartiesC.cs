namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Friends
{
    public class PatchPartiesC
    {
        public object config { get; set; }

        public PatchPatiesMetaC meta { get; set; }
        public int revision { get; set; } = 0;
    }

    public class PatchPatiesMetaC {
        public List<string> delete { get; set; }
        public Dictionary<string, object> update { get; set; }
    }
    //public class PatchPatiesConfig
    //{
    //    public string discoverability { get; set; } =
    //}
}
