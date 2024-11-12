using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.ConfigHelpers
{
    public class ItemPricing
    {
        public Skins skins { get; set; }
        public Pickaxes pickaxes { get; set; }
        public Gliders gliders { get; set; }
        public Wrap wrap { get; set; }
        public Emotes emotes { get; set; }
    }

    public class Skins
    {
        public int uncommon { get; set; }
        public int rare { get; set; }
        public int epic { get; set; }
        public int legendary { get; set; }
        public int dark { get; set; }
        public int dc { get; set; }
        public int gaminglegends { get; set; }
        public int frozen { get; set; }
        public int lava { get; set; }
        public int marvel { get; set; }
        public int shadow { get; set; }
        public int icon { get; set; }
    }

    public class Pickaxes
    {
        public int uncommon { get; set; }
        public int rare { get; set; }
        public int epic { get; set; }
        public int icon { get; set; }
        public int dark { get; set; }
        public int marvel { get; set; }
    }

    public class Gliders
    {
        public int uncommon { get; set; }
        public int rare { get; set; }
        public int epic { get; set; }
        public int legendary { get; set; }
        public int icon { get; set; }
        public int dc { get; set; }
        public int dark { get; set; }
    }

    public class Wrap
    {
        public int uncommon { get; set; }
        public int rare { get; set; }
        public int epic { get; set; }
    }

    public class Emotes
    {
        public int uncommon { get; set; }
        public int rare { get; set; }
        public int epic { get; set; }
        public int icon { get; set; }
        public int marvel { get; set; }
        public int dc { get; set; }
    }
}
