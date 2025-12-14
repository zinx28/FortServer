using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using System.IO;
using FortLibrary.ConfigHelpers;
using FortLibrary;
using System;

namespace FortBackend.src.App.Utilities.Helpers
{
    public class Grabber
    {
        public static Dictionary<int, float> Versions = new Dictionary<int, float>
        {
            { 3541083, 1.2f },
            { 3681159, 1.6f },
            { 3700114, 1.72f },
            { 3724489, 1.8f },
            { 3757339, 1.9f },
            { 3775276, 1.91f },
            { 3785438, 1.9f },
            { 3807424, 1.11f },
            { 3825894, 2.1f }
        };

        public class VersionClass
        {
            public int Season { get; set; } = 1;
            public float SeasonFull { get; set; } = 1;
            public int VersionID { get; set; } = -1;
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

                        int versionid = -1;
                        try
                        {
                            if (userAgentParts.Length > 3)
                            {
                                var potentialBuildId = userAgentParts[3].Split(',')[0];
                                if (int.TryParse(potentialBuildId, out versionid))
                                    VersionIG.VersionID = versionid;
                                else
                                {
                                    potentialBuildId = userAgentParts[3].Split(' ')[0];
                                    if (int.TryParse(potentialBuildId, out versionid))
                                        VersionIG.VersionID = versionid;
                                }
                            }
                        }
                        catch
                        {
                            try
                            {
                                var potentialBuildId = userAgent.Split('-')[1].Split('+')[0];
                                if (int.TryParse(potentialBuildId, out versionid))
                                    VersionIG.VersionID = versionid;
                            }
                            catch { }
                        }

                        if (userAgentParts.Length > 1)
                        {
                            //Console.WriteLine(userAgentParts[0]);
                            //Console.WriteLine("TEST " + VersionIG.VersionID);
            
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
                                        VersionIG.Season = (int)saved.Season;
                                    }
                                }
                                else
                                {
                                    
                                    if (VersionIG.VersionID < 3790078)
                                    {
                                        VersionIG.Season = 0;
                                        VersionIG.SeasonFull = 0.0f;
                                    }
                                    else if (VersionIG.VersionID <= 3790078) // Release-Cert-CL-3790078 (1.10)
                                    {
                                        VersionIG.Season = 1;
                                        VersionIG.SeasonFull = 1;
                                    }
                                    else
                                    {
                                        VersionIG.Season = 2;
                                        VersionIG.SeasonFull = 2;
                                    }

                                    // if i'm missing a version
                                    if (Versions.TryGetValue(VersionIG.VersionID, out float version))
                                    {
                                        VersionIG.SeasonFull = version;
                                    }

                                    //Console.WriteLine("231 " + VersionIG.SeasonFull);
                                    //if (seasonParts[0].Contains("Cert"))
                                    // {
                                    //     VersionIG.Season = 1;
                                    //  }
                                    // else if (seasonParts[0].Contains("Next"))
                                    //{
                                    //   VersionIG.Season = 2;
                                    //}
                                }
                            }
                        }
                    }

                    if (saved.ForceSeason)
                    {
                        VersionIG.Season = (int)saved.Season;
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
