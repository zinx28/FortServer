

using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items
{
    public class SandboxLoadout
    {
        [JsonProperty("templateId")]
        public string templateId { get; set; } = string.Empty;
        [JsonProperty("attributes")]
        public SandboxLoadoutAttributes attributes { get; set; } = new SandboxLoadoutAttributes();
        [JsonProperty("quantity")]
        public int quantity { get; set; } = 0;
    }

    public class SandboxLoadoutSlots
    {
        [JsonProperty("slots")]
        public LockerSlotsData slots { get; set; } = new LockerSlotsData();
    }

    public class SandboxLoadoutAttributes
    {
        [JsonProperty("locker_slots_data")]
        public SandboxLoadoutSlots locker_slots_data { get; set; } = new SandboxLoadoutSlots();
        [JsonProperty("banner_icon_template")]
        public string banner_icon_template { get; set; } = string.Empty;
        [JsonProperty("banner_color_template")]
        public string banner_color_template { get; set; } = string.Empty;
        [JsonProperty("locker_name")]
        public string locker_name { get; set; } = string.Empty;
        [JsonProperty("favorite")]
        public bool favorite { get; set; } = false;
        [JsonProperty("use_count")]
        public int use_count { get; set; } = 1;
        [JsonProperty("item_seen")]
        public bool item_seen { get; set; } = true;


    }

    public class LockerSlotsData
    {
        [JsonProperty("musicpack")]
        public Slots musicpack { get; set; } = new Slots();
        [JsonProperty("character")]
        public Slots character { get; set; } = new Slots();
        [JsonProperty("backpack")]
        public Slots backpack { get; set; } = new Slots();
        [JsonProperty("pickaxe")]
        public Slots pickaxe { get; set; } = new Slots();
        [JsonProperty("skydivecontrail")]
        public Slots skydivecontrail { get; set; } = new Slots();
        [JsonProperty("dance")]
        public Slots dance { get; set; } = new Slots();
        [JsonProperty("loadingscreen")]
        public Slots loadingscreen { get; set; } = new Slots();
        [JsonProperty("glider")]
        public Slots glider { get; set; } = new Slots();
        [JsonProperty("itemwrap")]
        public Slots itemwrap { get; set; } = new Slots();

        public Slots GetSlotName(string input)
        {
            switch (input.ToLower())
            {
                case "musicpack":
                    return musicpack;
                case "character":
                    return character;
                case "backpack":
                    return backpack;
                case "pickaxe":
                    return pickaxe;
                case "skydivecontrail":
                    return skydivecontrail;
                case "dance":
                    return dance;
                case "loadingscreen":
                    return loadingscreen;
                case "glider":
                    return glider;
                case "itemwrap":
                    return itemwrap;
                default:
                    throw new ArgumentException($"Slot '{input}' not found.");
            }
        }
    }


    public class Slots
    {
        [JsonProperty("items")]
        public List<string> items { get; set; } = new List<string>() { "" };
        [JsonProperty("activeVariants")]
        public List<object> activeVariants { get; set; } = new List<object>();
    }

    public class Loadout
    {
        [JsonProperty("templateId")]
        public string templateId { get; set; } = string.Empty;
        [JsonProperty("attributes")]
        public SandboxLoadoutAttributes attributes { get; set; } = new SandboxLoadoutAttributes();
        [JsonProperty("quantity")]
        public int quantity { get; set; }
    }
}
