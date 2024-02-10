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
                    try
                    {
                        string[] userAgentParts = userAgent.Split("-");
                        string[] seasonParts = userAgentParts[1].Split(".");
                        season = int.Parse(seasonParts[0]);
                    }
                    catch
                    {
                        season = 2;
                    }
                }
                else
                {
                    season = 2;
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
