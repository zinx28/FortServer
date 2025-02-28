using Discord;
using Discord.WebSocket;
using FortBackend.src.App.Utilities.MongoDB.Helpers;

namespace FortBackend.src.App.Utilities.Discord.Helpers.command
{
    public class Details
    {
        public static async Task Respond(SocketSlashCommand command)
        {
            var FindDiscordID = await GrabData.ProfileDiscord(command.User.Id.ToString());
            if (FindDiscordID == null || string.IsNullOrEmpty(FindDiscordID.AccountId))
            {
                var embed = new EmbedBuilder()
                .WithTitle("Failed to find account!")
                .WithDescription("You don't have a FortBackend Account.")
                .WithColor(Color.Blue)
                .WithCurrentTimestamp();

                await command.RespondAsync(embed: embed.Build(), ephemeral: true);
            }
            else
            {

                var DisplayNameField = new EmbedFieldBuilder()
                    .WithName("Display Name")
                    .WithValue($"{FindDiscordID.UserData.Username}")
                    .WithIsInline(false);

                var EmailField = new EmbedFieldBuilder()
                    .WithName("Email")
                    .WithValue($"||{FindDiscordID.UserData.Email}||")
                    .WithIsInline(false);

                var embed = new EmbedBuilder()
                  .WithTitle("Your details!")
                  .AddField(DisplayNameField)
                  .AddField(EmailField)
                  .WithColor(Color.Blue)
                  .WithCurrentTimestamp();

                await command.RespondAsync(embed: embed.Build(), ephemeral: true);
            }
        }
    }
}
