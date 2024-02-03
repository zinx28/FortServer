using Microsoft.AspNetCore.Mvc;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Saved;
using Newtonsoft.Json;

namespace FortBackend.src.App.Routes.APIS.Development
{
    [ApiController]
    [Route("temp")]
    public class TempController : ControllerBase
    {
        [HttpGet("yeah69")]
        public IActionResult YeahImsoGayyy()
        {
            return Ok(new { });
        }

        [HttpGet("/callback")]
        public async Task<IActionResult> CallBack([FromQuery] string code)
        {
            try
            {
                Config config = Saved.DeserializeConfig;

                if(string.IsNullOrEmpty(config.ApplicationClientID) || string.IsNullOrEmpty(config.ApplicationURI) || string.IsNullOrEmpty(config.ApplicationSecret))
                {
                    return Ok(new { test = "Blank Application Info" });
                }
                var Client = new HttpClient();
                var formData = new Dictionary<string, string>()
                {
                    { "client_id", config.ApplicationClientID },
                    { "client_secret", config.ApplicationSecret },
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", config.ApplicationURI }
                };
                var content = new FormUrlEncodedContent(formData);
                var response = await Client.PostAsync("https://discord.com/api/oauth2/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
                if(responseData == null)
                {
                    return Ok(new { test = "Null!" });
                }

                if (responseData.TryGetValue("access_token", out var accessToken))
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    response = await client.GetAsync("https://discord.com/api/users/@me");
                    responseContent = await response.Content.ReadAsStringAsync();

                    dynamic responseData1 = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    var username = responseData1.username;
                    var id = responseData1.id;
                    var email = responseData1.email;

                    // create acc? ig can't erlaly be bothered rn

                    //https://discord.com/api/users/@me
                    Console.WriteLine(responseContent);
                }
             
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return Ok(new { test = "ngl" });
        }
    }
}
