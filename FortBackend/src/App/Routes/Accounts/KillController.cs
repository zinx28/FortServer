using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary;
using FortLibrary.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace FortBackend.src.App.Routes.APIS.Accounts
{
    [ApiController]
    [Route("account/api")]
    public class KillController : ControllerBase
    {
        //https://account-public-service-prod.ol.epicgames.com/account/api/oauth/sessions/kill?killType=OTHERS_ACCOUNT_CLIENT_SERVICE
        [HttpDelete("oauth/sessions/kill")]
        public IActionResult KillSessions([FromQuery] string killType)
        {
            switch (killType)
            {
                case "ALL":

                    return NoContent();

                case "OTHERS":
                    return NoContent();

                default:
                    return NoContent();
            }
        }
        // all work on this soon
        [HttpDelete("oauth/sessions/kill/{accesstoken}")]
        public IActionResult KillAccessSessions(string accesstoken)
        {
            try
            {
                //var token = Request.Headers["Authorization"].ToString().Split("bearer ")[1];

                //var accessToken = token.Replace("eg1~", "");

                //var handler = new JwtSecurityTokenHandler();
                //var decodedToken = handler.ReadJwtToken(accessToken);
                //bool FoundAccount = false;
                //if (GlobalData.AccessToken.Any(e => e.token == token))
                //    FoundAccount = true;
                //else if (GlobalData.ClientToken.Any(e => e.token == token))
                //    FoundAccount = true;
                //else if (GlobalData.RefreshToken.Any(e => e.token == token))
                //    FoundAccount = true;

                //if (FoundAccount)
                //{
                //    var AccessTokenIndex = GlobalData.AccessToken.FindIndex(i => i.token == accesstoken);

                //    if (AccessTokenIndex != -1)
                //    {
                //        var AccessToken = GlobalData.AccessToken[AccessTokenIndex];
                //        GlobalData.AccessToken.RemoveAt(AccessTokenIndex);

                //        var XmppClient = GlobalData.Clients.Find(i => i.token == AccessToken.token);
                //        if (XmppClient != null)
                //        {
                //            XmppClient.Client.Dispose();
                //        }

                //        var RefreshTokenIndex = GlobalData.RefreshToken.FindIndex(i => i.accountId == AccessToken.accountId);
                //        if (RefreshTokenIndex != -1)
                //        {
                //            GlobalData.RefreshToken.RemoveAt(RefreshTokenIndex);
                //        }
                //    }

                //    if(GlobalData.ClientToken.Any(i => i.token == accesstoken))
                //    {
                //        var ClientTokenIndex = GlobalData.ClientToken.FindIndex(i => i.token == accesstoken);
                //        if (ClientTokenIndex != -1)
                //        {
                //            GlobalData.ClientToken.RemoveAt(ClientTokenIndex);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("OAUTH KILL " + ex.Message);
            }
            return NoContent();
        }

    }
}
