using FortLibrary.EpicResponses.Storefront;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Dynamics
{
    public class StoreBattlepass
    {
        public string name { get; set; } = string.Empty;
        public List<catalogEntrie> catalogEntries { get; set; } = new List<catalogEntrie>();

    }

    public class StoreBattlepassPages
    {
        public string name { get; set; } = string.Empty;
        public List<catalogEntrieStore> catalogEntries { get; set; } = new List<catalogEntrieStore>();

    }
}
