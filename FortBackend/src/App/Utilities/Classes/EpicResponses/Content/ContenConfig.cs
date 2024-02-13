namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Content
{
    class ContentConfig
    {
        public Battleroyalenewscontent battleroyalenews { get; set; } = new Battleroyalenewscontent();
        public List<Emergencynoticecontent> emergencynotice { get; set; } = new List<Emergencynoticecontent>();
    }

    class Battleroyalenewscontent
    {
        public List<TempMotds> motds { get; set; } = new List<TempMotds>();
        public List<TempMotds> messages { get; set; } = new List<TempMotds>();
    }

    class TempMotds
    {
        public string image { get; set; } = "";
        public string title { get; set; } = "FortBackend";
        public string body { get; set; } = "Play Universal on fort the backend yippeee";
    }
    class Emergencynoticecontent
    {
        public string title { get; set; } = "FortBackend";
        public string body { get; set; } = "Play Universal on fort the backend yippeee";
    }
}
