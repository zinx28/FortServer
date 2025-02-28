using Discord.WebSocket;
using FortBackend.src.App.Utilities.Discord.Helpers.command;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.ConfigHelpers;
using FortLibrary;
using Discord;

namespace FortBackend.src.App.Utilities.Discord.Helpers
{
    public class SlashCommand
    { 
        public static bool CheckIfRole(FortConfig config, SocketSlashCommand command, SocketGuild guild)
        {
            if (config.RoleID != 0)
            {
                if (guild != null)
                {
                    if (guild.Roles.Any(r => r.Id == config.RoleID))
                    {
                        SocketGuildUser user = guild.GetUser(command.User.Id);

                        if (user.Roles.Any(r => r.Id == config.RoleID))
                        {
                            return true;
                        }
                        else
                        {
                            command.RespondAsync("You do not have the permission to use this command!");
                            return false;
                        }
                    }
                    else
                    {
                        Logger.Error("Role was not found in this server!");
                    }
                }
                else
                {
                    Logger.Error("Bot is not in the server!");
                }
            }
            else
            {
                Logger.Error("Please set put a role id in the config!");
            }

            command.RespondAsync("Please the check backend logs");
            return false;
        }
        // ill implement this to others in the future
        private static Dictionary<ulong, CancellationTokenSource> _userInteractionCache = new();
        public static async Task Handler(FortConfig config, SocketSlashCommand command, SocketGuild guild)
        {
            try
            {
                // no BOTS gr
                if (command.User.IsBot)
                {
                    return;
                }

                switch (command.CommandName)
                {
                    case "test":
                        await Test.Respond(command);
                        break;
                    case "who":
                        if (CheckIfRole(config, command, guild))
                        {
                            if (_userInteractionCache.TryGetValue(command.User.Id, out var oldTask))
                            {
                                oldTask.Cancel();
                                oldTask.Dispose();
                                _userInteractionCache.Remove(command.User.Id);
                            }

                            var newTask = new CancellationTokenSource();
                            _userInteractionCache[command.User.Id] = newTask;

                            await Who.Respond(command, newTask.Token);
                        }
                        break;
                    case "details":
                        await Details.Respond(command);
                        break;
                    case "register":
                        await Create.Respond(command);
                        break;
                    case "change_password":
                        await PasswordUpdate.Respond(command);
                    break;
                    //case "events":
                    //    if (CheckIfRole(config, command, guild))
                    //    {
                    //        await TourEvents.Respond(command);
                    //    }
                    //    break;
                    default:
                        var embed = new EmbedBuilder()
                        .WithTitle("Failed to find command")
                        .WithDescription("?")
                        .WithColor(Color.Blue)
                        .WithCurrentTimestamp();

                        await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling command: {ex.Message}");
              //  await command.RespondAsync("testr", ephemeral: true);
            }
        }
    }
}
