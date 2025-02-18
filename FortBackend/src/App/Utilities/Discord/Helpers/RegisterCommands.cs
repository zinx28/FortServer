using Discord;
using Discord.WebSocket;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using System;

namespace FortBackend.src.App.Utilities.Discord.Helpers
{
    public class RegisterCommands
    {
        public static async Task Connect(FortConfig config, SocketGuild guild)
        {
            //TODO REWRITE THIS

            // This overwrites everything
            //await guild.BulkOverwriteApplicationCommandAsync(Array.Empty<SlashCommandProperties>());

            var TempCommand = new SlashCommandBuilder()
                .WithName("test")
                .WithDescription("FortBackend Test Command / Check if backend is online")
                .WithContextTypes(InteractionContextType.Guild);

            var CreateCommand = new SlashCommandBuilder()
            .WithName("register")
            .WithDescription("Create an account using FortBackend")
            .WithContextTypes(InteractionContextType.Guild)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("displayname")
                .WithDescription("Choose a username for your account!")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.String))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("email")
                .WithDescription("Set an Email For Your FortBackend Account!")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.String))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("password")
                .WithDescription("Set a Password for your account!")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.String));

            var PasswordChange = new SlashCommandBuilder()
              .WithName("change_password")
              .WithDescription("Change Your FortBackend Account!")
              .WithContextTypes(InteractionContextType.Guild)
              .AddOption(new SlashCommandOptionBuilder()
                  .WithName("password")
                  .WithDescription("Choose a password for your account")
                  .WithRequired(true)
                  .WithType(ApplicationCommandOptionType.String));

            var WhoCommand = new SlashCommandBuilder()
                .WithName("who")
                .WithDescription("Find a user who has an account on FortBackend!")
                .WithContextTypes(InteractionContextType.Guild)
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
                    .WithDescription("in-game name")
                    .WithRequired(false)
                    .WithType(ApplicationCommandOptionType.String));

            //var TourEventsCommand = new SlashCommandBuilder()
            //    .WithName("events")
            //    .WithDescription("Creates a tournament")
            //    .WithContextTypes(InteractionContextType.Guild);

            //await guild.CreateApplicationCommandAsync(TourEventsCommand.Build());
            await guild.CreateApplicationCommandAsync(TempCommand.Build());
            await guild.CreateApplicationCommandAsync(WhoCommand.Build());
            await guild.CreateApplicationCommandAsync(CreateCommand.Build());
            await guild.CreateApplicationCommandAsync(PasswordChange.Build());

            Logger.Log("Registered Commamnds!", "Discord");
        }
    }
}
