using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Dynamics
{
    public class BattlePass
    {
        public List<ItemInfo> Rewards = new List<ItemInfo>();
        public int Level { get; set; } = 0;
    }

    public class ItemInfo
    {
        public string TemplateId { get; set; } = string.Empty;
        public int Level { get; set; } = 0;
        public int Quantity { get; set; } = 0;
    }
}
