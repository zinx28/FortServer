using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.FortniteServices.Events
{
    public class ScoreC
    {
        public string trackedStat { get; set; } = string.Empty;
        public string matchRule { get; set; } = string.Empty;
        public List<dynamic> rewardTiers { get; set; } = new List<dynamic>();
    }
}
