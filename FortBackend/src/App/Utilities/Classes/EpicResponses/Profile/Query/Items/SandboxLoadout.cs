

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items
{
    public class SandboxLoadout
    {
        public string templateId { get; set; } = "0";
        public SandboxLoadoutAttributes attributes { get; set; } = new SandboxLoadoutAttributes();
        public int quantity { get; set; } = 0;
    }

    public class SandboxLoadoutSlots
    {
        public LockerSlotsData slots { get; set; } = new LockerSlotsData();
    }

    public class SandboxLoadoutAttributes
    {
        public SandboxLoadoutSlots locker_slots_data { get; set; } = new SandboxLoadoutSlots();
        public int use_count { get; set; }
        public string banner_icon_template { get; set; } = "notproper";
        public string banner_color_template { get; set; } = "notproper";
        public string locker_name { get; set; } = "notproper";
        public bool item_seen { get; set; }
        public bool favorite { get; set; }
    }

    public class LockerSlotsData
    {
        public Slots MusicPack { get; set; } = new Slots();
        public Slots Character { get; set; } = new Slots();
        public Slots Backpack { get; set; } = new Slots();
        public Slots SkyDiveContrail { get; set; } = new Slots();
        public Slots Dance { get; set; } = new Slots();
        public Slots LoadingScreen { get; set; } = new Slots();
        public Slots Pickaxe { get; set; } = new Slots();
        public Slots Glider { get; set; } = new Slots();
        public Slots ItemWrap { get; set; } = new Slots();
    }

    public class Slots
    {
        public List<string> items { get; set; } = new List<string>();
        public List<string> activeVariants { get; set; } = new List<string>();
    }

    public class Loadout
    {
        public string templateId { get; set; } = "notproper";
        public SandboxLoadoutAttributes attributes { get; set; } = new SandboxLoadoutAttributes();
        public int quantity { get; set; }
    }
}
