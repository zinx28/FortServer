using Amazon.Runtime.Internal.Transform;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Friends;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Shop.Helpers.Data;
using FortBackend.src.App.XMPP.Helpers.Resources;
using FortBackend.src.App.XMPP.Helpers.Send;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Linq;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace FortBackend.src.App.Routes.Friends
{
    [ApiController]
    [Route("party")]
    public class PartyController : ControllerBase
    {
        [HttpGet("api/v1/Fortnite/user/{accountId}")]
        public async Task<IActionResult> FortnitePartyUser(string accountId)
        {
            try
            {
                Response.ContentType = "application/json";
                var UserData = await Handlers.FindOne<User>("accountId", accountId);

                if (UserData != "Error")
                {

                    var CurrentParty = GlobalData.parties.Find(e => e.members.Any(a => a.account_id == accountId));

                    return Ok(new
                    {
                        current = CurrentParty != null ? new List<Parties> { CurrentParty } : new List<Parties>(),
                        pending = Array.Empty<object>(),
                        invites = Array.Empty<object>(),
                        pings = Array.Empty<object>()
                    });
                }else
                {
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
            }
            catch (Exception ex)
            {
                Logger.Error("PartyUserController: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for /party/api/v1/Fortnite/user/{accountId}",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/user/{accountId}" },
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

        [HttpPost("api/v1/Fortnite/parties")]
        public async Task<IActionResult> FortniteParty()
        {
            try
            {
                Response.ContentType = "application/json";
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    Console.WriteLine("TEST + " + requestBody);
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

                    PartiesC Parties = JsonConvert.DeserializeObject<PartiesC>(requestBody);
                    
                    if(Parties != null)
                    {
                        var foundClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == Parties.join_info.connection.id.Split('@')[0]);
                        if(foundClient != null)
                        {
                            var AccountId = Parties.join_info.connection.id.Split('@')[0];
                            foundClient.id = Guid.NewGuid().ToString().Replace("-", "");
                            foundClient.meta = Parties.meta;
                            
                            GlobalData.members.Add(new Members
                            {
                                account_id = AccountId,
                                meta = new Dictionary<string, object>
                                {
                                    {"urn:epic:member:dn_s", Parties.join_info.meta["urn:epic:member:dn_s"] }
                                },
                                connections = new List<Dictionary<string, object>>
                                {
                                    new Dictionary<string, object>
                                    {
                                        { "id", Parties.join_info.connection.id },
                                        { "connected_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") },
                                        { "updated_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")},
                                        { "yield_leadership", false },
                                        { "meta", Parties.join_info.connection.meta }
                                    }
                                },
                                revision = foundClient.revision,
                                updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                joined_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                role = "CAPTAIN"
                            });

                            GlobalData.parties.Add(new Parties
                            {
                                id = foundClient.id,
                                privacy = "PUBLIC",
                                members = GlobalData.members.Where(member => !string.IsNullOrEmpty(member.account_id)).ToList(),
                               // party = typeof(foundClient)
                            });;
                            Console.WriteLine("TEST " + Parties.config);
                            Console.WriteLine("TEST " + Parties.meta);
                            Console.WriteLine("TEST " + Parties.join_info);

                            //AccountId
                            //Notify
                            var Member = GlobalData.members.Select(members => members.account_id == AccountId);
                            foundClient.revision += 1;

                            Console.WriteLine("MEMBERPARTY " + Member);

                            Clients Clients = GlobalData.Clients.FirstOrDefault(client => client.accountId == AccountId);
                            Console.WriteLine("TE");
                            if (Clients == null)
                            {
                                Logger.Error("CLIENT IS NUYLL");
                            }
                            else
                            {
                                XElement message;
                                XNamespace clientNs = "jabber:client";
                                message = new XElement(clientNs + "message",
                                    new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                    new XAttribute("to", Clients.accountId),
                                    new XElement("body", @"{
                                        ""account_dn"": """ + Parties.join_info.meta["urn:epic:member:dn_s"] + @""",
                                        ""account_id"": """ + AccountId + @""",
                                        ""connection"": {
                                            ""connected_at"": """ + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @""",
                                            ""id"": """ + Parties.join_info.connection.id.Split('@')[0] + @""",
                                            ""meta"": " + JsonConvert.SerializeObject(Parties.join_info.connection.meta) + @",
                                            ""updated_at"": """ + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @""",
                                            ""joined_at"": """ + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @"""
                                        },
                                        ""member_state_update"": {
                                            ""urn:epic:member:dn_s"": """ + Parties.join_info.meta["urn:epic:member:dn_s"] + @"""
                                        },
                                        ""ns"": ""Fortnite"",
                                        ""party_id"": """ + foundClient.id + @""",
                                        ""revision"": """ + foundClient.revision + @""",
                                        ""sent"": """ + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @""",
                                        ""type"": ""com.epicgames.social.party.notification.v0.MEMBER_JOINED"",
                                        ""updated_at"": """ + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") + @"""
                                    }")
                                );

                                await FortBackend.src.App.XMPP.Helpers.Send.Client.SendClientMessage(Clients, message);
                            }

                            Console.WriteLine("TES!!T");
                            // New Member ~ 

                            var MemberAccountId = Parties.join_info.connection.id.Split('@')[0];
                            var meta = new Dictionary<string, object>()
                            {
                                { "urn:epic:member:dn_s", Parties.join_info.meta["urn:epic:member:dn_s"] }
                            };

                            var NewMember = new Members
                            {
                                account_id = MemberAccountId,
                                meta = meta,
                                connections = new List<Dictionary<string, object>>
                                {
                                    new Dictionary<string, object>
                                    {
                                        { "id", Parties.join_info.connection.id },
                                        { "connected_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") },
                                        { "updated_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")},
                                        { "yield_leadership", false },
                                        { "meta", Parties.join_info.connection.meta }
                                    }
                                },
                                revision = 0,
                                joined_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                role = "CAPTAIN"
                            };

                            GlobalData.members.Add(NewMember);
                            GlobalData.parties.Add(new Parties
                            {
                                id = foundClient.id,
                                privacy = "OPEN",
                                members = GlobalData.members.Where(member => !string.IsNullOrEmpty(member.account_id)).ToList(),
                            });

                            var ResponseParty = new Parties
                            {
                                id = foundClient.id,
                                privacy = "PUBLIC",
                                created_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                config = Parties.config,
                                members = new List<Members>() {
                                    new Members
                                    {
                                        account_id = MemberAccountId,
                                        meta = new Dictionary<string, object>()
                                        {
                                            { "meta", Parties.meta },
                                            { "connections", new List<Dictionary<string, object>>()
                                                {
                                                   new Dictionary<string, object>
                                                   {
                                                       { "id", Parties.join_info.connection.id },
                                                       { "connected_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") },
                                                       { "updated_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")},
                                                       { "yield_leadership", false },
                                                       { "meta", Parties.join_info.connection.meta }
                                                   }
                                                }
                                            },
                                            { "revision", 0 },
                                            { "joined_at", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") },
                                            { "updated_at", DateTime.Now.ToString("yyyy -MM-ddTHH:mm:ss.fffffffK") },
                                            { "role", "CAPTAIN" }
                                        },
                                        connections = new List<Dictionary<string, object>>(),
                                        revision = 0,
                                        updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        joined_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                                        role = "MEMBER"
                                    }
                                },
                                applicants = new List<object>(),
                                meta = meta,
                                invites = new List<object>(),
                                revision = 0,
                                intentions = new List<object>(),
                            };

                            int index = GlobalData.parties.FindIndex(x => x.id == ResponseParty.id);
                            if (index != -1)
                            {
                                GlobalData.parties[index] = ResponseParty;
                            }
                            Console.WriteLine(JsonConvert.SerializeObject(ResponseParty));
                            return Content(JsonConvert.SerializeObject(ResponseParty));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error("FortniteParty: " + ex.Message);
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
    }
}
