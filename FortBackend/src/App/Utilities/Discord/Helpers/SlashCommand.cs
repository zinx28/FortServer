using Discord.WebSocket;
using FortBackend.src.App.Utilities.Discord.Helpers.command;

namespace FortBackend.src.App.Utilities.Discord.Helpers
{
    public class SlashCommand
    { 
        public static async Task Handler(SocketSlashCommand command)
        {
            if(command.CommandName == "test")
            {
                await Test.Respond(command);
            }
        }
    }
}
