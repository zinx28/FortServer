using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary;
using FortLibrary.EpicResponses.FortniteServices.Content;
using FortLibrary.EpicResponses.FortniteServices.Events;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text.RegularExpressions;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    public class CreativeController : ControllerBase
    {
        // this file will most likely get support in the future for chapter 3 if i ever do that chapter (even if support without bp)
        [HttpPost("/links/api/fn/mnemonic")]
        public IActionResult MnemonicWhy() // uses auth normally
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var AcceptLanguage = Request.Headers["Accept-Language"].ToString();

            if (string.IsNullOrEmpty(AcceptLanguage))
            {
                AcceptLanguage = "en"; // weird
            }

            List<MnemonicC> ContentJsonResponse = new();
            if (NewsManager.mnemonicCs.TryGetValue(AcceptLanguage, out List<MnemonicC> Test))
            {
                ContentJsonResponse = Test;
            }

            return Ok(ContentJsonResponse);
            //{
            //     new
            //        {
            //         @namespace = "fn",
            //        mnemonic = "playlist_defaultsolo",
            //        linkType = "BR:Playlist",
            //        active = true,
            //        disabled = false,
            //        version = 95,
            //        moderationStatus = "Unmoderated",
            //        accountId = "epic",
            //        creatorName = "Epic",
            //        descriptionTags = new string[] { },
            //        discoveryIntent = "PUBLIC",
            //        metadata = new
            //        {
            //            image_url = "https://cdn2.unrealengine.com/solo-1920x1080-1920x1080-bc0a5455ce20.jpg",
            //            locale = "en",
            //            title = "Solo",
            //            matchmaking = new
            //            {
            //                override_playlist = "playlist_defaultsolo"
            //            },
            //            tagline = "Go it alone in a battle to be the last one standing.",
            //            introduction = "Go it alone in a battle to be the last one standing."
            //        }
            //    },
            //    //new
            //    // {
            //    //    @namespace = "fn",
            //    //    accountId = "e0121ce665474edfab586ce98ab791ed",
            //    //    creatorName = "FortBackend",
            //    //    mnemonic = "",
            //    //    linkType = "Creative:Island",
            //    //    metadata = new
            //    //    {
            //    //        tagline = "Endless 1v1 build fights with any guns!",
            //    //        islandType = "CreativePlot:temperate_medium",
            //    //        dynamicXp = new
            //    //        {
            //    //            uniqueGameVersion = "1",
            //    //            //calibrationPhase = null
            //    //        },
            //    //        title = "Femboy 1v1 :3",
            //    //        locale = "en",
            //    //        matchmaking = new
            //    //        {
            //    //            joinInProgressType = "JoinImmediately",
            //    //            playersPerTeam = -1,
            //    //            maximumNumberOfPlayers = 69,
            //    //            override_Playlist = "",
            //    //            playerCount = 69,
            //    //            mmsType = "keep_full",
            //    //            numberOfTeams = 69,
            //    //            bAllowJoinInProgress = true,
            //    //            minimumNumberOfPlayers = 69,
            //    //            joinInProgressTeam = 255
            //    //        },
            //    //        supportCode = "FortBackend",
            //    //        introduction = "CLOUDS 1v1 build fights with any guns!\n\n",
            //    //        generated_image_urls = new
            //    //        {
            //    //            url_s = "https://fortnite-island-screenshots-live-cdn.ol.epicgames.com/screenshots/5224-2376-0131_s.png",
            //    //            url_m = "https://fortnite-island-screenshots-live-cdn.ol.epicgames.com/screenshots/5224-2376-0131_s.png",
            //    //            url = "https://fortnite-island-screenshots-live-cdn.ol.epicgames.com/screenshots/5224-2376-0131_s.png"
            //    //        }
            //    //    },
            //    //    version = 69,
            //    //    active = true,
            //    //    disabled = false,
            //    //    created = "2019-03-29T20:05:34.195Z",
            //    //    descriptionTags = new[] { "1v1", "free for all", "pvp" },
            //    //    moderationStatus = "Approved"
            //    //}
            //});

        }

        [HttpGet("links/api/fn/mnemonic/{code}")]
        public IActionResult Mnemonic(string code) // uses auth normally
        {
            return Ok(new
            {
                @namespace = "fn",
                accountId = "e0121ce665474edfab586ce98ab791ed",
                creatorName = "FortBackend",
                mnemonic = code,
                linkType = code.Contains("playlist_") ? "BR:Playlist" : "Creative:Island",
                metadata = new
                {
                    tagline = "Endless 1v1 build fights with any guns!",
                    islandType = "CreativePlot:temperate_medium",
                    dynamicXp = new
                    {
                        uniqueGameVersion = "1",
                        //calibrationPhase = null
                    },
                    title = "Femboy 1v1 :3",
                    locale = "en",
                    matchmaking = new
                    {
                        joinInProgressType = "JoinImmediately",
                        playersPerTeam = -1,
                        maximumNumberOfPlayers = 69,
                        override_Playlist = "",
                        playerCount = 69,
                        mmsType = "keep_full",
                        numberOfTeams = 69,
                        bAllowJoinInProgress = true,
                        minimumNumberOfPlayers = 69,
                        joinInProgressTeam = 255
                    },
                    supportCode = "FortBackend",
                    introduction = "CLOUDS 1v1 build fights with any guns!\n\n",
                    generated_image_urls = new
                    {
                        url_s = "https://fortnite-island-screenshots-live-cdn.ol.epicgames.com/screenshots/5224-2376-0131_s.png",
                        url_m = "https://fortnite-island-screenshots-live-cdn.ol.epicgames.com/screenshots/5224-2376-0131_s.png",
                        url = "https://fortnite-island-screenshots-live-cdn.ol.epicgames.com/screenshots/5224-2376-0131_s.png"
                    }
                },
                version = 69,
                active = true,
                disabled = false,
                created = "2019-03-29T20:05:34.195Z",
                descriptionTags = new[] { "1v1", "free for all", "pvp" },
                moderationStatus = "Approved"
            });

        }


        [HttpPost("/api/v1/discovery/surface/{accountId}")]
        public ActionResult DiscoverySurface(string accountId)
        {
            try
            {
                var jsonResponse = JsonConvert.SerializeObject(NewsManager.CreativeDiscoveryResponse, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                return Content(jsonResponse, "application/json");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "MOTDTARGET");
            }
            return Ok(new { });
        }
    }
}
