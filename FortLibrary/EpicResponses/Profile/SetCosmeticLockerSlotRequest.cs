using FortLibrary.EpicResponses.Profile.Query.Items;

namespace FortLibrary.EpicResponses.Profile
{
    public class SetCosmeticLockerSlotRequest
    {
        public string lockerItem { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public string itemToSlot { get; set; } = string.Empty;
        public int slotIndex { get; set; }
        public List<AthenaItemVariants> variantUpdates { get; set; } = new List<AthenaItemVariants>();
        public int optLockerUseCountOverride { get; set; }
    }
}
