namespace FortBackend.src.App.Utilities.Helpers
{
    public class Grabber
    {
        public static async Task<int> SeasonUserAgent(HttpRequest Request)
        {
            int season = 2;
            try
            {
                var userAgent = Request.Headers["User-Agent"].ToString();


                if (!string.IsNullOrEmpty(userAgent))
                {
                    string[] userAgentParts = userAgent.Split('-');

                    if (userAgentParts.Length > 1)
                    {
                        string[] seasonParts = userAgentParts[1].Split('.');

                        if (seasonParts.Length > 0 && int.TryParse(seasonParts[0], out int parsedSeason))
                        {
                            season = parsedSeason;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("[Grabber:SeasonUserAgent] -> " + ex.Message);
            }
            return season;
        }
    }
}
