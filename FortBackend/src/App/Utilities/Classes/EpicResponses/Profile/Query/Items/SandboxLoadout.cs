

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items
{
    public class SandboxLoadout
    {
        public string templateId { get; set; } = string.Empty;
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
        public string banner_icon_template { get; set; } = string.Empty;
        public string banner_color_template { get; set; } = string.Empty;
        public string locker_name { get; set; } = string.Empty;
        public bool favorite { get; set; } = false;
        public int use_count { get; set; } = 1;
        public bool item_seen { get; set; } = true;


    }

    public class LockerSlotsData
    {
        public Slots musicpack { get; set; } = new Slots();
        public Slots character { get; set; } = new Slots();
        public Slots backpack { get; set; } = new Slots();
        public Slots pickaxe { get; set; } = new Slots();
        public Slots skydivecontrail { get; set; } = new Slots();
        public Slots dance { get; set; } = new Slots();
        public Slots loadingscreen { get; set; } = new Slots();
        public Slots glider { get; set; } = new Slots();
        public Slots itemwrap { get; set; } = new Slots();
    }

    public class Slots
    {
        public List<string> items { get; set; } = new List<string>();

        public List<object> activeVariants { get; set; } = new List<object>();
    }

    public class Loadout
    {
        public string templateId { get; set; } = string.Empty;
        public SandboxLoadoutAttributes attributes { get; set; } = new SandboxLoadoutAttributes();
        public int quantity { get; set; }
    }
}
