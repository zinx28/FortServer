using FortBackend.src.App.SERVER.Root;
using FortLibrary;
using Newtonsoft.Json;
using System.Text;
using static FortLibrary.DiscordAuth;

namespace FortBackend.src.App.Utilities.ADMIN
{
    public class NewAdmin
    {
        public static async void Send(string username, AdminData adminData)
        {
            string webhookUrl = Saved.Saved.DeserializeConfig.DetectedWebhookUrl;

            if (webhookUrl == "")
            {
                Logger.Error($"Webhook is null", "BanAndWebhook");
            }
            else
            {
                var embed2 = new
                {
                    title = "Added Moderator",
                    footer = new { text = $"Added by {adminData.AdminUserName}" },
                    fields = new[]
                    {
                        new { name = "Display Name", value = username, inline = false },
                    },
                    color = 0x00FFFF
                };

                string jsonPayload2 = JsonConvert.SerializeObject(new { embeds = new[] { embed2 } });

                using (var httpClient = new HttpClient())
                {

                    HttpContent httpContent2 = new StringContent(jsonPayload2, Encoding.UTF8, "application/json");
                    try
                    {
                        HttpResponseMessage response2 = await httpClient.PostAsync(webhookUrl, httpContent2);

                        if (!response2.IsSuccessStatusCode)
                        {
                            Logger.Error($"Failed to send message. Status code: {response2.StatusCode}", "BanAndWebhook");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Error($"Error sending request: {ex.Message}", "BanAndWebhook");
                    }
                }
            }
        }
    }
}
