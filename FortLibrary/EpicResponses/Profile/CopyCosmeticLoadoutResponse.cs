namespace FortLibrary.EpicResponses.Profile
{
    public class CopyCosmeticLoadoutResponse
    {
        public int sourceIndex { get; set; } = 0;
        public int targetIndex { get; set; } = 1;
        public string optNewNameForTarget { get; set; } = "Couldn't grab name!!";
    }
}
