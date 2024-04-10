using Discord.WebSocket;
using FortBackend.src.App.Utilities.Discord.Helpers.command;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;

namespace FortBackend.src.App.Utilities.Discord.Helpers
{
    public class SlashCommand
    { 
        public static bool CheckIfRole(Config config, SocketSlashCommand command, SocketGuild guild)
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
                }else
                {
                    Logger.Error("Bot is not in the server!");
                }
            }else
            {
                Logger.Error("Please set put a role id in the config!");
            }

            command.RespondAsync("Please the check backend logs");
            return false;
        }
        public static async Task Handler(Config config, SocketSlashCommand command, SocketGuild guild)
        {
            // no BOTS gr
            if(command.User.IsBot)
            {
                return;
            }

            if (command.CommandName == "test")
            {
                await Test.Respond(command);
            }

            if(command.CommandName == "who")
            {
                if(CheckIfRole(config, command, guild))
                {
                    await Who.Respond(command);
                }
            }
        }
    }
}
