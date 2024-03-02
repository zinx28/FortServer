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
    }
}
