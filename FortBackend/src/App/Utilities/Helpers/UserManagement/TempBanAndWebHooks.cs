using Discord;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Discord;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using MongoDB.Driver.Core.Servers;
using Newtonsoft.Json;
using System.Text;
using static FortLibrary.DiscordAuth;
using FortBackend.src.App.SERVER.Root;
using FortBackend.src.App.SERVER.Send;
using System.Xml.Linq;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using System.Net.WebSockets;
using FortLibrary.ConfigHelpers;
using FortLibrary;

namespace FortBackend.src.App.Utilities.Helpers.UserManagement
{
    public class TempBanAndWebHooks
    {
        public static async Task Init(FortConfig DeserializeConfig, UserInfo userinfo, string Message = "attemping to bypass ban.", string BodyMessage = "Auto Banned", string HowLong = "Exploiting")
        {
            try
            {
                string webhookUrl = DeserializeConfig.DetectedWebhookUrl;

                if (webhookUrl == "")
                {
                    Logger.Error($"Webhook is null", "BanAndWebhook");
                    return;
                }
                          

                var embed2 = new
                {
                    title = "User Temp Banned",
                    footer = new { text = userinfo.username + " Has Been Temp Banned!" },
                    fields = new[]
                    {
                        new { name = "Display Name", value = userinfo.username ?? "Couldn't find username?", inline = false },
                        new { name = "User Id", value = userinfo.id.ToString(), inline = false },
                        new { name = "Reason", value = Message, inline = false },
                        new { name = "How Long", value = HowLong, inline = false},
                        new { name = "Staff", value = BodyMessage, inline = false }
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
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "BanAndWebhooks");
            }
        }
    }
}
