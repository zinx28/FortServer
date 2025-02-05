using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Profile
{
    public class UpdateQuestClientObjectivesReq
    {
        public List<AdvancedCon> advance { get; set; } = new List<AdvancedCon>();
    }

    public class AdvancedCon
    {
        public string statName { get; set; } = string.Empty;
        public int count { get; set; } = 0;
        public int timestampOffset { get; set; } = 0;
    }
}

