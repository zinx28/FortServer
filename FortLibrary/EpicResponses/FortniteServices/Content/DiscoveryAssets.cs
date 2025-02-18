using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.FortniteServices.Content
{
    public class DATAClass
    {
        public Dictionary<string, FortCreativeDiscoverySurface> DATA { get; set; } = new();
    }

    public class FortCreativeDiscoverySurface
    {
        public Meta Meta { get; set; } = new Meta();
        public Dictionary<string, object> Assets { get; set; } = new();
    }

    public class Meta
    {
        public int Promotion { get; set; }
    }
}
