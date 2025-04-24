using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Dynamics
{
    public class ReportUserClass
    {
        public bool bBlockUserRequested { get; set; }
        public bool bIsCompetitiveEvent { get; set; }
        public bool bUserMarkedAsKnown { get; set; }
        public string creativeIslandGuid { get; set; } = string.Empty;
        public string creativeIslandOwnerAccountId { get; set; } = string.Empty;
        public string creativeIslandSharingLink { get; set; } = string.Empty;
        public string details { get; set; } = string.Empty;
        public string? gameSessionId { get; set; } = null;
        public string? token { get; set; } = null;
        public string offenderPlatform { get; set; } = string.Empty;
        public string playlistName { get; set; } = "None";
        public string reason { get; set; } = string.Empty; // type of reason
        public string reporterPlatform { get; set; } = "Windows";
        public string subGameName { get; set; } = "Athena"; // 
    }
}
