using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Friends;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using FortLibrary;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Reactive;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.Encoders.JWTCLASS;

namespace FortBackend.src.App.Routes.Friends
{
    [ApiController]
    [Route("party")]
    public class PartyController : ControllerBase
    {
        [HttpGet("api/v1/Fortnite/parties/{partyId}")]
        public IActionResult FortnitePartyGet(string partyId)
        {
            try
            {
                Response.ContentType = "application/json";
                var Party = GlobalData.parties.Find(x => x.id == partyId);
                Console.WriteLine("TESTAAA " + Party);
                if (Party != null)
                {
                    return Content(JsonConvert.SerializeObject(Party), "application/json");
                }

            }
            catch (Exception ex)
            {
                Logger.Error("FortnitePartyGet: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for GET: /party/api/v1/Fortnite/parties/{partyId}",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/parties/{partyId}" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"NGL for GET: /party/api/v1/Fortnite/parties/{partyId}",
            });
        }


        [HttpGet("api/v1/Fortnite/user/{accountId}")]
        public async Task<IActionResult> FortnitePartyUser(string accountId)
        {
            try
            {
                Response.ContentType = "application/json";
                var UserData = await Handlers.FindOne<User>("accountId", accountId);

                if (UserData != "Error")
                {
                    var CurrentParty = GlobalData.parties.FindAll(e => e.members.Any(a => a.account_id == accountId));
                    var Pings = GlobalData.pings.FindAll(e => e.sent_to == accountId);

                    //Console.WriteLine(new
                    //{
                    //    current = CurrentParty != null ? CurrentParty : new List<Parties>(),
                    //    pending = Array.Empty<object>(),
                    //    invites = Array.Empty<object>(),
                    //    pings = Pings != null ? Pings : new List<Pings>()
                    //});
                    return Ok(new
                    {
                        current = CurrentParty != null ? CurrentParty : new List<Parties>(),
                        pending = Array.Empty<object>(),
                        invites = Array.Empty<object>(),
                        pings = Pings != null ? Pings : new List<Pings>()
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("PartyUserController: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.common.authentication.authentication_failed",
                errorMessage = $"Authentication failed for /api/v1/Fortnite/user/{accountId}",
                messageVars = new List<string> { $"/api/v1/Fortnite/user/{accountId}" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"Authentication failed for /api/v1/Fortnite/user/{accountId}",
            });
        }

        /*
         * 
         *  
         {
            "config":{
                "discoverability":"INVITED_ONLY",
                "join_confirmation":true,
                "joinability":"INVITE_AND_FORMER",
                "max_size":16
            },
            "join_info":
            {
                "connection":{
                    "id":"644812f9-5e5e-4fd4-a670-b306e5956fd9@prod.ol.epicgames.com/V2:Fortnite:WIN::B9CF5D384BD5B481651C10BBC694F713",
                    "meta":{
                        "urn:epic:conn:platform_s":"WIN",
                        "urn:epic:conn:type_s":"game"
                    },
                    "yield_leadership":false
                },
                "meta":{
                    "urn:epic:member:dn_s":"Femboy Ozf"
                }
            },
            "meta":{
                "urn:epic:cfg:party-type-id_s":"default",
                "urn:epic:cfg:build-id_s":"1:3:15301536",
                "urn:epic:cfg:can-join_b":"true",
                "urn:epic:cfg:join-request-action_s":"Manual",
                "urn:epic:cfg:presence-perm_s":"Noone",
                "urn:epic:cfg:invite-perm_s":"Noone",
                "urn:epic:cfg:chat-enabled_b":"true",
                "urn:epic:cfg:accepting-members_b":"false",
                "urn:epic:cfg:not-accepting-members-reason_i":"0"
            }
        }
         * */

        [HttpGet("api/v1/Fortnite/user/{accountId}/notifications/undelivered/count")]
        public async Task<IActionResult> NotfiCount(string accountId)
        {
            try
            {
                Response.ContentType = "application/json";
                var UserData = await Handlers.FindOne<User>("accountId", accountId);

                if (UserData != "Error")
                {
                    var CurrentParty = GlobalData.parties.FindAll(e => e.members.Any(a => a.account_id == accountId));
                    var Pings = GlobalData.pings.FindAll(e => e.sent_to == accountId);

                    return Ok(new
                    {
                        current = CurrentParty != null ? CurrentParty : new List<Parties>(),
                        pending = Array.Empty<object>(),
                        invites = Array.Empty<object>(),
                        pings = Pings != null ? Pings : new List<Pings>()
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("PartyUserController: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.common.authentication.authentication_failed",
                errorMessage = $"Authentication failed for /api/v1/Fortnite/user/{accountId}",
                messageVars = new List<string> { $"/api/v1/Fortnite/user/{accountId}" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"Authentication failed for /api/v1/Fortnite/user/{accountId}",
            });
        }

        [HttpPatch("api/v1/Fortnite/parties/{partyId}")]
        [AuthorizeToken]
        public async Task<IActionResult> FortnitePartyPatch(string partyId)
        {
            try
            {
                Response.ContentType = "application/json";
                var tokenPayload = HttpContext.Items["Payload"] as TokenPayload;
                var AccountId = tokenPayload?.Sub;

                if (AccountId != null)
                {
                    var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;
                    if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                    {
                        using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                        {
                            string requestBody = await reader.ReadToEndAsync();
                      
                            if (string.IsNullOrEmpty(requestBody))
                            {
                                throw new BaseError
                                {
                                    errorCode = "errors.com.epicgames.common.iforgot",
                                    errorMessage = $"No Body for /party/api/v1/Fortnite/parties",
                                    messageVars = new List<string> { $"/party/api/v1/Fortnite/parties" },
                                    numericErrorCode = 1032,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = $"No Body for /party/api/v1/Fortnite/parties",
                                };
                            }

                            int PartyIndex = GlobalData.parties.FindIndex(x => x.id == partyId);
                            if (PartyIndex != -1)
                            {
                                var Parties = GlobalData.parties[PartyIndex];


                                var ClientIndex = GlobalData.Clients.FindIndex(x => x.accountId == AccountId);
                                if (ClientIndex != -1)
                                {
                                    var Client = GlobalData.Clients[ClientIndex];

                                    PatchPartiesC PatchPartyResponse = JsonConvert.DeserializeObject<PatchPartiesC>(requestBody)!;
                                    //PatchPatiesC
                                    if (Parties != null && PatchPartyResponse != null)
                                    {
                                        Console.WriteLine("yeah " + PatchPartyResponse.config);
                                        foreach (var prop in PatchPartyResponse.config.GetType().GetProperties())
                                        {
                                            Parties.config[prop.Name] = prop.GetValue(PatchPartyResponse.config)!;
                                            Logger.Warn($"{prop.Name}: {prop.GetValue(PatchPartyResponse.config)}");
                                        }

                                        if (PatchPartyResponse.meta != null)
                                        {
                                            foreach (var prop in PatchPartyResponse.meta.delete)
                                            {
                                                Parties.meta.Remove(prop);
                                            }

                                            foreach (var prop in PatchPartyResponse.meta.update)
                                            {
                                                if (!Parties.meta.ContainsKey(prop.Key))
                                                {
                                                    Logger.Warn("ADDING !!! YOO!");
                                                    Parties.meta.Add(prop.Key, prop.Value);
                                                }
                                                else
                                                {
                                                    Parties.meta[prop.Key] = prop.Value;
                                                }
                                            }
                                        }

                                        Parties.revision += 1;
                                        Parties.updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");

                                        var captain = Parties.members.FirstOrDefault(x => x.role == "CAPTAIN");

                                        if (captain != null)
                                        {
                       
                                           // Console.WriteLine("r " + JsonConvert.SerializeObject(PartiesGR));
                                           // Console.WriteLine("f " + JsonConvert.SerializeObject(Parties));
                                            
                                            Parties.members.ForEach(async x =>
                                            {
                                                var foundClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == x.account_id);

                                                if (foundClient != null /*&& foundClient.Client.CloseStatus == 0*/)
                                                {
                                                    XElement message;
                                                    XNamespace clientNs = "jabber:client";
               
                                                    message = new XElement(clientNs + "message",
                                                        new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                        new XAttribute("to", foundClient.jid),
                                                        new XElement("body", JsonConvert.SerializeObject(new {
                                                            captain_id = captain.account_id,
                                                            created_at = Parties.created_at,
                                                            invite_ttl_seconds = 14400,
                                                            max_number_of_members = 16,
                                                            ns = "Fortnite",
                                                            party_id = Parties.id,
                                                            party_privacy_type = Parties.privacy,
                                                            party_state_overriden = new { },
                                                            party_state_removed = PatchPartyResponse.meta?.delete,
                                                            party_state_updated = PatchPartyResponse.meta?.update,
                                                            party_sub_type = Parties.meta["urn:epic:cfg:party-type-id_s"],
                                                            party_type = "DEFAULT",
                                                            revision = Parties.revision,
                                                            sent = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                                            type = "com.epicgames.social.party.notification.v0.PARTY_UPDATED",
                                                            updated_at = Parties.updated_at
                                                        }))
                                                    );
                                                    Console.WriteLine("SEND A XMPP MESSAGE");
                                                    await SERVER.Send.Client.SendClientMessage(foundClient, message);
                                                    
                                                }

                                            });

                                            return StatusCode(204);
                                        }

                                    }
                                }
                                else
                                {
                                    Console.WriteLine("CLIENTINDEX IS NULL!!!");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FortnitePartyPatch: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for /party/api/v1/Fortnite/parties/{partyId}",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/parties/{partyId}" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"NGL for /party/api/v1/Fortnite/parties/{partyId}",
            });
        }

        //party/api/v1/Fortnite/parties/333aed1193ce47dca9531f2f82ce5403/members/c2092fc3-8f1b-4d70-a036-3b2461e62a1a/meta

    /*
     * 
     {"delete":[],"revision":0,"update":{"internal:voicechatmuted_b":"false"}}

            */

        [HttpPatch("api/v1/Fortnite/parties/{partyId}/members/{accountId}/meta")]
        public async Task<IActionResult> FortnitePartyPatch(string partyId, string accountId)
        {
            try 
            { 
                Response.ContentType = "application/json";



                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string requestBody = await reader.ReadToEndAsync();
                  //  Console.WriteLine("TEST11 + " + requestBody);
                    if (string.IsNullOrEmpty(requestBody))
                    {
                        throw new BaseError
                        {
                            errorCode = "errors.com.epicgames.common.iforgot",
                            errorMessage = $"No Body for /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/meta",
                            messageVars = new List<string> { $"/party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/meta" },
                            numericErrorCode = 1032,
                            originatingService = "any",
                            intent = "prod",
                            error_description = $"No Body for /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/meta",
                        };
                    }
                    PatchPartyMeta PartiesGR = JsonConvert.DeserializeObject<PatchPartyMeta>(requestBody)!;

                    int index = GlobalData.parties.FindIndex(x => x.id == partyId);
                    if (index != -1 && PartiesGR != null)
                    {
                            var Parties = GlobalData.parties[index];

                    

                            int members = Parties.members.FindIndex(x => x.account_id == accountId);

                            if(members != -1)
                            {
                                if (PartiesGR.delete != null)
                                {
                                    foreach (var prop in PartiesGR.delete)
                                    {
                                        Parties.members[members].meta.Remove(prop);
                                    }
                                }

                                if (PartiesGR.update != null)
                                {
                                    foreach (var prop in PartiesGR.update)
                                    {
                                        if (!Parties.members[members].meta.ContainsKey(prop.Key))
                                        {
                                            Logger.Warn("ADDING !!! YOO!");
                                            Parties.members[members].meta.Add(prop.Key, prop.Value);
                                        }
                                        else
                                        {
                                            Parties.members[members].meta[prop.Key] = prop.Value;
                                        }
                                    }
                                }

                                //Parties.members[members].revision += 1;
                                Parties.members[members].updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
                                Parties.updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");

                                Parties.members.ForEach(async x =>
                                {
                                    var foundClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == x.account_id);

                                    if (foundClient != null /*&& foundClient.Client.CloseStatus == 0*/)
                                    {
                                        XElement message;
                                        XNamespace clientNs = "jabber:client";
                                        /*@"{
                                                ""account_id"": """ + accountId + @""",
                                                ""account_dn"": """ + Parties.members[members].meta["urn:epic:member:dn_s"] + @""",
                                                ""member_state_updated"": " + PartiesGR.update + @",
                                                ""member_state_removed"": " + PartiesGR.delete + @",
                                                ""member_state_overridden"": " + new { } + @",
                                                ""party_id"": " + Parties.id + @",
                                                ""updated_at"": " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @",
                                                ""sent"": " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @",
                                                ""revision"": " + Parties.members[members].revision + @""",
                                                ""ns"": ""Fortnite"",
                                                ""type"": ""com.epicgames.social.party.notification.v0.MEMBER_STATE_UPDATED""
                                            }"
                                        */
                                      //  Console.WriteLine(Parties.members[members].meta);
                                        message = new XElement(clientNs + "message",
                                            new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                            new XAttribute("to", foundClient.jid),
                                            new XElement("body", JsonConvert.SerializeObject(new {
                                                account_id = accountId,
                                                account_dn = x.meta["urn:epic:member:dn_s"],
                                                member_state_updated = PartiesGR.update,
                                                member_state_removed = PartiesGR.delete,
                                                member_state_overridden = new { },
                                                party_id = Parties.id,
                                                updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                                sent = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                                revision = Parties.members[members].revision,
                                                ns = "Fortnite",
                                                type = "com.epicgames.social.party.notification.v0.MEMBER_STATE_UPDATED"
                                            }))
                                        );

                                        await SERVER.Send.Client.SendClientMessage(foundClient, message);
                                    }

                                });

                                return StatusCode(204);
                            }


                            //    var ClientIndex = GlobalData.Clients.FindIndex(x => x.accountId == AccountId);
                            //    if (ClientIndex == -1)
                            //    {
                            //        Console.WriteLine("CLIENTINDEX IS NULL!!!");
                            //    }
                            //    else
                            //    {
                            //        var Client = GlobalData.Clients[ClientIndex];

                            //    }
                        }
                    }
            }
            catch (Exception ex)
            {
                Logger.Error("FortnitePartyPatch: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/meta",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/meta" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"NGL for /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/meta",
            });
        }

            //party/api/v1/Fortnite/user/c2092fc3-8f1b-4d70-a036-3b2461e62a1a/pings/88e30971-b97d-451d-ba55-e6322bcfe31f
        [HttpPost("api/v1/Fortnite/user/{accountId}/pings/{pingerId}")]
        [AuthorizeToken]
        public async Task<IActionResult> FortnitePartyPings(string accountId, string pingerId)
        {
            try
            {
                Response.ContentType = "application/json";
                VersionClass season = await SeasonUserAgent(Request);

                var tokenPayload = HttpContext.Items["Payload"] as TokenPayload;

                var AccountId = tokenPayload?.Sub;

                if (AccountId != null)
                {
                    var SelfprofileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;

                    if (SelfprofileCacheEntry != null && !string.IsNullOrEmpty(SelfprofileCacheEntry.AccountId))
                    {

                        //List<Parties> CurrentPartysPing = GlobalData.pings.FindAll(e => e.sent_to == accountId);

                        //  List<Pings> filteredParties = GlobalData.pings.Where(x => x.sent_to == accountId).FirstOrDefault(x => x.sent_by == pingerId);

                        var matchingPing = GlobalData.pings.FirstOrDefault(x => x.sent_to == accountId && x.sent_by == pingerId);

                        if (matchingPing != null)
                        {
                            GlobalData.pings.RemoveAll(x => x.sent_to == accountId);
                        }

                        

                        var PingResp = new
                        {
                            sent_to = accountId,
                            sent_by = pingerId,
                            sent_at = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                            expires_at = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                            meta = new { },
                        };

                        GlobalData.pings.Add(new Pings
                        {
                            sent_to = accountId,
                            sent_by = pingerId,
                            time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
                        });

                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(pingerId);

                        if (profileCacheEntry != null)
                        {
                            var foundClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == accountId);

                            if (foundClient != null)
                            {
                                XElement message;
                                XNamespace clientNs = "jabber:client";

                                message = new XElement(clientNs + "message",
                                    new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                    new XAttribute("to", foundClient.jid),
                                    new XElement("body", JsonConvert.SerializeObject(new
                                    {
                                        account_id = accountId,
                                        meta = new { },
                                        ns = "Fortnite",
                                        pinger_dn = profileCacheEntry.UserData.Username,
                                        pinger_id = pingerId,
                                        sent = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        version = season.SeasonFull,
                                        type = "com.epicgames.social.party.notification.v0.PING"
                                    }))
                                );

                                await SERVER.Send.Client.SendClientMessage(foundClient, message);
                            }
                        }

                        return Content(JsonConvert.SerializeObject(PingResp), "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FortnitePartyPings: " + ex.Message);

            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for /party/api/v1/Fortnite/user/{accountId}/pings/{pingerId}",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/user/{accountId}/pings/{pingerId}" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"NGL for /party/api/v1/Fortnite/user/{accountId}/pings/{pingerId}",
            });
        }


    // Missing all this time :fire:
        [HttpPost("api/v1/Fortnite/parties/{partyId}/invites/{accountId}")]
        [AuthorizeToken]
        public async Task<IActionResult> PartyInvite(string partyId, string accountId)
        {
            try
            {
                Response.ContentType = "application/json";
                VersionClass season = await SeasonUserAgent(Request);
                var tokenPayload = HttpContext.Items["Payload"] as TokenPayload;

                var AccountId = tokenPayload?.Sub;

                if (AccountId != null)
                {
                    var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;

                    if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                    {
                        Parties CurrentParty = GlobalData.parties.FirstOrDefault(e => e.id == partyId)!;

                        if (CurrentParty != null)
                        {
                            Console.WriteLine(JsonConvert.SerializeObject(CurrentParty));
                            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                            {
                                string requestBody = await reader.ReadToEndAsync();
                                Console.WriteLine("INVITER + " + requestBody);

                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                };

                                var invite = System.Text.Json.JsonSerializer.Deserialize<Invite>(requestBody, options);

                                if (invite != null)
                                {
                                    Console.WriteLine($"Member: {invite.MemberDisplayName ?? "N/A"}");
                                    Console.WriteLine($"Platform: {invite.ConnectionPlatform ?? "N/A"}");

                                    var Invites = new
                                    {
                                        party_id = CurrentParty.id,
                                        sent_by = profileCacheEntry.AccountId,
                                        meta = invite,
                                        sent_to = accountId,
                                        sent_at = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        updated_at = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        expires_at = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        status = "SENT",
                                    };

                                    CurrentParty.invites.Add(Invites);
                                    CurrentParty.updated_at = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");

                                    Console.WriteLine(profileCacheEntry.AccountId);

                                    var UserInviting = CurrentParty.members.FirstOrDefault((member) => member.account_id == profileCacheEntry.AccountId);

                                    Console.WriteLine(JsonConvert.SerializeObject(UserInviting));
                                    if (UserInviting != null)
                                    {
                                        var foundClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == accountId);

                                        if (foundClient != null)
                                        {
                                            XElement message;
                                            XNamespace clientNs = "jabber:client";

                                            message = new XElement(clientNs + "message",
                                                new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                new XAttribute("to", foundClient.jid),
                                                new XElement("body", JsonConvert.SerializeObject(new
                                                {
                                                    expires = Invites.expires_at,
                                                    meta = Invites.meta,
                                                    ns = "Fortnite",
                                                    party_id = Invites.party_id,
                                                    inviter_dn = UserInviting.meta["urn:epic:member:dn_s"],
                                                    inviter_id = profileCacheEntry.AccountId,
                                                    invitee_id = accountId,
                                                    members_count = CurrentParty.members.Count,
                                                    sent_at = Invites.sent_at,
                                                    updated_at = Invites.updated_at,
                                                    friends_ids = CurrentParty.members.FindAll((e) =>
                                                        profileCacheEntry.UserFriends.Accepted.Find((friend) => friend.accountId == e.account_id) != null
                                                    ).Select(member => member.account_id).ToList() ?? new List<string>(),
                                                    sent = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                                    type = "com.epicgames.social.party.notification.v0.INITIAL_INVITE"
                                                }))
                                            );

                                            Console.WriteLine(message.ToString());

                                            await SERVER.Send.Client.SendClientMessage(foundClient, message);
                                        }

                                        return StatusCode(204);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Failed to deserialize the request body.");
                                }
                            }
                         }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("PartyInvite: " + ex.Message);

            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for /party/api/v1/Fortnite/parties/{partyId}/invites/{{accountId}}",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/parties/{partyId}/invites/{accountId}" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"NGL for /party/api/v1/Fortnite/parties/{partyId}/invites/{accountId}",
            });
        }

        [HttpPost("api/v1/Fortnite/parties")]
        public async Task<IActionResult> FortniteParty()
        {
            try
            {
                Response.ContentType = "application/json";
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    Console.WriteLine("Fortnite/parties + " + requestBody);
                    if (string.IsNullOrEmpty(requestBody))
                    {
                        throw new BaseError
                        {
                            errorCode = "errors.com.epicgames.common.iforgot",
                            errorMessage = $"No Body for /party/api/v1/Fortnite/parties",
                            messageVars = new List<string> { $"/party/api/v1/Fortnite/parties" },
                            numericErrorCode = 1032,
                            originatingService = "any",
                            intent = "prod",
                            error_description = $"No Body for /party/api/v1/Fortnite/parties",
                        };
                    }

                    PartiesC Parties = JsonConvert.DeserializeObject<PartiesC>(requestBody)!;

                    if (Parties != null)
                    {


                        var foundClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == Parties.join_info.connection.id.Split('@')[0]);
                        if (foundClient != null)
                        {
                         //   var AccountId = Parties.join_info.connection.id.Split("@")[0];
                            foundClient.id = Guid.NewGuid().ToString().Replace("-", "");
                            foundClient.meta = Parties.meta;

                            var ResponseParty = new Parties
                            {
                                id = foundClient.id,
                                //privacy = "PUBLIC",
                                created_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                config = Parties.config,
                                members = new List<Members>() {
                                    new Members
                                    {
                                        account_id = Parties.join_info.connection.id.Split("@")[0],
                                        meta = Parties.join_info.meta,
                                        connections = new List<Dictionary<string, object>>()
                                        {
                                            new Dictionary<string, object>
                                            {
                                                { "id", Parties.join_info.connection.id },
                                                { "connected_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") },
                                                { "updated_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")},
                                                { "yield_leadership", Parties.join_info.connection.yield_leadership },
                                                { "meta", Parties.join_info.connection.meta }
                                            }
                                        },
                                        revision = 0,
                                        updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        joined_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        role = "CAPTAIN"
                                    }
                                },
                                applicants = new List<object>(),
                                meta = Parties.meta,
                                invites = new List<object>(),
                                revision = 0,
                                intentions = new List<object>(),
                            };

                            int index = GlobalData.parties.FindIndex(x => x.id == ResponseParty.id);
                            if (index == -1)
                            {
                                GlobalData.parties.Add(ResponseParty);
                            }
                            Console.WriteLine("PARTY RESPONSE " + JsonConvert.SerializeObject(ResponseParty));
                            return Content(JsonConvert.SerializeObject(ResponseParty), "application/json");
                        }
                    }
                }
                   // }
                //}

            }
            catch (Exception ex)
            {
                Logger.Error("FortnitePartyPingPost: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for /party/api/v1/Fortnite/parties",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/parties" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"NGL for /party/api/v1/Fortnite/parties",
            });
        }

        //party/api/v1/Fortnite/user/c2092fc3-8f1b-4d70-a036-3b2461e62a1a/notifications/undelivered/count

        //  [DELETE]: /party/api/v1/Fortnite/parties/1174c2d9a9cf4fbdbae665b753b76c22/members/88e30971-b97d-451d-ba55-e6322bcfe31f


        [HttpDelete("api/v1/Fortnite/parties/{partyId}/members/{accountId}")]
        public IActionResult FortniteParty(string partyId, string accountId)
        {
            try
            {
                Response.ContentType = "application/json";

                var Party = GlobalData.parties.Find(x => x.id == partyId);
                if (Party != null)
                {
                    var MemberIndex = Party.members.FindIndex(x => x.account_id == accountId);

                    if (MemberIndex != -1)
                    {
                        var RemovedMember = Party.members[MemberIndex];

                        Party.members.ForEach(async x =>
                        {
                            var foundClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == x.account_id);

                            if (foundClient != null /*&& foundClient.Client.CloseStatus == 0*/)
                            {
                                XElement message;
                                XNamespace clientNs = "jabber:client";
                                /*
                                 * @"{
                                            ""account_id"": """ + accountId + @""",
                                            ""party_id"": """ + Party.id + @""",
                                            ""kicked"": " + true + @",
                                            ""updated_at"": """ + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @""",
                                            ""sent"": """ + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @""",
                                            ""revision"": """ + Party.revision + @""",
                                            ""ns"": ""Fortnite"",
                                            ""type"": ""com.epicgames.social.party.notification.v0.MEMBER_KICKED""
                                        }"
                                */
                                    message = new XElement(clientNs + "message",
                                    new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                    new XAttribute("to", foundClient.jid),
                                    new XElement("body", JsonConvert.SerializeObject(new {
                                        account_id = accountId,
                                        //member_state_update = new { },
                                        party_id = Party.id,
                                       // kicked = true,
                                        //updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        sent = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        revision = Party.revision,
                                        ns = "Fortnite",
                                        type = "com.epicgames.social.party.notification.v0.MEMBER_LEFT"
                                    }))
                                );

                                await SERVER.Send.Client.SendClientMessage(foundClient, message);
                            }
                        });

                        Party.members.Remove(RemovedMember);
                        if (Party.members.Count != 0)
                        {
                            Party.revision += 1;
                            Party.updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
                        }
                        else
                        {
                            GlobalData.parties.Remove(Party);
                        }

                        return StatusCode(204);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error("FortnitePartyDelete: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for Delete: /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/parties/{partyId}/members/{accountId}" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"NGL for Delete: /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}",
            });
        }

        // [POST]: /party/api/v1/Fortnite/parties/67e7a9822fe54880b20df3bcee7c7008/members/88e30971-b97d-451d-ba55-e6322bcfe31f/join

        [HttpPost("api/v1/Fortnite/parties/{partyId}/members/{accountId}/join")]
        [AuthorizeToken]
        public async Task<IActionResult> FortniteJoinParty(string partyId, string accountId)
        {
            try
            {
                Response.ContentType = "application/json";

                var Party = GlobalData.parties.Find(x => x.id == partyId);
                if (Party != null)
                {
                    var tokenPayload = HttpContext.Items["Payload"] as TokenPayload;
                    var AccountId = tokenPayload?.Sub;

                    if (AccountId != null)
                    {
                        var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;

                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                        {
                            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                            {
                                string requestBody = await reader.ReadToEndAsync();
                               // Console.WriteLine("TESTdsa + " + requestBody);
                                if (string.IsNullOrEmpty(requestBody))
                                {
                                    throw new BaseError
                                    {
                                        errorCode = "errors.com.epicgames.iforgot",
                                        errorMessage = $"NGL for Join: /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/join",
                                        messageVars = new List<string> { $"/party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/join" },
                                        numericErrorCode = 1032,
                                        originatingService = "party",
                                        intent = "prod",
                                        error_description = $"NGL for Join: /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/join",
                                    };
                                }

                                PostJoinParty JoinParty = JsonConvert.DeserializeObject<PostJoinParty>(requestBody)!;

                                if (JoinParty != null)
                                {

                                    var ClientIndex = GlobalData.Clients.FindIndex(x => x.accountId == AccountId);

                                    if (ClientIndex != -1)
                                    {
                                        var Client = GlobalData.Clients[ClientIndex];
                                        

                                        var MemberIndex = Party.members.FindIndex(x => x.account_id == accountId);

                                        if (MemberIndex != -1)
                                        {
                                            Party.members.RemoveAt(MemberIndex);
                                        }
                                        else
                                        {
                                            var Member = new Members
                                            {
                                                account_id = JoinParty.connection.id.Split("@")[0],
                                                meta = JoinParty.meta,
                                                connections =  new List<Dictionary<string, object>>() {
                                                   new Dictionary<string, object>
                                                    {
                                                        { "id", JoinParty.connection.id.Split("@")[0] },
                                                        { "connected_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") },
                                                        { "updated_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")},
                                                        { "yield_leadership", JoinParty.connection.yield_leadership ? true : false },
                                                        { "meta", JoinParty.connection.meta }
                                                    }
                                                },
                                                revision = 0,
                                                updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                                joined_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                                role = JoinParty.connection.yield_leadership ? "CAPTAIN": "MEMBER"
                                            };

                                            Party.members.Add(Member);

                                            var v = Party.meta.ContainsKey("Default:RawSquadAssignments_j") ? "Default:RawSquadAssignments_j" : "RawSquadAssignments_j";

                                            if (Party.meta.TryGetValue(v, out var metaValue) && metaValue is string stringValue)
                                            {
                                               //Console.WriteLine(stringValue);
                                                //{"RawSquadAssignments":[{"memberId":"88e30971-b97d-451d-ba55-e6322bcfe31f","absoluteMemberIdx":0}]}
                                                var rsa = JsonConvert.DeserializeObject<RawSquadAssignmentsWrapper>(stringValue);
                                                if (rsa != null)
                                                {
                                                    rsa.RawSquadAssignments ??= new List<RawSquadAssignment>();
                                                    rsa.RawSquadAssignments.Add(new RawSquadAssignment
                                                    {
                                                        memberId = JoinParty.connection.id.Split("@")[0],
                                                        absoluteMemberIdx = Party.members.Count - 1
                                                    });
                                                }
                                                
                                                Party.meta[v] = metaValue;
                                                //Party.meta[v]
                                                Party.revision += 1;
                                                Party.updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");


                                                var Captain = Party.members.Find(x => x.role == "CAPTAIN");

                                                if (Captain != null)
                                                {

                                                    Party.members.ForEach(async x =>
                                                    {
                                                        var foundClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == x.account_id);

                                                        if (foundClient != null)
                                                        {
                                                            XElement message;
                                                            XNamespace clientNs = "jabber:client";
                                                            Console.WriteLine(JoinParty.connection.meta);
                                                            foreach(var testig in JoinParty.connection.meta)
                                                            {
                                                                Console.WriteLine(testig.Key + " " + testig.Value);
                                                            }
                                                            message = new XElement(clientNs + "message",
                                                            new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                            new XAttribute("to", foundClient.jid),
                                                            new XElement("body", JsonConvert.SerializeObject(new
                                                                        {
                                                                            account_id = JoinParty.connection.meta["urn:epic:member:dn_s"],
                                                                            account_dn = JoinParty.connection.id.Split("@")[0],
                                                                            connection = new
                                                                            {
                                                                                connected_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                                                                id = JoinParty.connection.id,
                                                                                meta = JoinParty.connection.meta,
                                                                                updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
                                                                            },
                                                                            joined_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                                                            member_state_updated = JoinParty.meta,
                                                                            ns = "Fortnite",
                                                                            party_id = Party.id,
                                                                            revision = Member.revision,
                                                                            send = Member.joined_at,
                                                                            type = "com.epicgames.social.party.notification.v0.MEMBER_JOINED",
                                                                            updated_at = Member.updated_at
                                                                        }))
                                                         );

                                                            await SERVER.Send.Client.SendClientMessage(foundClient, message);
                                                            // }

                                                            /*
                                                             * @"{
                                                                ""captain_id"": """ + Captain.account_id + @""",
                                                                ""created_at"": """ + Party.created_at + @""",
                                                                ""invite_ttl_seconds"": " + 14400 + @",
                                                                ""max_number_of_members"": " + 16 + @",
                                                                ""ns"": ""Fortnite"",
                                                                ""party_id"": """ + Party.id + @""",
                                                                ""party_privacy_type"": ""PUBLIC"",
                                                                ""party_state_overriden"": " + new { } + @",
                                                                ""party_state_removed"": " + new List<string>() + @",
                                                                ""party_state_updated"": " + JsonConvert.SerializeObject(rsa) + @",
                                                                ""party_sub_type"": ""default"",
                                                                ""party_type"": ""DEFAULT"",
                                                                ""revision"": " + Party.revision + @",
                                                                ""sent"": """ + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @""",
                                                                ""type"": ""com.epicgames.social.party.notification.v0.PARTY_UPDATED"",
                                                                ""updated_at"": """ + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @"""
                                                            }"*/
                                                            //message = new XElement(clientNs + "message",
                                                            //    new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                            //    new XAttribute("to", foundClient.jid),
                                                            //    new XElement("body", JsonConvert.SerializeObject(new
                                                            //    {
                                                            //        captain_id = Captain.account_id,
                                                            //        created_at = Party.created_at,
                                                            //        invite_ttl_seconds = 14400,
                                                            //        max_number_of_members = 16,
                                                            //        ns = "Fortnite",
                                                            //        party_id = Party.id,
                                                            //        party_privacy_type = "PUBLIC",
                                                            //        party_state_overriden = new { },
                                                            //        party_state_removed = new List<string>(),
                                                            //        party_state_updated = rsa,
                                                            //        party_sub_type = "default",
                                                            //        party_type = "DEFAULT",
                                                            //        revision = Party.revision,
                                                            //        sent = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                                            //        type = "com.epicgames.social.party.notification.v0.PARTY_UPDATED",
                                                            //        updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
                                                            //    }))
                                                            //);

                                                            //await SERVER.Send.Client.SendClientMessage(foundClient, message);
                                                            Console.WriteLine("joined PARTY");
                                                        }
                                                    });
                                                }
                                                return Ok(new
                                                {
                                                    status = "JOINED",
                                                    party_id = Party.id
                                                });
                                            }else
                                            {
                                                Console.WriteLine("WTF");
                                            }
                                        }
                                    } else
                                    {
                                        Console.WriteLine("IT IS -1");
                                    }
                                }else
                                {
                                    Console.WriteLine("JOIN PARTY IS NULL");
                                }
                            }
                        }else
                        {
                            Console.WriteLine("ACCOUNT ISNT FOUND");
                        }
                    }else
                    {
                        Console.WriteLine("ACCOUNT NULL");
                    }
                    //var MemberIndex = Party.members.FindIndex(x => x.account_id == accountId);

                    //if (MemberIndex != -1)
                    //{
                    //    var RemovedMember = Party.members[MemberIndex];

                    //    Party.members.Remove(RemovedMember);
                    //    Party.revision += 1;
                    //    Party.updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");

                    //    Party.members.ForEach(async x =>
                    //    {
                    //        var foundClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == x.account_id);

                    //        if (foundClient != null && foundClient.Client.CloseStatus == 0)
                    //        {
                    //            XElement message;
                    //            XNamespace clientNs = "jabber:client";
                    //            message = new XElement(clientNs + "message",
                    //                new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                    //                new XAttribute("to", foundClient.accountId),
                    //                new XElement("body", @"{
                    //                        ""account_id"": """ + accountId + @""",
                    //                        ""party_id"": " + Party.id + @",
                    //                        ""kicked"": " + true + @",
                    //                        ""updated_at"": " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @""",
                    //                        ""sent"": " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @""",
                    //                        ""revision"": " + Party.revision + @""",
                    //                        ""ns"": ""Fortnite"",
                    //                        ""type"": ""com.epicgames.social.party.notification.v0.MEMBER_KICKED""
                    //                    }")
                    //            );

                    //            await XMPP.Helpers.Send.Client.SendClientMessage(foundClient, message);
                    //        }
                    //    });
                    //    return StatusCode(204);
                    //}
                }

            }
            catch (Exception ex)
            {
                Logger.Error("FortnitePartyDelete: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for Join: /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/join",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/join" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"NGL for Join: /party/api/v1/Fortnite/parties/{partyId}/members/{accountId}/join",
            });
        }


    }
}
