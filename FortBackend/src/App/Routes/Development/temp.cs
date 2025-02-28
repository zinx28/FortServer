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
using Newtonsoft.Json.Linq;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.EpicResponses.Profile.Purchases;
using Microsoft.IdentityModel.Tokens;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.App.Routes.Development
{
    // goal is to use the same for each ones?!?!
    public class GameServerRequest
    {
        public string DisplayName { get; set; } = string.Empty;
    }
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

        //[HttpGet("/CalXP/{accountId}")]
        //public async Task<IActionResult> AllXP(string accountId)
        //{
        //    ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
        //    if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
        //    {
        //        if (profileCacheEntry.AccountData.commoncore.Seasons.Any(x => x.SeasonNumber == 8))
        //        {
        //            SeasonClass FoundSeason = profileCacheEntry.AccountData.commoncore?.Seasons.FirstOrDefault(x => x.SeasonNumber == 8)!;
        //            Console.WriteLine(FoundSeason.SeasonNumber);

        //            Console.WriteLine(FoundSeason.SeasonXP);
        //            Console.WriteLine(FoundSeason.Level);
        //            List<SeasonXP> SeasonXpIg = BattlepassManager.SeasonBattlePassXPItems.FirstOrDefault(e => e.Key == FoundSeason.SeasonNumber).Value;

        //            // List<SeasonXP> SeasonXpIg = BattlepassManager.SeasonBattlePassXPItems.FirstOrDefault(e => e.Key == FoundSeason.SeasonNumber).Value;
        //            //int BeforeLevelXP = SeasonXpIg.FirstOrDefault(e => e.Level == (FoundSeason.BookLevel)).XpTotal;

        //            var beforeLevelXPElement = SeasonXpIg.FirstOrDefault(e => e.Level == FoundSeason.Level);
        //            //
        //            int CurrentLevelXP;
        //            if (beforeLevelXPElement != null && SeasonXpIg.IndexOf(beforeLevelXPElement) == SeasonXpIg.Count - 1)
        //            {
        //                FoundSeason.SeasonXP = 0;
        //            }

        //            CurrentLevelXP = SeasonXpIg.FirstOrDefault(e => e.XpTotal >= (beforeLevelXPElement.XpTotal + FoundSeason.SeasonXP)).XpTotal + FoundSeason.SeasonXP;

        //            //int BeforeLevelXP = SeasonXpIg.FirstOrDefault(e => e.Level == (FoundSeason.Level)).XpTotal;
        //            //int CurrentLevelXP = SeasonXpIg.FirstOrDefault(e => e.XpToNextLevel >= (BeforeLevelXP + FoundSeason.SeasonXP)).XpTotal + FoundSeason.SeasonXP;


        //            Console.WriteLine(beforeLevelXPElement.XpTotal);


        //            Console.WriteLine(CurrentLevelXP);

        //            Console.WriteLine("CURRENT LEVEL~ cal how much to level up");

        //            Console.WriteLine(FoundSeason.Level);
        //            Console.WriteLine(FoundSeason.BookXP);
        //            bool uh = false;
        //            (FoundSeason, uh) = await LevelUpdater.Init(FoundSeason.SeasonNumber, FoundSeason, uh);

        //            Console.WriteLine(uh);
        //            Console.WriteLine(FoundSeason.Level);

        //            Console.WriteLine(FoundSeason.BookLevel);
        //            Console.WriteLine(FoundSeason.BookXP);

        //            List<string> SpecialItems = new List<string>
        //            {
        //                "",
        //                "cid_random",
        //                "glider_random",
        //                "pickaxe_random",
        //            };

        //            var ResponseIGIDFK = "s:pickaxe_random";
        //            Console.WriteLine(!ResponseIGIDFK.Contains(":"));
        //            Console.WriteLine(ResponseIGIDFK != "");
        //            Console.WriteLine(
        //                ResponseIGIDFK != "" &&
        //                !(ResponseIGIDFK.Contains(":") &&
        //                (SpecialItems.Contains(ResponseIGIDFK) ||
        //                    SpecialItems.Contains(ResponseIGIDFK.Split(":")[1]) ||
        //                    ResponseIGIDFK == ":"))
        //            );


        //        }

        //    }
        //    return Ok(new { });
        //}

        //[HttpGet("/bp/free")]
        //public async Task<IActionResult> TestA()
        //{
        //    try
        //    {
        //        List<Battlepass> FreeTier = BattlepassManager.FreeBattlePassItems.FirstOrDefault(e => e.Key == 2).Value;
        //        string Data = JsonConvert.SerializeObject(FreeTier);
        //        return Content(Data, "application/json");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex.Message); // 
        //    }

        //    return BadRequest(new { message = "error" });
        //}

        //[HttpGet("/bp/paid")]
        //public async Task<IActionResult> TestB()
        //{
        //    try
        //    {
        //        List<Battlepass> FreeTier = BattlepassManager.PaidBattlePassItems.FirstOrDefault(e => e.Key == 2).Value;
        //        string Data = JsonConvert.SerializeObject(FreeTier);
        //        return Content(Data, "application/json");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex.Message); // 
        //    }

        //    return BadRequest(new { message = "error" });
        //}

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

        /*
         * Add Vbucks
         * POST Request
         * "/FortBackend/v1/vbucks/:theamountofvbucks
         * with Authorization JWTToken that's from config
         * BODY { "DisplayName": "AccountName" }
         */ // 

        [HttpPost("/FortBackend/v1/vbucks/{amount}")]
        public async Task<IActionResult> GiveCertainVbucks(string amount)
        {
            try
            {
                Response.ContentType = "application/json";

                int Amount = int.Parse(amount ?? "0");

                if (Amount > 0)
                {
                    if (string.IsNullOrEmpty(Saved.DeserializeConfig.JWTKEY))
                    {
                        Logger.Error("JWTKEY is empty!", "GiveCertainVbucks");
                        return BadRequest(new
                        {
                            Message = "SERVER ERROR, Check Logs",
                            Status = 400
                        });
                    }

                    if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
                    {
                        Logger.Error("Authorization Header is empty!", "GiveCertainVbucks");
                        return BadRequest(new
                        {
                            Message = "SERVER ERROR, Check Logs",
                            Status = 400
                        });
                    }

                    if(authHeader != Saved.DeserializeConfig.JWTKEY)
                    {
                        Logger.Error("Wrong Authorization", "GiveCertainVbucks");
                        return BadRequest(new
                        {
                            Message = "SERVER ERROR, Check Logs",
                            Status = 400
                        });
                    }
                 

                    using (var reader = new StreamReader(Request.Body))
                    {
                        var body = await reader.ReadToEndAsync();
                        var requestData = JsonConvert.DeserializeObject<GameServerRequest>(body);

                        string DisplayName = requestData?.DisplayName!;
                        if(string.IsNullOrEmpty(DisplayName))
                        {
                            return BadRequest(new
                            {
                                Message = "make sure body is correct! ~ displayName HAS to be 'DisplayName'",
                                Status = 400
                            });
                        }

                        // really using discord for this
                        ProfileCacheEntry profileCacheEntry = await GrabData.ProfileDiscord(DisplayName, "Username");

                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                        {
                            // Goal, give vbucks BUT if they havent claimed it yet instead of adding a new giftbox just update the currency
                            Logger.Warn("ds");
                            //profileCacheEntry.AccountData.commoncore.Gifts.FirstOrDefault()
                            if (profileCacheEntry?.AccountData?.commoncore?.Gifts is Dictionary<string, GiftCommonCoreItem> gifts)
                            {
                                var purchasedGift = gifts.FirstOrDefault(gift =>
                                    gift.Value?.attributes?.lootList is List<NotificationsItemsClassOG> lootList &&
                                    lootList.Count > 0 &&
                                    lootList[0]?.itemGuid?.ToString().Contains("Currency:MtxPurchased") == true);

                               
                                int GrabPlacement3 = profileCacheEntry.AccountData.commoncore.Items.Select((pair, index) => (pair.Key, pair.Value, index))
                                             .TakeWhile(pair => !pair.Key.Equals("Currency")).Count();

                                if (GrabPlacement3 != -1)
                                {
                                    var Value = profileCacheEntry.AccountData.commoncore.Items["Currency"];

                                    if (Value.templateId != null || Value != null)
                                    {
                                        if (Value.templateId == "Currency:MtxPurchased")
                                        {
                                            Value.quantity += Amount;
                                        }
                                    }
                                }
                                Logger.Warn("SIGMA");
                                Logger.Error(purchasedGift.Key);
                                if (string.IsNullOrEmpty(purchasedGift.Key))
                                {
                                    // user has no gicffts
                                    var RandomOfferId = Guid.NewGuid().ToString();
                                    int Season = -1;
                                    if (Saved.DeserializeGameConfig.ForceSeason)
                                    {
                                        Season = Saved.DeserializeGameConfig.Season;
                                    }

                                    profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                                    {
                                        templateId = Season > 10 ? "GiftBox:GB_RMTOffer" : "GiftBox:GB_Default",
                                        attributes = new GiftCommonCoreItemAttributes
                                        {
                                            lootList = new List<NotificationsItemsClassOG>()
                                            {
                                                new NotificationsItemsClassOG
                                                {
                                                    itemGuid = "Currency:MtxPurchased",
                                                    itemType = "Currency:MtxPurchased",
                                                    quantity =  Amount
                                                }
                                            }
                                        },
                                        quantity = 1
                                    });
                                    profileCacheEntry.AccountData.commoncore.RVN += 1;
                                    profileCacheEntry.AccountData.commoncore.CommandRevision += 1;

                                    Clients Client = GlobalData.Clients.FirstOrDefault(client => client.accountId == profileCacheEntry.AccountId)!;

                                    if (Client != null)
                                    {
                                        string xmlMessage;
                                        byte[] buffer;
                                        WebSocket webSocket = Client.Game_Client;

                                        if (webSocket != null && webSocket.State == WebSocketState.Open)
                                        {
                                            XNamespace clientNs = "jabber:client";

                                            var message = new XElement(clientNs + "message",
                                                new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                new XAttribute("to", profileCacheEntry.AccountId),
                                                new XElement(clientNs + "body", JsonConvert.SerializeObject(new
                                                {
                                                    payload = new { },
                                                    type = "com.epicgames.gift.received",
                                                    timestamp = DateTime.UtcNow.ToString("o")
                                                }))
                                            );

                                            xmlMessage = message.ToString();
                                            buffer = Encoding.UTF8.GetBytes(xmlMessage);

                                            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                        }

                                    }
                                }
                                else
                                {
                                    // user has vbucks gift already!
                                    purchasedGift.Value.attributes.lootList[0].quantity += Amount;
                                }

                                return Ok(new
                                {
                                    Message = $"Granted vbucks to {profileCacheEntry.UserData.Username}",
                                    Status = 200
                                });
                            }
                        }
                    }
                }
                else
                {
                    return BadRequest(new
                    {
                        Message = "You can't give 0 vbucks",
                        Status = 400
                    });
                }
               
               
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return BadRequest(new
            {
                Message = "Check backend logs <3",
                Status = 400
            });
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

        [HttpGet("/js/{file}")]
        public IActionResult FileJSResponse(string file)
        {
            try
            {
                Response.ContentType = "text/javascript";

                if (!Regex.IsMatch(file, @"^[a-zA-Z0-9\-._]+$"))
                {
                    return BadRequest("Invalid image parameter");
                }

                string filePath = Path.Combine(PathConstants.BaseDir, "JS", file);

                //Console.WriteLine(filePath);
                if (System.IO.File.Exists(filePath))
                {
                    string cssContent = System.IO.File.ReadAllText(filePath);


                    return Content(cssContent);
                    // await context.Response.WriteAsync(cssContent);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "JS");
            }

            return NoContent();
        }
    }
}
