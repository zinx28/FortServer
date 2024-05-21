namespace FortMatchmaker.src.App.Utilities.Constants
{
    public class PathConstants
    {
        public static readonly string BaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        public static readonly string LocalAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Saved.DeserializeConfig.ProjectName);

    }
}
