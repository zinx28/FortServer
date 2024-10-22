using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using FortLibrary.EpicResponses.FortniteServices.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using FortBackend.src.App.Utilities.Constants;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    public class CreativeController : ControllerBase
    {
        // this file will most likely get support in the future for chapter 3 if i ever do that chapter (even if support without bp)
        [HttpGet("links/api/fn/mnemonic/{code}")]
        public IActionResult Mnemonic(string code) // uses auth normally
        {
            return Ok(new
            {
                @namespace = "fn",
                accountId = "e0121ce665474edfab586ce98ab791ed",
                creatorName = "FortBackend",
                mnemonic = code,
                linkType = "Creative:Island",
                metadata = new
                {
                    tagline = "Endless 1v1 build fights with any guns!",
                    islandType = "CreativePlot:temperate_medium",
                    dynamicXp = new
                    {
                        uniqueGameVersion = "1",
                        //calibrationPhase = null
                    },
                    title = "CLOUDS 1v1 BUILD FIGHTS",
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
    }
}
