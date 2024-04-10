using Discord;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Discord;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.XMPP_Server.Globals;
using MongoDB.Driver.Core.Servers;
using Newtonsoft.Json;
using System.Text;
using static FortLibrary.DiscordAuth;

namespace FortBackend.src.App.Utilities.Helpers.UserManagement
{
    public class BanAndWebHooks
    {
        public static async Task Init(Config DeserializeConfig, UserInfo userinfo, string Message = "attemping to bypass ban.", string BodyMessage = "Auto Banned")
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
                        var user = await DiscordBot.Client.GetUserAsync(userId);

                        if (user != null)
                        {
                            var embed = new EmbedBuilder
                            {
                                Title = "You have been banned!",
                                Description = $"You were banned from Luna for {Message}",
                                Color = Color.Red
                            }.Build();

                            try
                            {
                                var dmChannel = await user.CreateDMChannelAsync();
                                if (dmChannel != null)
                                {
                                    await dmChannel.SendMessageAsync(embed: embed);
                                }
                            }
                            catch { }
                            try
                            {
                                await guild.AddBanAsync(user, reason: Message);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message);
                            }
                        }
                        else
                        {
                            Logger.Error("Returning id is false");
                            return;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Why is this null");
                }

                var embed2 = new
                {
                    title = "User Banned",
                    footer = new { text = userinfo.username + " Has Been Banned!" },
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
