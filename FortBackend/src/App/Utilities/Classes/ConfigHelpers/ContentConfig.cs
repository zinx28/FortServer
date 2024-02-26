namespace FortBackend.src.App.Utilities.Classes.ConfigHelpers
{
    class ContentConfig
    {
        public Battleroyalenewscontent battleroyalenews { get; set; } = new Battleroyalenewscontent();
        public List<Emergencynoticecontent> emergencynotice { get; set; } = new List<Emergencynoticecontent>();
        public List<shopSectionsItems> shopSections { get; set; } = new List<shopSectionsItems>();
        public List<TournamentInformation> tournamentinformation { get; set; } = new List<TournamentInformation>();
    }

    public class TournamentInformation
    {
        public string title_color { get; set; } = "FFFFFF";
        public string loading_screen_image { get; set; } = string.Empty;
        public string background_text_color { get; set; } = string.Empty;
        public string background_right_color { get; set; } = string.Empty;
        public string poster_back_image { get; set; } = string.Empty;
        public string _type { get; set; } = string.Empty;
        public string pin_earned_text { get; set; } = string.Empty;
        public string tournament_display_id { get; set; } = string.Empty;
        public string highlight_color { get; set; } = string.Empty;
        public string schedule_info { get; set; } = string.Empty;
        public string primary_color { get; set; } = string.Empty;
        public string flavor_description { get; set; } = string.Empty;
        public string poster_front_image { get; set; } = string.Empty;
        public string short_format_title { get; set; } = string.Empty;
        public string title_line_2 { get; set; } = string.Empty;
        public string title_line_1 { get; set; } = string.Empty;
        public string shadow_color { get; set; } = string.Empty;
        public string details_description { get; set; } = string.Empty;
        public string background_left_color { get; set; } = string.Empty;
        public string long_format_title { get; set; } = string.Empty;
        public string poster_fade_color { get; set; } = string.Empty;
        public string secondary_color { get; set; } = string.Empty;
        public string playlist_tile_image { get; set; } = string.Empty;
        public string base_color { get; set; } = string.Empty;
    }
    class shopSectionsItems
    {
        public int landingPriority { get; set; } = 0;
        public string sectionId { get; set; } = "TEST";
        public string sectionDisplayName = "TEST";
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
