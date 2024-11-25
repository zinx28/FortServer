using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FortBackend.src.App.Utilities.Discord.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.ConfigHelpers;
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
            FortConfig DeserializeConfig = Saved.Saved.DeserializeConfig;

            if(string.IsNullOrEmpty(DeserializeConfig.DiscordToken))
            {
                Logger.Error("Discord Token is null! oops");
                return;
            }

            if(DeserializeConfig.ActivityType > 5)
            {
                DeserializeConfig.ActivityType = 3; // Watching
            }

            DiscordSocketConfig config = new DiscordSocketConfig
            {
                UseInteractionSnowflakeDate = true,//, // lets say discord sucks right... discord loves snow right right this just fixes the blank responses
                                                   // GatewayIntents = GatewayIntents.All
              //  GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages
            };

            Client = new(config);
            CommandService = new CommandService();

            Client.Log += DiscLog;
            Client.Ready += OnReady;
            Client.Connected += OnReconnected;
            Client.Disconnected += OnDisconnected;

            Client.SlashCommandExecuted += async (command) => await SlashCommand.Handler(DeserializeConfig, command, guild);

            await Client.LoginAsync(TokenType.Bot, DeserializeConfig.DiscordToken);
            await Client.StartAsync();
            if(DeserializeConfig.bShowBotMessage)
            {
                await Client.SetActivityAsync(new Game(DeserializeConfig.DiscordBotMessage, (ActivityType)DeserializeConfig.ActivityType));
            }
        }
        private static Task DiscLog(LogMessage DiscordBotMessage)
        {
            Logger.Log(DiscordBotMessage.Message, "DiscordBot");
            return Task.CompletedTask;
        }


        private static async Task OnReady()
        {
            Logger.Log("Discord Bot is connected", "Discord");

            //Client.InteractionCreated -= OnInteractionCreated;
            //Client.InteractionCreated += OnInteractionCreated;

            guild = Client.GetGuild(Saved.Saved.DeserializeConfig.ServerID);
            await RegisterCommands.Connect(Saved.Saved.DeserializeConfig, guild);
        }

        private static Task OnReconnected()
        {
            Logger.Log("RECONNECTING", "Discord");
            Client.Ready -= OnReady;
            Client.Ready += OnReady;
           /// DiscordBot.Client.InteractionCreated += OnInteractionCreated;
            return Task.CompletedTask;
        }

        //private static async Task OnInteractionCreated(SocketInteraction interaction)
        //{
        //    //await RegisterCommands.Connect(Saved.Saved.DeserializeConfig, guild);
        //}

        private static Task OnDisconnected(Exception exception)
        {
            Logger.Log("DISCONNECTED", "Discord");
            DiscordBot.Client.Ready -= OnReady;
           // DiscordBot.Client.InteractionCreated -= OnInteractionCreated;
            return Task.CompletedTask;
        }
    }
}
