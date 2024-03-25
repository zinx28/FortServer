using Discord;
using Discord.WebSocket;
using FortBackend.src.App.Utilities.Saved;
using System;

namespace FortBackend.src.App.Utilities.Discord.Helpers
{
    public class RegisterCommands
    {
        public static async Task Connect(Config config, SocketGuild guild)
        {
            

            // This overwrites everything
            //await Guild.BulkOverwriteApplicationCommandAsync(Array.Empty<SlashCommandProperties>());


            var TempCommand = new SlashCommandBuilder()
                .WithName("test")
                .WithDescription("no way walter black")
                .WithDMPermission(false);

            var WhoCommand = new SlashCommandBuilder()
                .WithName("who")
                .WithDescription("Find a user who has a account on FortBackend!")
                .WithDMPermission(false)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("Mention a user you need to search up")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Mentionable));

            await guild.CreateApplicationCommandAsync(TempCommand.Build());
            await guild.CreateApplicationCommandAsync(WhoCommand.Build());
        }
    }
}
