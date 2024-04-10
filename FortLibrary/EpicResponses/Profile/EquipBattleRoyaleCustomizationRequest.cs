namespace FortLibrary.EpicResponses.Profile
{
    public class EquipBattleRoyaleCustomizationRequest
    {
        public string slotName { get; set; } = string.Empty;
        public string itemToSlot { get; set; } = string.Empty;
        public int indexWithinSlot { get; set; } = 0;
        public List<object> variantUpdates { get; set; } = new List<object>();
    }
}
