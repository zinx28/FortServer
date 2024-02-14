using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using FortBackend.src.App.Utilities.Saved;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace FortBackend.src.App.Utilities.Shop.Helpers
{
    public class DiscordWebsocket
    {

        public class DailyField
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public async static void SendEmbed(/*SavedData dataSaved*/)
        {
            Config DeserializeConfig = Saved.Saved.DeserializeConfig;
            if (string.IsNullOrEmpty(DeserializeConfig.ShopWebhookUrl))
            {
                Logger.Error("Shop Webhook url is missing from config!");
                return;
            }
            string webhookUrl = DeserializeConfig.ShopWebhookUrl;
            var OutPutFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "output.png");
            byte[] imageData = File.ReadAllBytes(OutPutFile);
            ByteArrayContent imageContent = new ByteArrayContent(imageData);
            var embed2 = new
            {
                title = "Fortnite test:",
                footer = new { text = "Any Issues Please Tell Us! ~ this is just for testing dw" },
                image = new
                {
                    url = "attachment://image.png",
                },
                color = 0x00FFFF
            };

            string jsonEmbed1 = JsonConvert.SerializeObject(embed2, Formatting.Indented);
            Console.WriteLine(jsonEmbed1);

            string jsonPayload2 = JsonConvert.SerializeObject(new { embeds = new[] { embed2 } });

            using (var httpClient = new HttpClient())

            {
                using (var formData = new MultipartFormDataContent())
                {
                    HttpContent httpContent2 = new StringContent(jsonPayload2, Encoding.UTF8, "application/json");
                    try
                    {
                        formData.Add(httpContent2, "payload_json");
                      
                        ///formData.Add(imageContent, "image", "image.jpg");
                       // byte[] imageData = File.ReadAllBytes(OutPutFile);
                       // ByteArrayContent imageContent = new ByteArrayContent(imageData);
                        formData.Add(imageContent, "image", "image.png");

                        HttpResponseMessage response2 = await httpClient.PostAsync(webhookUrl + "?payload_json=" + Uri.EscapeDataString(jsonPayload2), formData);

                        if (response2.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Message sent successfully!");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to send message. Status code: {response2.StatusCode}");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Error sending request: {ex.Message}");
                    }
                }
            }


        }
    }
}
