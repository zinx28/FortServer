using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using System.Collections.Generic;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile
{
    public class Mcp
    {
        public int profileRevision { get; set; } = 0;
        public string profileId { get; set; }
        public int profileChangesBaseRevision { get; set; } = 1;
        public List<dynamic> profileChanges { get; set; } = new List<dynamic>();
        public DateTime serverTime { get; set; }

        public int profileCommandRevision { get; set; } = 1;
        public int responseVersion { get; set; } = 1;
    }
}
