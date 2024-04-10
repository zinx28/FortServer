namespace FortLibrary.EpicResponses.Profile
{
    public class BulkEquipBattleRoyaleCustomizationResponse
    {
        public List<LoadoutData> LoadoutData { get; set; } = new List<LoadoutData>(); // default object
    }

    public class LoadoutData
    {
        public string slotName { get; set; } = string.Empty;
        public string itemToSlot { get; set; } = string.Empty;
        public int indexWithinSlot { get; set; } = 0;
    }
}
