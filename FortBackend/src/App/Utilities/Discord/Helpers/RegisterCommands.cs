using Discord;
using FortBackend.src.App.Utilities.Saved;

namespace FortBackend.src.App.Utilities.Discord.Helpers
{
    public class RegisterCommands
    {
        public static async Task Connect(Config config)
        {
            var Guild = DiscordBot.Client.GetGuild(config.ServerID);

            // This overwrites everything
            //await Guild.BulkOverwriteApplicationCommandAsync(Array.Empty<SlashCommandProperties>());


            var TempCommand = new SlashCommandBuilder()
                .WithName("test")
                .WithDescription("no way walter black")
                .WithDMPermission(false);

            await Guild.CreateApplicationCommandAsync(TempCommand.Build());
        }
    }
}
