using Microsoft.AspNetCore.Mvc;
using FortBackend.src.App.Utilities;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortLibrary.Dynamics;
using FortBackend.src.App.Utilities.Constants;
using FortLibrary.MongoDB.Module;

namespace FortBackend.src.App.Routes.Development
{
    [ApiController]
    [Route("temp")]
    public class TempController : ControllerBase
    {
        private IMongoDatabase _database;
        public TempController(IMongoDatabase database)
        {
            _database = database;
        }

        //[HttpGet("/PRIVATE/DEVELOPER/DATA/{accountId}")]
        //public async Task<IActionResult> GrabDATA(string accountId)
        //{
        //    try
        //    {
        //        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
        //        string Data = JsonConvert.SerializeObject(profileCacheEntry);
        //        if(!string.IsNullOrEmpty(Data)) { return Ok(Data); }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex.Message); // 
        //    }

        //    return BadRequest(new { message = "error" });
        //}

        //[HttpGet("/")]
        //public async Task<IActionResult> asa()
        //{
        //    Request.ContentType = "application/json";
        //    var responseObj = new
        //    {
        //        status = "OK"
        //    };

        //    return Ok(responseObj);
        //}

        // stuff that are useless

        /* int BeforeLevelXP = SeasonXpIg.FirstOrDefault(e => e.Level == (FoundSeason.Level - 1)).XpTotal;
            int CurrentLevelXP = SeasonXpIg.FirstOrDefault(e => e.XpTotal >= (BeforeLevelXP + FoundSeason.SeasonXP)).XpTotal;*/

        [HttpGet("/CalXP/{accountId}")]
        public async Task<IActionResult> AllXP(string accountId)
        {
            ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
            if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
            {
                if (profileCacheEntry.AccountData.commoncore.Seasons.Any(x => x.SeasonNumber == 8))
                {
                    SeasonClass FoundSeason = profileCacheEntry.AccountData.commoncore?.Seasons.FirstOrDefault(x => x.SeasonNumber == 8)!;
                    Console.WriteLine(FoundSeason.SeasonNumber);

                    Console.WriteLine(FoundSeason.SeasonXP);
                    Console.WriteLine(FoundSeason.Level);
                    List<SeasonXP> SeasonXpIg = BattlepassManager.SeasonBattlePassXPItems.FirstOrDefault(e => e.Key == FoundSeason.SeasonNumber).Value;

                    int BeforeLevelXP = SeasonXpIg.FirstOrDefault(e => e.Level == (FoundSeason.Level)).XpTotal;
                    int CurrentLevelXP = SeasonXpIg.FirstOrDefault(e => e.XpToNextLevel >= (BeforeLevelXP + FoundSeason.SeasonXP)).XpTotal + FoundSeason.SeasonXP;


                    Console.WriteLine(BeforeLevelXP);


                    Console.WriteLine(CurrentLevelXP);

                }

            }
            return Ok(new { });
        }

        [HttpGet("/bp/free")]
        public async Task<IActionResult> TestA()
        {
            try
            {
                List<Battlepass> FreeTier = BattlepassManager.FreeBattlePassItems.FirstOrDefault(e => e.Key == 2).Value;
                string Data = JsonConvert.SerializeObject(FreeTier);
                return Content(Data, "application/json");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message); // 
            }

            return BadRequest(new { message = "error" });
        }

        [HttpGet("/bp/paid")]
        public async Task<IActionResult> TestB()
        {
            try
            {
                List<Battlepass> FreeTier = BattlepassManager.PaidBattlePassItems.FirstOrDefault(e => e.Key == 2).Value;
                string Data = JsonConvert.SerializeObject(FreeTier);
                return Content(Data, "application/json");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message); // 
            }

            return BadRequest(new { message = "error" });
        }

        [HttpGet("/PRIVATE/DEVELOPER/DATA/TOKEN/{token}")]
        public async Task<IActionResult> GrabTokenDATA(string token)
        {
            try
            {
                ProfileCacheEntry profileCacheEntry = await GrabData.Profile("", true, token);
                string Data = JsonConvert.SerializeObject(profileCacheEntry);
                if (!string.IsNullOrEmpty(Data)) { return Ok(Data); }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message); // 
            }

            return BadRequest(new { message = "error" });
        }

      

        //[HttpGet("/{a}/{s}/{idk}")]
        //public IActionResult GameRating(string a, string s, string idk)
        //{
        //    try
        //    {
        //        // Prevent Weird Characters
        //        if (!Regex.IsMatch(idk, @"^[a-zA-Z0-9\-._]+$"))
        //        {
        //            return BadRequest("Invalid image parameter");
        //        }
        //        var imagePath = PathConstants.ReturnImage("Trans_Boykisser.png");
        //        if (System.IO.File.Exists(imagePath))
        //        {
        //            return PhysicalFile(imagePath, "image/jpeg");
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    catch /*(Exception ex)*/
        //    {

        //        return StatusCode(500, "Internal Server Error");
        //    }
        //}

        [HttpGet("/image/{image}")]
        public IActionResult ImageEnd(string image)
        {
            try
            {
                // Prevent Weird Characters
                if (!Regex.IsMatch(image, @"^[a-zA-Z0-9\-._]+$"))
                {
                    return BadRequest("Invalid image parameter");
                }
                var imagePath = PathConstants.ReturnImage(image);
                if (System.IO.File.Exists(imagePath))
                {
                    return PhysicalFile(imagePath, "image/jpeg");
                }
                else
                {
                    return NotFound();
                }
            }
            catch /*(Exception ex)*/
            {

                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("/css/{file}")]
        public IActionResult FileResponse(string file)
        {
            try
            {
                Response.ContentType = "text/css";

                if (!Regex.IsMatch(file, @"^[a-zA-Z0-9\-._]+$"))
                {
                    return BadRequest("Invalid image parameter");
                }

                string filePath = Path.Combine(PathConstants.BaseDir, "CSS", file);

                if (System.IO.File.Exists(filePath))
                {
                    string cssContent = System.IO.File.ReadAllText(filePath);


                    return Content(cssContent);
                    // await context.Response.WriteAsync(cssContent);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CSS");
            }

            return NoContent();
        }
    }
}
