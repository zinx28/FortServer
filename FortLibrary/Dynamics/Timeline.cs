using FortLibrary.EpicResponses.Storefront;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Dynamics
{
    public class Timeline
    {
        public List<ActiveEventData> ClientEvents { get; set; } = new List<ActiveEventData>();
    }
}
