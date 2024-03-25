using Discord;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Discord;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using MongoDB.Driver.Core.Servers;
using Newtonsoft.Json;
using System.Text;
using static FortBackend.src.App.Utilities.Classes.DiscordAuth;

namespace FortBackend.src.App.Routes.Development
{
    public class BanAndWebHooks
    {
        public static async void Init(Config DeserializeConfig, UserInfo userinfo)
        {
            string webhookUrl = DeserializeConfig.DetectedWebhookUrl;

            if(webhookUrl == "")
            {
                Logger.Error($"Webhook is null", "BanAndWebhook");
                return;
            }

            if(DiscordBot.guild != null)
            {
                var guild = DiscordBot.Client.GetGuild(DeserializeConfig.ServerID);
                if (ulong.TryParse(userinfo.id, out ulong userId))
                {
                    var user = guild.GetUser(userId);

                    if (user != null)
                    {
                        var embed = new EmbedBuilder
                        {
                            Title = "You have been banned!",
                            Description = "You were banned from Luna for attemping to bypass ban.",
                            Color = Color.Red
                        }.Build();

                        try
                        {
                            await user.SendMessageAsync(embed: embed);
                        }
                        catch { }
                        await guild.AddBanAsync(user, reason: "Trying To Bypass Ban");
                    }
                    else
                    {
                        return;
                    }
                }
            }
            var embed2 = new
            {
                title = "Auto Banned",
                footer = new { text = userinfo.username + " Has Been Banned!" },
                fields = new[]
                {
                    new { name = "UserId", value = userinfo.id.ToString(), inline = false },
                    new { name = "Reason", value = "Attempt To Bypass Ban", inline = false },
                },
                color = 0x00FFFF
            };

            string jsonPayload2 = JsonConvert.SerializeObject(new { embeds = new[] { embed2 } });

            using (var httpClient = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    HttpContent httpContent2 = new StringContent(jsonPayload2, Encoding.UTF8, "application/json");
                    try
                    {
                        HttpResponseMessage response2 = await httpClient.PostAsync(webhookUrl, formData);

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
