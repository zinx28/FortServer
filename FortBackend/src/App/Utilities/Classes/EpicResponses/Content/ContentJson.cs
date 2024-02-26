using System.Text.Json.Serialization;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Content
{
    public class ContentJson
    {
        [JsonPropertyName("jcr:isCheckedOut")]
        public bool isCheckedOut { get; set; } = false;

        public string _title { get; set; } = "Fortnite Game";

        [JsonPropertyName("jcr:baseVersion")]
        public string baseVersion { get; set; } = "a7ca237317f1e7883b3279";

        public string _activeDate { get; set; } = "2017-08-30T03:20:48.050Z";
        public string lastModified { get; set; } = "2023-11-09T18:08:17.334Z";
        public string _locale { get; set; } = "en_US";
        public BattleRoyaleNews battleroyalenews { get; set; } = new BattleRoyaleNews();
        public BattleRoyaleNewsV2 battleroyalnewsv2 { get; set; } = new BattleRoyaleNewsV2();
        public EmergencyNotice emergencynotice { get; set; } = new EmergencyNotice();
        public EmergencyNoticeV2 emergencynoticev2 { get; set; } = new EmergencyNoticeV2();
        public DynamicBackground dynamicbackgrounds { get; set; } = new DynamicBackground();

        public ShopCarousel shopCarousel { get; set; } = new ShopCarousel();
        public ShopSections shopSections { get; set; } = new ShopSections();

        public Tournamentinformation tournamentinformation { get; set; } = new Tournamentinformation();
    }
}
