using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FortBackend.src.App.Routes.APIS.FriendsController
{
    [ApiController]
    [Route("friend")]
    public class FriendController : ControllerBase
    {
        [HttpGet("v1/{accountId}/blocklist")]
        public async Task<ActionResult> GrabBlockList(string accountId)
        {
            Response.ContentType = "application/json";
            var FriendList = new List<dynamic>();
            try
            {
                var FriendsData = await Handlers.FindOne<Friends>("accountId", accountId);
                if (FriendsData != "Error")
                {
                    Friends FriendsDataParsed = JsonConvert.DeserializeObject<Friends[]>(FriendsData)?[0];

                    if(FriendsDataParsed != null)
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
                var FriendsData = await Handlers.FindOne<Friends>("accountId", accountId);
                if (FriendsData != "Error")
                {
                    Friends FriendsDataParsed = JsonConvert.DeserializeObject<Friends[]>(FriendsData)[0];
                    if(FriendsDataParsed != null)
                    {
                        foreach (FriendsObject AcceptedList in FriendsDataParsed.Accepted)
                        {
                            response.friends.Add(new
                            {
                                accountId = AcceptedList.accountId.ToString(),
                                groups = Array.Empty<string>(),
                                mutual = 0,
                                alias = AcceptedList.alias != null ? AcceptedList.alias.ToString() : "",
                                note = "",
                                created = DateTime.Parse(AcceptedList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                favorite = false
                            });
                        }

                        foreach (FriendsObject IncomingList in FriendsDataParsed.Incoming)
                        {
                            response.incoming.Add(new
                            {
                                accountId = IncomingList.accountId.ToString(),
                                groups = Array.Empty<string>(),
                                mutual = 0,
                                alias = IncomingList.alias != null ? IncomingList.alias.ToString() : "",
                                note = "",
                                created = DateTime.Parse(IncomingList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                favorite = false
                            });
                        }

                        foreach (FriendsObject OutgoingList in FriendsDataParsed.Outgoing)
                        {
                            response.outgoing.Add(new
                            {
                                accountId = OutgoingList.accountId.ToString(),
                                groups = Array.Empty<string>(),
                                mutual = 0,
                                alias = OutgoingList.alias != null ? OutgoingList.alias.ToString() : "",
                                note = "",
                                created = DateTime.Parse(OutgoingList.created.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                favorite = false
                            });
                        }

                        foreach (FriendsObject BLockedList in FriendsDataParsed.Blocked)
                        {
                            response.blocklist.Add(new
                            {
                                accountId = BLockedList.accountId.ToString(),
                                groups = Array.Empty<string>(),
                                mutual = 0,
                                alias = BLockedList.alias != null ? BLockedList.alias.ToString() : "",
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
    }
}
