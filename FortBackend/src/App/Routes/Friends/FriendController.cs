using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Xml.Linq;
using FortLibrary;
using FortLibrary.XMPP;
using FortBackend.src.App.SERVER.Send;
using FortBackend.src.XMPP.Data;

namespace FortBackend.src.App.Routes.Friends
{
    [ApiController]
    [Route("friends/api")]
    public class FriendController : ControllerBase
    {
        [HttpGet("v1/{accountId}/blocklist")]
        public async Task<ActionResult> GrabBlockList(string accountId)
        {
            Response.ContentType = "application/json";
            var FriendList = new List<dynamic>();
            try
            {
                ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    foreach (dynamic BLockedList in profileCacheEntry.UserFriends.Blocked)
                    {
                        FriendList.Add(new
                        {
                            accountId = BLockedList.accountId.ToString(),
                            created = DateTime.Parse(BLockedList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), // skunky
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FriendController: " + ex.Message);
            }

            return Ok(FriendList);
        }

        [HttpGet("public/list/fortnite/{accountId}/recentPlayers")]
        public ActionResult GrabChapter1RecentPlayers(string accountId)
        {
            return Ok(new List<object>());
        }

        [HttpGet("v1/{accountId}/settings")]
        public ActionResult GrabChapter1Settings(string accountId)
        {
            return Ok(new
            {
                acceptInvites = "public"
            });
        }

        [HttpGet("public/blocklist/{accountId}")]
        public async Task<ActionResult> GrabChapter1BlockList(string accountId)
        {
            Response.ContentType = "application/json";
            var FriendList = new List<dynamic>();
            try
            {
                ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    foreach (dynamic BLockedList in profileCacheEntry.UserFriends.Blocked)
                    {
                        FriendList.Add(new
                        {
                            accountId = BLockedList.accountId.ToString(),
                            created = DateTime.Parse(BLockedList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), // skunky
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FriendController: " + ex.Message);
            }

            return Ok(FriendList);
        }

        [HttpGet("public/friends/{accountId}")]
        public async Task<ActionResult> Chapter1FriendList(string accountId)
        {
            Response.ContentType = "application/json";
            // List that only changes when needed to shouldnt have errors
            var response = new List<object>(); ;
            try
            {
                ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    foreach (FriendsObject AcceptedList in profileCacheEntry.UserFriends.Accepted)
                    {
                        response.Add(new
                        {
                            AcceptedList.accountId,
                            status = "ACCEPTED",
                            direction = "INBOUND",
                            created = DateTime.Parse(AcceptedList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            favorite = false
                        });
                    }

                    foreach (FriendsObject IncomingList in profileCacheEntry.UserFriends.Incoming)
                    {
                        response.Add(new
                        {
                            accountId = IncomingList.accountId,
                            status = "PENDING",
                            direction = "INBOUND",
                            groups = Array.Empty<string>(),
                            created = DateTime.Parse(IncomingList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            favorite = false
                        });
                    }

                    foreach (FriendsObject OutgoingList in profileCacheEntry.UserFriends.Outgoing)
                    {
                        response.Add(new
                        {
                            OutgoingList.accountId,
                            status = "PENDING",
                            direction = "OUTBOUND",
                            created = DateTime.Parse(OutgoingList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            favorite = false
                        });
                    }

                    foreach (FriendsObject BLockedList in profileCacheEntry.UserFriends.Blocked)
                    {
                        response.Add(new
                        {
                            BLockedList.accountId,
                            status = "BLOCKED",
                            direction = "INBOUND",
                            created = DateTime.Parse(BLockedList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            favorite = false
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("[Friends:Chapter1Friendlist] ->" + ex.Message);
            }
            return Ok(response);
        }

        [HttpGet("v1/{accountId}/summary")]
        public async Task<ActionResult> SummaryList(string accountId)
        {
            Response.ContentType = "application/json";
            // List that only changes when needed to shouldnt have errors
            var response = new
            {
                friends = new List<object>(),
                incoming = new List<object>(),
                outgoing = new List<object>(),
                suggested = new List<object>(),
                blocklist = new List<object>(),
                settings = new
                {
                    acceptInvites = "public"
                }
            };
            try
            {
                ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                
                    foreach (FriendsObject AcceptedList in profileCacheEntry.UserFriends.Accepted)
                    {
                        response.friends.Add(new
                        {
                            AcceptedList.accountId,
                            groups = Array.Empty<string>(),
                            mutual = 0,
                            alias = AcceptedList.alias != null ? AcceptedList.alias : "",
                            note = "",
                            created = DateTime.Parse(AcceptedList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            favorite = false
                        });
                    }

                    foreach (FriendsObject IncomingList in profileCacheEntry.UserFriends.Incoming)
                    {
                        response.incoming.Add(new
                        {
                            IncomingList.accountId,
                            groups = Array.Empty<string>(),
                            mutual = 0,
                            alias = IncomingList.alias != null ? IncomingList.alias : "",
                            note = "",
                            created = DateTime.Parse(IncomingList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            favorite = false
                        });
                    }

                    foreach (FriendsObject OutgoingList in profileCacheEntry.UserFriends.Outgoing)
                    {
                        response.outgoing.Add(new
                        {
                            OutgoingList.accountId,
                            groups = Array.Empty<string>(),
                            mutual = 0,
                            alias = OutgoingList.alias != null ? OutgoingList.alias : "",
                            note = "",
                            created = DateTime.Parse(OutgoingList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            favorite = false
                        });
                    }

                    foreach (FriendsObject BLockedList in profileCacheEntry.UserFriends.Blocked)
                    {
                        response.blocklist.Add(new
                        {
                            BLockedList.accountId,
                            groups = Array.Empty<string>(),
                            mutual = 0,
                            alias = BLockedList.alias != null ? BLockedList.alias : "",
                            note = "",
                            created = DateTime.Parse(BLockedList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            favorite = false
                        });
                    }  
                }
            }
            catch (Exception ex)
            {
                Logger.Error("[Friends:SummaryList] ->" + ex.Message);
            }
            return Ok(response);
        }

        [HttpGet("v1/{accountId}/recent/fortnite")]
        public ActionResult RecentFriends(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }


        [HttpPost("v1/{accountId}/friends/{friendId}")]
        public async Task<ActionResult> FriendsAccountId(string accountId, string friendID)
        {
            Response.ContentType = "application/json";
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split("bearer ")[1];
                var accessToken = token.Replace("eg1~", "");
                bool FoundAccount = false;
                if (GlobalData.AccessToken.Any(e => e.token == token))
                    FoundAccount = true;
                else if (GlobalData.ClientToken.Any(e => e.token == token))
                    FoundAccount = true;
                else if (GlobalData.RefreshToken.Any(e => e.token == token))
                    FoundAccount = true;

                if (FoundAccount && !string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(friendID))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var decodedToken = handler.ReadJwtToken(accessToken);

                    var displayName = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "dn")?.Value;
                    var accountId1 = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
                    var clientId = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "clid")?.Value;

                    if (!string.IsNullOrEmpty(accountId1))
                    {
                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId1); // use the auth account not from url
                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                        {
                            if (profileCacheEntry.AccountData != null && profileCacheEntry.UserData != null)
                            {
                                if (profileCacheEntry.UserData.banned)
                                {
                                    return StatusCode(403);
                                }
                            }
                            ProfileCacheEntry friendsprofileCacheEntry = await GrabData.Profile(friendID); // friends
                            if (friendsprofileCacheEntry != null && !string.IsNullOrEmpty(friendsprofileCacheEntry.AccountId))
                            {
                                if (profileCacheEntry.UserFriends.Incoming != null && friendsprofileCacheEntry.UserFriends.AccountId != null)
                                {
                                    bool? FoundFriend = profileCacheEntry.UserFriends.Incoming.Any(account => account?.accountId?.ToString() == friendsprofileCacheEntry.UserFriends.AccountId?.ToString());

                                    Console.WriteLine(FoundFriend);
                                    if (FoundFriend.HasValue && FoundFriend.Value)
                                    {
                                        //Jarray2 == FriendsAccountDataParsed
                                        List<FriendsObject> incomingFriendsArray = profileCacheEntry.UserFriends.Incoming;
                                        List<FriendsObject> incomingFriendsArray2 = friendsprofileCacheEntry.UserFriends.Outgoing;

                                        if (!incomingFriendsArray.Any(friend =>
                                        {
                                            var friendObject = friend;
                                            return friendObject != null && friendObject.accountId?.ToString() == friendID;
                                        }))
                                        {
                                            return StatusCode(403);
                                        }

                                        if (!incomingFriendsArray2.Any(friend =>
                                        {
                                            var friendObject = friend;
                                            return friendObject != null && friendObject.accountId?.ToString() == accountId;
                                        }))
                                        {
                                            return StatusCode(403);

                                        }

                                        var itemsToRemove = incomingFriendsArray.Where(friend =>
                                        {
                                            var friendObject = friend;
                                            return friendObject != null && friendObject.accountId?.ToString() == friendID;
                                        }).ToList();

                                        foreach (var item in itemsToRemove)
                                        {
                                            incomingFriendsArray.Remove(item);
                                        }

                                        profileCacheEntry.UserFriends.Incoming.AddRange(incomingFriendsArray);

                                        var newFriend3 = new FriendsObject
                                        {
                                            accountId = friendID,
                                            created = DateTime.UtcNow
                                        };

                                        profileCacheEntry.UserFriends.Accepted.Add(newFriend3);

                                        var itemsToRemove2 = incomingFriendsArray2.Where(friend =>
                                        {
                                            var friendObject = friend;
                                            return friendObject != null && friendObject.accountId?.ToString() == accountId;
                                        }).ToList();

                                        foreach (var item in itemsToRemove2)
                                        {
                                            incomingFriendsArray2.Remove(item);
                                        }

                                        friendsprofileCacheEntry.UserFriends.Outgoing.AddRange(incomingFriendsArray2);

                                        var newFriend4 = new FriendsObject
                                        {
                                            accountId = accountId.ToString(),
                                            created = DateTime.UtcNow
                                        };

                                        friendsprofileCacheEntry.UserFriends.Accepted.Add(newFriend4);

                                        Clients targetClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == accountId)!;
                                        Clients targetClient2 = GlobalData.Clients.FirstOrDefault(client => client.accountId == friendID)!;
                                        //    Services.Xmpp.Helpers.Send.Client.SendClientMessage(targetClient2, message);

                                        XNamespace clientNs = "jabber:client";

                                        XElement message;

                                        message = new XElement(clientNs + "message",
                                            new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                            new XAttribute("to", accountId),
                                            new XElement("body", @"{
                                            ""payload"": {
                                                ""accountId"": """ + friendsprofileCacheEntry.AccountId + @""",
                                                ""status"": ""ACCEPTED"",
                                                ""direction"": ""OUTBOUND"",
                                                ""created"": """ + DateTime.UtcNow.ToString("o") + @""",
                                                ""favorite"": false
                                            },
                                            ""type"": ""com.epicgames.friends.core.apiobjects.Friend"",
                                            ""timestamp"": """ + DateTime.UtcNow.ToString("o") + @"""
                                        }")
                                        );

                                        await Client.SendClientMessage(targetClient, message);


                                        message = new XElement(clientNs + "message",
                                               new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                               new XAttribute("to", friendsprofileCacheEntry.AccountId),
                                               new XElement("body", @"{
                                                ""payload"": {
                                                    ""accountId"": """ + accountId1 + @""",
                                                    ""status"": ""ACCEPTED"",
                                                    ""direction"": ""INBOUND"",
                                                    ""created"": """ + DateTime.UtcNow.ToString("o") + @""",
                                                    ""favorite"": false
                                                },
                                                ""type"": ""com.epicgames.friends.core.apiobjects.Friend"",
                                                ""timestamp"": """ + DateTime.UtcNow.ToString("o") + @"""
                                            }")
                                           );

                                        await Client.SendClientMessage(targetClient2, message);


                                        message = new XElement(clientNs + "message",
                                            new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                            new XAttribute("to", accountId),
                                            new XElement("type", "available")
                                        );


                                        await Client.SendClientMessage(targetClient, message);

                                        message = new XElement(clientNs + "message",
                                            new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                            new XAttribute("to", accountId),
                                            new XElement("type", "available")
                                        );
                                        await Client.SendClientMessage(targetClient2, message);


                                        return StatusCode(204);
                                    }
                                    else
                                    {
                                        //Jarray2 == FriendsAccountDataParsed
                                        List<FriendsObject> incomingToken = profileCacheEntry.UserFriends.Outgoing;
                                        List<FriendsObject> incomingToken2 = friendsprofileCacheEntry.UserFriends.Incoming;

                                        if (incomingToken != null && incomingToken2 != null)
                                        {
                                            //List<object> incomingFriends = incomingToken.ToObject<List<object>>();

                                            var newFriend = new FriendsObject
                                            {
                                                accountId = friendsprofileCacheEntry.AccountId,
                                                alias = "", // idk
                                                created = DateTime.UtcNow
                                            };

                                            string json = JsonConvert.SerializeObject(newFriend);
                                            var jsonDeserialized = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                                            profileCacheEntry.UserFriends.Outgoing.Add(newFriend);

                                            var newFriend2 = new FriendsObject
                                            {
                                                accountId = accountId1,
                                                alias = "",
                                                created = DateTime.UtcNow
                                            };
                                            string json1 = JsonConvert.SerializeObject(newFriend2);
                                            var jsonDeserialized1 = JsonConvert.DeserializeObject<Dictionary<string, object>>(json1);

                                            friendsprofileCacheEntry.UserFriends.Incoming.Add(newFriend2);

                                            Clients targetClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == accountId)!;
                                            Clients targetClient2 = GlobalData.Clients.FirstOrDefault(client => client.accountId == friendID)!;
                                            
                                            if(targetClient != null && targetClient2 != null)
                                            {
                                                //Services.Xmpp.Helpers.Send.Client.SendClientMessage(targetClient, message);
                                                XNamespace clientNs = "jabber:client";
                                                XElement message;

                                                message = new XElement(clientNs + "message",
                                                new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                new XAttribute("to", accountId),
                                                new XElement("body", @"{
                                                    ""payload"": {
                                                        ""accountId"": """ + friendsprofileCacheEntry.AccountId + @""",
                                                        ""status"": ""PENDING"",
                                                        ""direction"": ""OUTBOUND"",
                                                        ""created"": """ + DateTime.UtcNow.ToString("o") + @""",
                                                        ""favorite"": false
                                                    },
                                                    ""type"": ""com.epicgames.friends.core.apiobjects.Friend"",
                                                    ""timestamp"": """ + DateTime.UtcNow.ToString("o") + @"""
                                                }")
                                                );

                                                await Client.SendClientMessage(targetClient, message);

                                                message = new XElement(clientNs + "message",
                                                new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                new XAttribute("to", friendsprofileCacheEntry.AccountId),
                                                new XElement("body", @"{
                                                    ""payload"": {
                                                        ""accountId"": """ + accountId1 + @""",
                                                        ""status"": ""PENDING"",
                                                        ""direction"": ""INBOUND"",
                                                        ""created"": """ + DateTime.UtcNow.ToString("o") + @""",
                                                        ""favorite"": false
                                                    },
                                                    ""type"": ""com.epicgames.friends.core.apiobjects.Friend"",
                                                    ""timestamp"": """ + DateTime.UtcNow.ToString("o") + @"""
                                                }")
                                                );
                                                await Client.SendClientMessage(targetClient2, message);

                                            }



                                            return StatusCode(204);
                                        }
                                        else
                                        {
                                            return StatusCode(403);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return StatusCode(403);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[Friends:Chapter1] -> {ex.Message}");
            }

            return StatusCode(403);

        }

        [HttpDelete("v1/{accountId}/friends/{friendId}")]
        public async Task<ActionResult> RemoveFriendsV1(string accountId, string friendID)
        {
            Response.ContentType = "application/json";
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split("bearer ")[1];
                var accessToken = token.Replace("eg1~", "");

                bool FoundAccount = false;
                if (GlobalData.AccessToken.Any(e => e.token == token))
                    FoundAccount = true;
                else if (GlobalData.ClientToken.Any(e => e.token == token))
                    FoundAccount = true;
                else if (GlobalData.RefreshToken.Any(e => e.token == token))
                    FoundAccount = true;

                if (FoundAccount)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var decodedToken = handler.ReadJwtToken(accessToken);

                    Console.WriteLine(decodedToken);

                    var displayName = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "dn")?.Value;
                    var accountId1 = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
                    var clientId = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "clid")?.Value;

                    if (accountId1 != null)
                    {
                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId1);
                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                        {
                            if (profileCacheEntry.UserData.banned == true) // imagine banned person redirected few apis then the rest worked
                            {
                                return StatusCode(403);
                            }

                            ProfileCacheEntry friendsprofileCacheEntry = await GrabData.Profile(friendID);
                            if (friendsprofileCacheEntry != null && !string.IsNullOrEmpty(friendsprofileCacheEntry.AccountId))
                            {
                                // basically we use the data account id for people who could call the auth the change the account from the request
                                if (friendsprofileCacheEntry.UserFriends.Accepted.Find(d => d.accountId == accountId1) != null)
                                {
                                    await Handlers.PullFromArray<UserFriends>("accountId", friendsprofileCacheEntry.AccountId, "accepted", "accountId", profileCacheEntry.AccountId);
                                    await Handlers.PullFromArray<UserFriends>("accountId", profileCacheEntry.AccountId, "accepted", "accountId", friendsprofileCacheEntry.AccountId);
                                }

                                if (profileCacheEntry.UserFriends.Incoming != null)
                                {
                                    await XmppFriend.SendMessageToId(new
                                    {
                                        Payload = new
                                        {
                                            AccountId = profileCacheEntry.AccountId,
                                            Reason = "DELETED"
                                        },
                                        Type = "com.epicgames.friends.core.apiobjects.FriendRemoval",
                                        Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                    }, friendsprofileCacheEntry.AccountId);

                                    await XmppFriend.SendMessageToId(new
                                    {
                                        Payload = new
                                        {
                                            AccountId = friendsprofileCacheEntry.AccountId,
                                            Reason = "DELETED"
                                        },
                                        Type = "com.epicgames.friends.core.apiobjects.FriendRemoval",
                                        Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                    }, profileCacheEntry.AccountId);
                                }
                                return StatusCode(204);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return StatusCode(403);
        }

    }
}
