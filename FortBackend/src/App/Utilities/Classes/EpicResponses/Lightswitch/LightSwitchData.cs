namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Lightswitch
{
    public class LightSwitchData
    {
        public string serviceInstanceId { get; set; } = "fortnite";
        public string status { get; set; } = "UP";
        public string message { get; set; } = "servers up.";
        public string maintenanceUri { get; set; } = "http://127.0.0.1:1111"; // yk
        public string[] overrideCatalogIds { get; set; } = new string[1] { "" };
        public string[] allowedActions { get; set; } = new string[2] { "PLAY", "DOWNLOAD" };
        public bool banned { get; set; } = false;
        public launcherInfoDTO launcherInfoDTO { get; set; } = new launcherInfoDTO();
    }
}
