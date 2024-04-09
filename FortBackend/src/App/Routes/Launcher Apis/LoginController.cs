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
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Attributes;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;

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

                Console.WriteLine(authToken);

                if (authToken != null)
                {
                    ProfileCacheEntry profileCacheEntry = await GrabData.Profile("", true, authToken);
        
                    if (!string.IsNullOrEmpty(profileCacheEntry.AccountId))
                    {
                        var Character = "CID_001_Athena_Commando_F_Default";
                        if (profileCacheEntry.UserData.banned)
                        {
                            return Unauthorized(); // Banned
                        }
                        SandboxLoadout sandboxLoadout = profileCacheEntry.AccountData.athena.loadouts_data.FirstOrDefault(e => e.Key.Contains("sandbox_loadout"))!.Value;
                        if (sandboxLoadout != null)
                        {
                            var CharacterData = sandboxLoadout.attributes.locker_slots_data.slots.character.items[0];
                            if(!string.IsNullOrEmpty(CharacterData) && !CharacterData.Contains("cid_random"))
                            {
                                Character = CharacterData.Split(":")[1;
                            }
                        }
                       
                        return Ok(new
                        {
                            username = profileCacheEntry.UserData.Username,
                            email = profileCacheEntry.UserData.Email,
                            character = Character,
                            DiscordId = profileCacheEntry.UserData.DiscordId,
                        });                
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
