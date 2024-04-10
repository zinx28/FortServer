namespace FortLibrary.EpicResponses.Profile.Query
{
    public class ProfileChange
    {
        public string ChangeType { get; set; } = "fullProfileUpdate";
        public ProfileData Profile { get; set; } = new ProfileData();
    }
}
