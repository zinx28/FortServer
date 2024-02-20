namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query
{
    public class ProfileChange
    {
        public string ChangeType { get; set; } = "fullProfileUpdate";
        public ProfileData Profile { get; set; }
    }
}
