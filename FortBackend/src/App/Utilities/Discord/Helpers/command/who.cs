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

        public static async void CheckAccount(SocketSlashCommand command, SocketGuildUser user, string ign, string id)
        {
            string UserId = string.Empty;
            string UserName = string.Empty;
            if(user == null)
            {
                if(ign == null)
                {
                    if (ulong.TryParse(id, out ulong userId))
                    {
                        var userfound = await DiscordBot.Client.Rest.GetUserAsync(userId);
                        UserId = id;
                        UserName = userfound.Username;
                    }
                }else
                {
                    UserName = ign;
                }
            }else
            {
                UserId = user.Id.ToString();
                UserName = user.Username;
            }
            var FindDiscordID = string.Empty;
            if (UserId == null || ign != null)
            {
                FindDiscordID = await Handlers.FindOne<User>("Username", ign);
            }
            else
            {
                FindDiscordID = await Handlers.FindOne<User>("DiscordId", UserId);
            }

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
                    .WithIsInline(false);

                var MentionUserField = new EmbedFieldBuilder()
                    .WithName("Discord")
                    .WithValue($"<@{RespondBack.DiscordId}>")
                    .WithIsInline(false);

                var embed = new EmbedBuilder()
                    .WithTitle("Player: " + RespondBack.Username)
                    .AddField(WhoIsField)
                    .AddField(MentionUserField)
                    .WithColor(Color.Blue)
                    .WithCurrentTimestamp();

                await command.RespondAsync(embed: embed.Build(), ephemeral: true);
            }
            else
            {
                var embed = new EmbedBuilder()
                    .WithDescription(UserName + " is not found")
                    .WithColor(Color.Blue)
                    .WithCurrentTimestamp();

                await command.RespondAsync(embed: embed.Build(), ephemeral: true);
            }
            
        }
        public static async Task Respond(SocketSlashCommand command)
        {
            try
            {
                if (command.Data.Options.FirstOrDefault(o => o.Name == "mention")?.Value != null)
                {
                    var username = command.Data.Options.FirstOrDefault(o => o.Name == "mention")?.Value as SocketGuildUser;
                    if (username != null)
                    {
                        CheckAccount(command, username, null, null);
                    }
                    else
                    {
                        await command.RespondAsync("The user is not found!", ephemeral: true);
                    }
                }
                else if (command.Data.Options.FirstOrDefault(o => o.Name == "id")?.Value != null)
                {
                    CheckAccount(command, null, null, command.Data.Options.FirstOrDefault(o => o.Name == "id")?.Value?.ToString());
                }
                else if (command.Data.Options.FirstOrDefault(o => o.Name == "ign")?.Value != null)
                {
                    CheckAccount(command, null, command.Data.Options.FirstOrDefault(o => o.Name == "ign")?.Value?.ToString(), null);
                }
                else
                {
                    await command.RespondAsync("Whoops make sure you selected atleast one thing", ephemeral: true);
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
