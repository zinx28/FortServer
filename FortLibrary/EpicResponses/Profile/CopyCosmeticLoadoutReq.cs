namespace FortLibrary.EpicResponses.Profile
{
    public class CopyCosmeticLoadoutReq
    {
        public int sourceIndex { get; set; } = 0;
        public int targetIndex { get; set; } = 1;
        public string optNewNameForTarget { get; set; } = string.Empty;
    }

    public class DeleteCosmeticLoadoutReq
    {
        public int index { get; set; } = 0;
        public int fallbackLoadoutIndex { get; set; } = -1;
        public bool leaveNullSlot { get; set; } = true;
    }
}
