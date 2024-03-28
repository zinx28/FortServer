namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Friends
{
    public class PatchPartiesC
    {
        public object config { get; set; } = new object();

        public PatchPatiesMetaC meta { get; set; } = new PatchPatiesMetaC();
        public int revision { get; set; } = 0;
    }

    public class PatchPatiesMetaC {
        public List<string> delete { get; set; } = new List<string>();
        public Dictionary<string, object> update { get; set; } = new Dictionary<string, object>();
    }
    //public class PatchPatiesConfig
    //{
    //    public string discoverability { get; set; } =
    //}
}
