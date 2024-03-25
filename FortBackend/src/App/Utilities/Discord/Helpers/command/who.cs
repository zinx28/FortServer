using Discord;
using Discord.WebSocket;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Classes.DiscordAuth;

namespace FortBackend.src.App.Utilities.Discord.Helpers.command
{
    public class who
    {
        public static async Task Respond(SocketSlashCommand command)
        {
            try
            {
                var username = command.Data.Options.First(o => o.Name == "user")?.Value as SocketGuildUser;
                if (username != null)
                {
                    var FindDiscordID = await Handlers.FindOne<User>("DiscordId", username.Id);
                    if (FindDiscordID != "Error")
                    {
                        User RespondBack = JsonConvert.DeserializeObject<User[]>(FindDiscordID)?[0];
                        if (RespondBack == null)
                        {
                            await command.RespondAsync("Backend issue ;(((", ephemeral: true);
                            return;
                        }

                        var WhoIsField = new EmbedFieldBuilder()
                          .WithName("Banned")
                          .WithValue(RespondBack.banned.ToString())
                          .WithIsInline(true);

                        var embed = new EmbedBuilder()
                            .WithTitle("Player: " + RespondBack.Username)
                            .AddField(WhoIsField)
                            .WithColor(Color.Blue)
                            .WithCurrentTimestamp();

                        await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                    }
                    else
                    {
                        var embed = new EmbedBuilder()
                           .WithDescription(username.Username + " is not found")
                           .WithColor(Color.Blue)
                           .WithCurrentTimestamp();

                        await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                    }
                }
                else
                {
                    await command.RespondAsync("The user is not found!", ephemeral: true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                await command.RespondAsync("error!", ephemeral: true);
            }
        }
    }
}
