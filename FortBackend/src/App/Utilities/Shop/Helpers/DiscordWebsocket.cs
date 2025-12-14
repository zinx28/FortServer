using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Shop;
using FortLibrary.XMPP;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace FortBackend.src.App.Utilities.Shop.Helpers
{
    public class DiscordWebsocket
    {

        public class DailyField
        {
            public string name { get; set; } = string.Empty;
            public string value { get; set; } = string.Empty;
        }

        public async static Task SendEmbed(SavedData dataSaved)
        {
            FortConfig DeserializeConfig = Saved.Saved.DeserializeConfig;
            if (string.IsNullOrEmpty(DeserializeConfig.ShopWebhookUrl))
            {
                Logger.Error("Shop Webhook url is missing from config!", "ItemShop");
                return;
            }

            string webhookUrl = DeserializeConfig.ShopWebhookUrl;

            var embed = new
            {
                title = "Daily:",
                fields = dataSaved.DailyFields,
                footer = new { text = "Any Issues Please Tell Us!" },
                color = 0x00FFFF
            };

            string jsonEmbed = JsonConvert.SerializeObject(embed, Formatting.Indented);

            var embed2 = new
            {
                title = "Weekly:",
                fields = dataSaved.WeeklyFields,
                footer = new { text = "Any Issues Please Tell Us!" },
                color = 0x00FFFF
            };

            string jsonEmbed1 = JsonConvert.SerializeObject(embed2, Formatting.Indented);
            string jsonPayload2 = JsonConvert.SerializeObject(new { embeds = new[] { embed2 } });
            string jsonPayload1 = JsonConvert.SerializeObject(new { embeds = new[] { embed } });

            using (var httpClient = new HttpClient())
            {
                HttpContent httpContent1 = new StringContent(jsonPayload1, Encoding.UTF8, "application/json");
                HttpContent httpContent2 = new StringContent(jsonPayload2, Encoding.UTF8, "application/json");
                try
                {
                    HttpResponseMessage response32 = await httpClient.PostAsync(webhookUrl, httpContent1);
                    HttpResponseMessage response2 = await httpClient.PostAsync(webhookUrl, httpContent2);
                    if (response2.IsSuccessStatusCode && response32.IsSuccessStatusCode)
                    {
                        Logger.Log("Message sent successfully!", "DiscWebSocket");
                    }
                    else
                    {
                        Logger.Error($"Failed to send message. Status code: {response2.StatusCode}", "DiscWebSocket");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Logger.Error($"Error sending request: {ex.Message}", "DisWebSocket");
                }
            }

        }
    }
}
