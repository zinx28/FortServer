using Discord;
using Discord.WebSocket;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.ConfigHelpers;
using System;

namespace FortBackend.src.App.Utilities.Discord.Helpers
{
    public class RegisterCommands
    {
        public static async Task Connect(FortConfig config, SocketGuild guild)
        {
            

            // This overwrites everything
            //await Guild.BulkOverwriteApplicationCommandAsync(Array.Empty<SlashCommandProperties>());


            var TempCommand = new SlashCommandBuilder()
                .WithName("test")
                .WithDescription("FortBackend Test Command / Check if backend is online")
                .WithDMPermission(false);

            var WhoCommand = new SlashCommandBuilder()
                .WithName("who")
                .WithDescription("Find a user who has a account on FortBackend!")
                .WithDMPermission(false)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("mention")
                    .WithDescription("Mention a user you need to search up")
                    .WithRequired(false)
                    .WithType(ApplicationCommandOptionType.Mentionable))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("id")
                    .WithDescription("discord id")
                    .WithRequired(false)
                    .WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("ign")
                    .WithDescription("ingame name")
                    .WithRequired(false)
                    .WithType(ApplicationCommandOptionType.String));

            await guild.CreateApplicationCommandAsync(TempCommand.Build());
            await guild.CreateApplicationCommandAsync(WhoCommand.Build());
        }
    }
}
