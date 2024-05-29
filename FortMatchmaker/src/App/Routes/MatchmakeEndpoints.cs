using FortLibrary;
using FortMatchmaker.src.App.Utilities;
using FortMatchmaker.src.App.Utilities.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace FortMatchmaker.src.App.Routes
{
 
    [ApiController]
    [Route("fortmatchmaker/removeUser")]
    public class MatchmakeEndpoints : ControllerBase
    {

        [HttpPost("{accountId}")]
        public IActionResult RemoveUser(string accountId)
        {
            try
            {
                var tokenArray = Request.Headers["Authorization"].ToString().Split("bearer ");
                var token = tokenArray.Length > 1 ? tokenArray[1] : "";

                if (Saved.DeserializeConfig.JWTKEY == token)
                {
                    var haha = MatchmakerData.SavedData.FirstOrDefault(e => e.Value.AccountId == accountId);
                    if (haha.Value != null && haha.Key.State != WebSocketState.Closed)
                    {
                        haha.Key.CloseAsync(WebSocketCloseStatus.NormalClosure, "bc why not", CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "MatchmakerEndpoints");
            }

            return Ok("gagaga");
        }

    }
    
}
