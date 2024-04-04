using FortBackend.src.App.Utilities.Helpers.Encoders;
using FortBackend.src.App.Utilities.Helpers.UserManagement;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using static FortBackend.src.App.Utilities.Classes.DiscordAuth;
using FortBackend.src.App.Utilities.Saved;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities;

namespace FortBackend.src.App.Routes.LUNA_CUSTOMS
{
    [ApiController]
    [Route("launcher/api/v1")]
    public class LoginController : ControllerBase
    {
        private IMongoDatabase _database;
        public LoginController(IMongoDatabase database)
        {
            _database = database;
        }


        [HttpGet("login")]
        public async Task<IActionResult> LoginApi()
        {
            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    return Unauthorized(); 
                }

                string authToken = Request.Headers["Authorization"];

                if (authToken != null)
                {
                    var FindUser = await Handlers.FindOne<User>("accesstoken", authToken);
                    if (FindUser != "Error")
                    {
                        User UserData = JsonConvert.DeserializeObject<User[]>(FindUser)?[0]!;
                        if (UserData != null)
                        {
                            if (UserData.banned)
                            {
                                return Unauthorized(); // Banned
                            }

                            return Ok(new
                            {
                                username = UserData.Username,
                                email = UserData.Email,
                                DiscordId = UserData.DiscordId,
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "Login APi!!!");
            }
            return Unauthorized();
        }

    }
}
