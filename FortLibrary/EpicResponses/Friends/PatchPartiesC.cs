namespace FortLibrary.EpicResponses.Friends
{
    public class PatchPartiesC
    {
        public PatchPatiesConfig config { get; set; } = new PatchPatiesConfig();

        public PatchPatiesMetaC meta { get; set; } = new PatchPatiesMetaC();
        public int revision { get; set; } = 0;
    }

    public class PatchPatiesMetaC {
        public List<string> delete { get; set; } = new List<string>();
        public Dictionary<string, object> update { get; set; } = new Dictionary<string, object>();
    }

    // eas
    public class PatchPatiesConfig
    {
        public string discoverability { get; set; } = "ALL";
        public string joinability { get; set; } = "OPEN";
    }
}
