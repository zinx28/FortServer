using Discord;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Discord;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using MongoDB.Driver.Core.Servers;
using Newtonsoft.Json;
using System.Text;
using static FortLibrary.DiscordAuth;
using FortLibrary.ConfigHelpers;

namespace FortBackend.src.App.Utilities.Helpers.UserManagement
{
    public class unbanAndWebhooks
    {
        public static async Task Init(FortConfig DeserializeConfig, UserInfo userinfo, string Message = "false ban", string BodyMessage = "Moderator")
        {
            try
            {
                string webhookUrl = DeserializeConfig.DetectedWebhookUrl;

                if (webhookUrl == "")
                {
                    Logger.Error($"Webhook is null", "BanAndWebhook");
                    return;
                }

                if (DiscordBot.guild != null)
                {
                    var guild = DiscordBot.Client.GetGuild(DeserializeConfig.ServerID);
                    if (ulong.TryParse(userinfo.id, out ulong userId))
                    {
                        try
                        {
                            RequestOptions NoWay = new RequestOptions();
                            NoWay.AuditLogReason = Message;

                            await guild.RemoveBanAsync(userId, NoWay);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Why is this null");
                }

                var embed2 = new
                {
                    title = "Ban Revoked",
                    footer = new { text = userinfo.username + " Has Been Unbanned!" },
                    fields = new[]
                    {
                        new { name = "Display Name", value = userinfo.username ?? "Couldn't find username?", inline = false },
                        new { name = "User Id", value = userinfo.id.ToString(), inline = false },
                        new { name = "Reason", value = Message, inline = false },
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
