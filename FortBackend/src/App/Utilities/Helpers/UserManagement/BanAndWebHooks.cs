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

namespace FortBackend.src.App.Utilities.Helpers.UserManagement
{
    public class BanAndWebHooks
    {
        public static async Task Init(FortConfig DeserializeConfig, UserInfo userinfo, string Message = "attemping to bypass ban.", string BodyMessage = "Auto Banned")
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
                        if (string.IsNullOrEmpty(userId.ToString()))
                        {
                            Logger.Error("WEIRD");
                            return;
                        }
                        var user = await DiscordBot.Client.GetUserAsync(userId);

                        if (user != null)
                        {
                            var embed = new EmbedBuilder
                            {
                                Title = "You have been banned!",
                                Description = $"You were banned from Luna for {Message}",
                                Color = Color.Red
                            }.Build();

                            Clients targetClient2 = GlobalData.Clients.FirstOrDefault(client => client.DiscordId == userId.ToString())!;
                            if (targetClient2 != null)
                            {
                                try
                                {
                                    //if (targetClient2.Launcher_Client != null && targetClient2.Launcher_Client.State == WebSocketState.Open)
                                    //{
                                    //    string message = "banned";
                                    //    byte[] buffer = Encoding.UTF8.GetBytes(message);
                                    //    await targetClient2.Launcher_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                                    //    // LAUNCHER SHOULD FORCE KILL
                                    //}

                                    if (targetClient2.Game_Client != null && targetClient2.Game_Client.State == WebSocketState.Open)
                                    {
                                        await Client.CloseClient(targetClient2.Game_Client);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"{ex.Message}");
                                }
                              
                            }
                            //    Services.Xmpp.Helpers.Send.Client.SendClientMessage(targetClient2, message);



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
