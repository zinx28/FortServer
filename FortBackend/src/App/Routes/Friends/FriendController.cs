using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.XMPP.Helpers.Resources;
using FortBackend.src.App.XMPP.Helpers.Send;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Xml.Linq;

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
                var FriendsData = await Handlers.FindOne<UserFriends>("accountId", accountId);
                if (FriendsData != "Error")
                {
                    UserFriends FriendsDataParsed = JsonConvert.DeserializeObject<UserFriends[]>(FriendsData)?[0];

                    if (FriendsDataParsed != null)
                    {
                        foreach (dynamic BLockedList in FriendsDataParsed.Blocked)
                        {
                            FriendList.Add(new
                            {
                                accountId = BLockedList.accountId.ToString(),
                                created = DateTime.Parse(BLockedList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), // skunky
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FriendController: " + ex.Message);
            }

            return Ok(FriendList);
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
                var FriendsData = await Handlers.FindOne<UserFriends>("accountId", accountId);
                if (FriendsData != "Error")
                {
                    UserFriends FriendsDataParsed = JsonConvert.DeserializeObject<UserFriends[]>(FriendsData)[0];
                    if (FriendsDataParsed != null)
                    {
                        foreach (FriendsObject AcceptedList in FriendsDataParsed.Accepted)
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

                        foreach (FriendsObject IncomingList in FriendsDataParsed.Incoming)
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

                        foreach (FriendsObject OutgoingList in FriendsDataParsed.Outgoing)
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

                        foreach (FriendsObject BLockedList in FriendsDataParsed.Blocked)
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
            }
            catch (Exception ex)
            {
                Logger.Error("[Friends:SummaryList] ->" + ex.Message);
            }
            return Ok(response);
        }

        [HttpGet("v1/{accountId}/recent/fortnite")]
        public async Task<ActionResult> RecentFriends(string accountId)
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
                var handler = new JwtSecurityTokenHandler();
                var decodedToken = handler.ReadJwtToken(accessToken);

                var displayName = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "dn")?.Value;
                var accountId1 = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
                var clientId = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "clid")?.Value;

                if(!string.IsNullOrEmpty(accountId1))
                {
                    var UserData = await Handlers.FindOne<User>("accountId", accountId1);
                    var FriendsData = await Handlers.FindOne<UserFriends>("accountId", accountId1);
                    var AccountData = await Handlers.FindOne<Account>("accountId", accountId1);

                    if (UserData != "Error" && FriendsData != "Error" && AccountData != "Error")
                    {
                        User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)?[0];
                        UserFriends FriendsDataParsed = JsonConvert.DeserializeObject<UserFriends[]>(FriendsData)?[0];
                        Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)?[0];

                        if (AccountDataParsed != null && UserDataParsed != null && AccountDataParsed.ToString().Contains(accessToken))
                        {
                            if (UserDataParsed.banned == true)
                            {
                                return StatusCode(403);
                            }
                        }
                        Console.WriteLine("TEST");
                        var FriendsAccountData = await Handlers.FindOne<UserFriends>("accountId", friendID);
                        if (FriendsAccountData != "Error")
                        {
                            Console.WriteLine("TEST2");
                            UserFriends FriendsAccountDataParsed = JsonConvert.DeserializeObject<UserFriends[]>(FriendsAccountData)[0];

                            if (FriendsAccountDataParsed == null)
                            {
                                return StatusCode(403);
                            }

                            if (FriendsDataParsed != null && FriendsDataParsed.Incoming != null && FriendsAccountDataParsed.AccountId != null)
                            {
                                bool? FoundFriend = FriendsDataParsed.Incoming.Any(account => account?.accountId?.ToString() == FriendsAccountDataParsed.AccountId?.ToString());

                                Console.WriteLine(FoundFriend);
                                if (FoundFriend.HasValue && FoundFriend.Value)
                                {
                                    //Jarray2 == FriendsAccountDataParsed
                                    List<FriendsObject> incomingFriendsArray = FriendsDataParsed.Incoming;
                                    List<FriendsObject> incomingFriendsArray2 = FriendsAccountDataParsed.Outgoing;

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

                                    await Handlers.UpdateOne<UserFriends>("accountId", accountId, new Dictionary<string, object>
                                    {
                                        {
                                                $"incoming", incomingFriendsArray
                                        }
                                    });

                                    var newFriend3 = new FriendsObject
                                    {
                                        accountId = friendID,
                                        created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                    };
                                    string json2 = JsonConvert.SerializeObject(newFriend3);
                                    var jsonDeserialized2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(json2);
                                    await Handlers.PushOne<UserFriends>("accountId", accountId, new Dictionary<string, object>
                                    {
                                        {
                                                $"accepted", jsonDeserialized2
                                        }
                                    });

                                    var itemsToRemove2 = incomingFriendsArray2.Where(friend =>
                                    {
                                        var friendObject = friend;
                                        return friendObject != null && friendObject.accountId?.ToString() == accountId;
                                    }).ToList();

                                    foreach (var item in itemsToRemove2)
                                    {
                                        incomingFriendsArray2.Remove(item);
                                    }

                                    await Handlers.UpdateOne<UserFriends>("accountId", friendID, new Dictionary<string, object>
                                    {
                                        {
                                                $"outgoing", incomingFriendsArray2
                                        }
                                    });

                                    var newFriend4 = new FriendsObject
                                    {
                                        accountId = accountId.ToString(),
                                        created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                    };
                                    string json3 = JsonConvert.SerializeObject(newFriend4);
                                    var jsonDeserialized3 = JsonConvert.DeserializeObject<Dictionary<string, object>>(json3);
                                    await Handlers.PushOne<UserFriends>("accountId", friendID, new Dictionary<string, object>
                                    {
                                        {
                                                $"accepted", jsonDeserialized3
                                        }
                                    });

                                    Clients targetClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == accountId);
                                    Clients targetClient2 = GlobalData.Clients.FirstOrDefault(client => client.accountId == friendID);
                                    //    Services.Xmpp.Helpers.Send.Client.SendClientMessage(targetClient2, message);

                                    XNamespace clientNs = "jabber:client";

                                    XElement message;

                                    message = new XElement(clientNs + "message",
                                        new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                        new XAttribute("to", accountId),
                                        new XElement("body", @"{
                                            ""payload"": {
                                                ""accountId"": """ + FriendsAccountDataParsed.AccountId.ToString() + @""",
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
                                           new XAttribute("to", FriendsAccountDataParsed.AccountId.ToString()),
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
                                    List<FriendsObject> incomingToken = FriendsDataParsed.Outgoing;
                                    List<FriendsObject> incomingToken2 = FriendsAccountDataParsed.Incoming;

                                    if (incomingToken != null && incomingToken2 != null)
                                    {
                                        //List<object> incomingFriends = incomingToken.ToObject<List<object>>();

                                        var newFriend = new FriendsObject
                                        {
                                            accountId = FriendsAccountDataParsed.AccountId.ToString(),
                                            alias = "", // idk
                                            created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                        };

                                        string json = JsonConvert.SerializeObject(newFriend);
                                        var jsonDeserialized = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                        await Handlers.PushOne<UserFriends>("accountId", accountId, new Dictionary<string, object>
                                        {
                                            {
                                                    $"outgoing", jsonDeserialized
                                            }
                                        });

                                        var newFriend2 = new FriendsObject
                                        {
                                            accountId = accountId1,
                                            alias = "",
                                            created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                        };
                                        string json1 = JsonConvert.SerializeObject(newFriend2);
                                        var jsonDeserialized1 = JsonConvert.DeserializeObject<Dictionary<string, object>>(json1);

                                        await Handlers.PushOne<UserFriends>("accountId", FriendsAccountDataParsed.AccountId.ToString(), new Dictionary<string, object>
                                        {
                                            {
                                                    $"incoming", jsonDeserialized1
                                            }
                                        });
                                        Clients targetClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == accountId);
                                        Clients targetClient2 = GlobalData.Clients.FirstOrDefault(client => client.accountId == friendID);
                                        //Services.Xmpp.Helpers.Send.Client.SendClientMessage(targetClient, message);
                                        XNamespace clientNs = "jabber:client";
                                        XElement message;

                                        message = new XElement(clientNs + "message",
                                                  new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                  new XAttribute("to", accountId),
                                                  new XElement("body", @"{
                                                ""payload"": {
                                                    ""accountId"": """ + FriendsAccountDataParsed.AccountId.ToString() + @""",
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
                                                  new XAttribute("to", FriendsAccountDataParsed.AccountId.ToString()),
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
                var handler = new JwtSecurityTokenHandler();
                var decodedToken = handler.ReadJwtToken(accessToken);

                Console.WriteLine(decodedToken);

                var displayName = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "dn")?.Value;
                var accountId1 = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
                var clientId = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "clid")?.Value;

                var UserData = await Handlers.FindOne<User>("accountId", accountId1);
                var FriendsData = await Handlers.FindOne<UserFriends>("accountId", accountId1);
                var AccountData = await Handlers.FindOne<Account>("accountId", accountId1);

                if (UserData != "Error" && FriendsData != "Error" && AccountData != "Error")
                {
                    User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)?[0];
                    UserFriends FriendsDataParsed = JsonConvert.DeserializeObject<UserFriends[]>(FriendsData)?[0];
                    Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)?[0];

                    if (AccountDataParsed != null && UserDataParsed != null && AccountDataParsed.ToString().Contains(accessToken))
                    {
                        if (UserDataParsed.banned == true)
                        {
                            return StatusCode(403);
                        }
                    }


                    var FriendsAccountData = await Handlers.FindOne<UserFriends>("accountId", friendID);
                    if (FriendsAccountData != "Error")
                    {
                        UserFriends FriendsAccountDataParsed = JsonConvert.DeserializeObject<UserFriends[]>(FriendsAccountData)[0];

                        if (FriendsAccountDataParsed == null)
                        {
                            return StatusCode(403);
                        }
                        if (FriendsAccountDataParsed.Accepted.Find(d => d.accountId == accountId1) != null)
                        {
                            await Handlers.PullFromArray<UserFriends>("accountId", friendID, "accepted", "accountId", accountId);
                            await Handlers.PullFromArray<UserFriends>("accountId", accountId, "accepted", "accountId", friendID);
                        }


                        if (FriendsDataParsed != null && FriendsDataParsed.Incoming != null && FriendsAccountDataParsed.AccountId != null)
                        {


                            await XmppFriend.SendMessageToId(new
                            {
                                Payload = new
                                {
                                    AccountId = FriendsDataParsed.AccountId,
                                    Reason = "DELETED"
                                },
                                Type = "com.epicgames.friends.core.apiobjects.FriendRemoval",
                                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                            }, FriendsAccountDataParsed.AccountId);

                            await XmppFriend.SendMessageToId(new
                            {
                                Payload = new
                                {
                                    AccountId = FriendsAccountDataParsed.AccountId,
                                    Reason = "DELETED"
                                },
                                Type = "com.epicgames.friends.core.apiobjects.FriendRemoval",
                                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                            }, FriendsDataParsed.AccountId);
                        }
                        return StatusCode(204);
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
