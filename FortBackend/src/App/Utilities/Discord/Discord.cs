using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FortBackend.src.App.Utilities.Discord.Helpers;
using FortBackend.src.App.Utilities.Saved;
using System;

namespace FortBackend.src.App.Utilities.Discord
{
    public class DiscordBot
    {
        
        public static DiscordSocketClient Client { get; private set; } = new DiscordSocketClient();
        private static CommandService CommandService = new CommandService();
        public static SocketGuild guild { get; set; } = null!;
        public static async Task Start()
        {
            Logger.Log("Initializing Discord", "Discord");
            Config DeserializeConfig = Saved.Saved.DeserializeConfig;

            if(string.IsNullOrEmpty(DeserializeConfig.DiscordToken))
            {
                Logger.Error("Discord Token is null! oops");
                return;
            }

            DiscordSocketConfig config = new DiscordSocketConfig
            {
                UseInteractionSnowflakeDate = true // lets say discord sucks right... discord loves snow right right this just fixes the blank responses
            };

            Client = new(config);
            CommandService = new CommandService();

            Client.Ready += async () =>
            {
                Logger.Log("Discord Bot is connected", "Discord");
                guild = DiscordBot.Client.GetGuild(DeserializeConfig.ServerID);
                await RegisterCommands.Connect(DeserializeConfig, guild);
            };

            Client.SlashCommandExecuted += async (command) => await SlashCommand.Handler(DeserializeConfig, command, guild);

            await Client.LoginAsync(TokenType.Bot, DeserializeConfig.DiscordToken);
            await Client.StartAsync();
            if(DeserializeConfig.bShowBotMessage)
            {
                await Client.SetActivityAsync(new Game(DeserializeConfig.DiscordBotMessage, ActivityType.Playing));
            }
        }
    }
}
