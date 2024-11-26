using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using System.IO;
using FortLibrary.ConfigHelpers;
using FortLibrary;

namespace FortBackend.src.App.Utilities.Helpers
{
    public class Grabber
    {
        public class VersionClass
        {
            public int Season { get; set; } = 1;
            public float SeasonFull { get; set; } = 1;
        }
        public static async Task<VersionClass> SeasonUserAgent(HttpRequest Request)
        {
            return await Task.Run(() =>
            {
                VersionClass VersionIG = new VersionClass();
                try
                {
                    FortGameConfig saved = Saved.Saved.DeserializeGameConfig;
                    var userAgent = Request.Headers["User-Agent"].ToString();


                    if (!string.IsNullOrEmpty(userAgent))
                    {
                        string[] userAgentParts = userAgent.Split('-');
                        //Console.WriteLine(userAgent);
                        if (userAgentParts.Length > 1)
                        {
                            string[] seasonParts = userAgentParts[1].Split('.');

                            if (seasonParts.Length > 0)
                            {
                                if (float.TryParse(seasonParts[0], out float parsedSeason1) && float.TryParse(seasonParts[1], out float parsedSeason2))
                                {
                                    VersionIG.SeasonFull = float.Parse($"{parsedSeason1}.{parsedSeason2}");
                                }
                                //Console.WriteLine(seasonParts[0]);
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
                                else
                                {
                                    if (seasonParts[0].Contains("Cert"))
                                    {
                                        VersionIG.Season = 1;
                                    }
                                    else if (seasonParts[0].Contains("Next"))
                                    {
                                        VersionIG.Season = 2;
                                    }
                                }
                            }
                        }
                    }

                    if (saved.ForceSeason)
                    {
                        VersionIG.Season = saved.Season;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("[Grabber:SeasonUserAgent] -> " + ex.Message);
                }
                return VersionIG;
            });
        }
    }
}
