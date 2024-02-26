using FortBackend.src.App.Utilities.Classes.ConfigHelpers;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Content
{
    public class Tournamentinformation
    {
        public ConversionConfig conversion_config { get; set; } = new ConversionConfig();
        public TournamentInfo tournament_info { get; set; } = new TournamentInfo();
        public string _title { get; set; } = "tournamentinformation";
        public bool _noIndex { get; set; } = false;
        public string _activeDate { get; set; } = "2018-11-13T22:32:47.734Z";
        public string lastModified { get; set; } = "2022-03-21T14:20:49.608Z";
        public string _locale { get; set; } = "en-US";
        public string _templateName { get; set; } = "FortniteGameTournamentInfo";
    }

    public class ConversionConfig
    {
        public string containerName { get; set; } = "tournament_info";
        public string _type { get; set; } = "Conversion Config";
        public bool enableReferences { get; set; } = true;
        public string contentName { get; set; } = "tournaments";
    }

    public class TournamentInfo
    {
        public List<TournamentInformation> tournaments { get; set; } = new List<TournamentInformation>();
        public string _type { get; set; } = "Tournaments Info";
        //TournamentInformation
    }
}
