using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Fortnite
{
    public class StatsBody
    {
        public string appId { get; set; } = string.Empty;
        public long startDate { get; set; } = 0;
        public long endDate { get; set; } = 0;
        public string[] owners { get; set; } = new string[0];
        public string[] stats { get; set; } = new string[0];
    }
}
