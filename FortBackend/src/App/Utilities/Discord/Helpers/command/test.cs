using Discord.WebSocket;

namespace FortBackend.src.App.Utilities.Discord.Helpers.command
{
    public class Test
    {
        public static async Task Respond(SocketSlashCommand command)
        {
            await command.RespondAsync("test!!");
        }
    }
}
