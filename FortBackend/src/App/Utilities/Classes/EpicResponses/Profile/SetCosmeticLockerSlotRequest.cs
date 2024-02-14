namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile
{
    public class SetCosmeticLockerSlotRequest
    {
        public string lockerItem { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public string itemToSlot { get; set; } = string.Empty;
        public int slotIndex { get; set; }
        public List<object> variantUpdates { get; set; } = new List<object>();
        public int optLockerUseCountOverride { get; set; }
    }
}
