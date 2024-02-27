using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using System.IO;

namespace FortBackend.src.App.Utilities.Helpers
{
    public class Grabber
    {
        public class VersionClass
        {
            public int Season { get; set; } = 2;
            public float SeasonFull { get; set; } = 2;
        }
        public static async Task<VersionClass> SeasonUserAgent(HttpRequest Request)
        {
            VersionClass VersionIG = new VersionClass();
            try
            {
                Config saved = Saved.Saved.DeserializeConfig;
                var userAgent = Request.Headers["User-Agent"].ToString();


                if (!string.IsNullOrEmpty(userAgent))
                {
                    string[] userAgentParts = userAgent.Split('-');

                    if (userAgentParts.Length > 1)
                    {
                        string[] seasonParts = userAgentParts[1].Split('.');

                        if(seasonParts.Length > 0)
                        {
                            if (float.TryParse(seasonParts[0], out float parsedSeason1) && float.TryParse(seasonParts[1], out float parsedSeason2))
                            {
                                VersionIG.SeasonFull = float.Parse($"{parsedSeason1}.{parsedSeason2}");
                            }

                            if (int.TryParse(seasonParts[0], out int parsedSeason))
                            {
                                if (saved.ForceSeason == false)
                                {
                                    VersionIG.Season = parsedSeason;
                                }
                                else
                                {
                                    VersionIG.Season = saved.Season;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("[Grabber:SeasonUserAgent] -> " + ex.Message);
            }
            return VersionIG;
        }
    }
}
