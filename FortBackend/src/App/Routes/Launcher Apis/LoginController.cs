using FortLibrary.Encoders;
using FortBackend.src.App.Utilities.Helpers.UserManagement;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FortBackend.src.App.Utilities.Saved;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary;

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

                //Console.WriteLine(authToken);

                if (authToken != null)
                {
                    ProfileCacheEntry profileCacheEntry = await GrabData.Profile("", true, authToken);
        
                    if (!string.IsNullOrEmpty(profileCacheEntry.AccountId))
                    {
                        var Character = "CID_001_Athena_Commando_F_Default";
                        if (profileCacheEntry.UserData.banned)
                        {
                            return Ok(new
                            {
                                banned = true,
                                username = profileCacheEntry.UserData.Username,
                                email = profileCacheEntry.UserData.Email,
                                character = Character,
                                DiscordId = profileCacheEntry.UserData.DiscordId,
                            });
                            return Unauthorized(); // Banned
                        }
                        SandboxLoadout sandboxLoadout = profileCacheEntry.AccountData.athena.loadouts_data.FirstOrDefault(e => e.Key.Contains("sandbox_loadout"))!.Value;
                        if (sandboxLoadout != null)
                        {
                            var CharacterData = sandboxLoadout.attributes.locker_slots_data.slots.character.items[0];
                            if(!string.IsNullOrEmpty(CharacterData) && !CharacterData.Contains("cid_random"))
                            {
                                Character = CharacterData.Split(":")[1];
                            }
                        }

                        var Wins = 0;
                        var MatchesPlayed = 0;
                        var Kills = 0;
                        var Vbucks = 0;

                        // not the best way but only way
                        if (profileCacheEntry.StatsData.stats.Count > 0)
                        {
                            // Solos
                            if(profileCacheEntry.StatsData.stats.TryGetValue("br_placetop1_pc_m0_p2", out int SoloWin)) { Wins += SoloWin; }
                            if (profileCacheEntry.StatsData.stats.TryGetValue("br_matchesplayed_pc_m0_p2", out int SoloMatchedPlayed)) { MatchesPlayed += SoloMatchedPlayed; }
                            if (profileCacheEntry.StatsData.stats.TryGetValue("br_kills_pc_m0_p2", out int SolosKills)) { Kills += SolosKills; }
                            // Duos
                            if (profileCacheEntry.StatsData.stats.TryGetValue("br_placetop1_pc_m0_p10", out int DuoWin)) { Wins += DuoWin; }
                            if (profileCacheEntry.StatsData.stats.TryGetValue("br_matchesplayed_pc_m0_p10", out int DuosMatchedPlayed)) { MatchesPlayed += DuosMatchedPlayed; }
                            if (profileCacheEntry.StatsData.stats.TryGetValue("br_kills_pc_m0_p10", out int DuoKills)) { Kills += DuoKills; }
                            // Squads
                            if (profileCacheEntry.StatsData.stats.TryGetValue("br_placetop1_pc_m0_p9", out int SquadWin)) { Wins += SquadWin; }
                            if (profileCacheEntry.StatsData.stats.TryGetValue("br_matchesplayed_pc_m0_p9", out int SquadMatchedPlayed)) { MatchesPlayed += SquadMatchedPlayed; }
                            if (profileCacheEntry.StatsData.stats.TryGetValue("br_kills_pc_m0_p9", out int SquadKills)) { Kills += SquadKills; }
                        }

                        int GrabPlacement3 = profileCacheEntry.AccountData.commoncore.Items.Select((pair, index) => (pair.Key, pair.Value, index))
                          .TakeWhile(pair => !pair.Key.Equals("Currency")).Count();

                        if (GrabPlacement3 != -1)
                        {
                            Vbucks = profileCacheEntry.AccountData.commoncore.Items["Currency"].quantity;
                        }

                        return Ok(new
                        {
                            banned = profileCacheEntry.UserData.banned,
                            username = profileCacheEntry.UserData.Username,
                            email = profileCacheEntry.UserData.Email,
                            character = Character,
                            vbucks = Vbucks,
                            stats = new
                            {
                                Wins,
                                MatchesPlayed,
                                Kills
                            },
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
