namespace FortBackend.src.App.Utilities.MongoDB.Extentions
{
    public interface IProfileRevisions
    {
        DateTime Updated { get; set; }
        int RVN { get; set; }
        int CommandRevision { get; set; }
    }

    public static class ProfileRevisionExtensions
    {
        public static void BumpRevisions(this IProfileRevisions profile)
        {
            profile.Updated = DateTime.UtcNow;
            profile.RVN++;
            profile.CommandRevision++;
        }

        public static int GetBaseRevision(this IProfileRevisions profile, int Season)
        {
            return Season >= 17
                ? profile.CommandRevision
                : profile.RVN;
        }
    }
}
