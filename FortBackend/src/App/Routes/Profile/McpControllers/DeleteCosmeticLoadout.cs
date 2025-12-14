using Discord;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Extentions;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;
//using MongoDB.Bson.IO;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class DeleteCosmeticLoadout
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, DeleteCosmeticLoadoutReq Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> ProfileChanges = new List<object>();

                // this is my idea of the function from a few tests
                // if you are trying to delete the preset that YOU arent using at the momenet this will be the response
                /*
                 * 3
                 * -1
                 * True
                 */
                //
                // 3 means that this is the preset we will delete <3
                // -1 means that we dont need to switch to a different preset
                // BUT if you are tryinf to delete the one that you are using this is the response
                /*
                 *  4
                 *  3
                 *  True
                 */
                // ^^ deleting the last preset with it equiped, 
                // 4 is the preset we are deleting
                // 3 is the preset we will be going to
                // True is to make it empty? but wouldnt we just remove it from the array

                string Indx2 = profileCacheEntry.AccountData.athena.loadouts![Body.index];
                if (!string.IsNullOrEmpty(Indx2))
                {
                    // We check if someone is trying to delete sandbox, if so we are cooked </3
                    if (Indx2 != "sandbox_loadout")
                    {
                        if(Body.fallbackLoadoutIndex != -1)
                        {
                            string Indx3 = profileCacheEntry.AccountData.athena.loadouts![Body.fallbackLoadoutIndex];
                            if (!string.IsNullOrEmpty(Indx3))
                            {
                                profileCacheEntry.AccountData.athena.last_applied_loadout = Indx3;

                                // I'm not sure if its supposed to auto copy it, it feels wrong to do that
                                ProfileChanges.Add(new
                                {
                                    changeType = "statModified",
                                    name = "last_applied_loadout",
                                    value = Indx3
                                });
                            }
                        }

                        // delete the loadout from array and preset list
                        if(profileCacheEntry.AccountData.athena.loadouts.Find((e) => e == Indx2) != null)
                        {
                            profileCacheEntry.AccountData.athena.loadouts.Remove(Indx2);

                            ProfileChanges.Add(new
                            {
                                changeType = "itemRemoved",
                                itemId = Indx2,
                            });

                            ProfileChanges.Add(new
                            {
                                changeType = "statModified",
                                name = "loadouts",
                                value = profileCacheEntry.AccountData.athena.loadouts
                            });
                        }
                    }

                }


                if (ProfileChanges.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.Now;
                    profileCacheEntry.AccountData.athena.BumpRevisions();
                }

                if (BaseRev_G != RVN)
                {
                    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    ProfileChanges = test.profileChanges;
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev,
                    profileChanges = ProfileChanges,
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };

            }

            return new Mcp();
        }
    }
}
