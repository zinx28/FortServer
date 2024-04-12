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
                UseInteractionSnowflakeDate = true, // lets say discord sucks right... discord loves snow right right this just fixes the blank responses
                GatewayIntents = GatewayIntents.All
            };

            Client = new(config);
            CommandService = new CommandService();

            Client.Ready += OnReady;
            DiscordBot.Client.Connected += OnReconnected;
            DiscordBot.Client.Disconnected += OnDisconnected;

            Client.SlashCommandExecuted += async (command) => await SlashCommand.Handler(DeserializeConfig, command, guild);

            await Client.LoginAsync(TokenType.Bot, DeserializeConfig.DiscordToken);
            await Client.StartAsync();
            if(DeserializeConfig.bShowBotMessage)
            {
                await Client.SetActivityAsync(new Game(DeserializeConfig.DiscordBotMessage, ActivityType.Playing));
            }
        }

        private static async Task OnReady()
        {
            Logger.Log("Discord Bot is connected", "Discord");
            guild = DiscordBot.Client.GetGuild(Saved.Saved.DeserializeConfig.ServerID);
            await RegisterCommands.Connect(Saved.Saved.DeserializeConfig, guild);
        }

        private static Task OnReconnected()
        {
            DiscordBot.Client.Ready += OnReady;
           /// DiscordBot.Client.InteractionCreated += OnInteractionCreated;
            return Task.CompletedTask;
        }

        private static async Task OnInteractionCreated(SocketInteraction interaction)
        {
            await RegisterCommands.Connect(Saved.Saved.DeserializeConfig, guild);
        }

        private static Task OnDisconnected(Exception exception)
        {
            DiscordBot.Client.Ready -= OnReady;
           // DiscordBot.Client.InteractionCreated -= OnInteractionCreated;
            return Task.CompletedTask;
        }
    }
}
