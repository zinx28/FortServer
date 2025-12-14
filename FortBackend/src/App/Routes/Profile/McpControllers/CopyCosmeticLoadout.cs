using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Extentions;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB.Module;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class CopyCosmeticLoadout
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, CopyCosmeticLoadoutReq Body)
        {
            if (ProfileId == "athena")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> MultiUpdates = new List<object>();

                if (Body.targetIndex > 100 || Body.sourceIndex > 100 || Body.targetIndex < 0 || Body.sourceIndex < 0)
                {
                    throw new BaseError
                    {
                        errorCode = "errors.com.epicgames.modules.outOfBounds",
                        errorMessage = "OutOfBounds | FR",
                        messageVars = new List<string> { "CopyCosmeticLoadout" },
                        numericErrorCode = 12801,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "OutOfBounds | FR",
                    };
                }

            
                //profileCacheEntry.AccountData.athena.loadouts_data[Body.targetIndex] = profileCacheEntry.AccountData.athena.CosmeticLoadouts[Body.sourceIndex];

                // 0 is sandbox_loadout if this is higher then 0 its reverted and we are copying to sandbox_loadout
                // ^^ 99% sure fortnite deleted the sandbox_loadout and just had presets
                // also deteals how it probally should be
                // sandbox_loadout is the loadout i would say should "copied" and "changed" and we shouldn't change the loadout index
                // but idk mann i could implement this 100 ways
                // i might change this in the future since Yes sir!
                //Console.WriteLine(profileCacheEntry.AccountData.athena.loadouts.Count);
                if (Body.sourceIndex == 0 && Body.targetIndex != 0)
                {
                    string RandomNewId = Guid.NewGuid().ToString();
                    //Body.sourceIndex ~ 0 should always be sandbox loadout
                    // i wished i couldj ust be like rust
                    if ((profileCacheEntry.AccountData.athena.loadouts.Count - 1) < Body.targetIndex)
                    {
                        var ClonedSandbox = System.Text.Json.JsonSerializer.Deserialize<SandboxLoadout>(
                            System.Text.Json.JsonSerializer.Serialize(profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"]))!;

                        profileCacheEntry.AccountData.athena.loadouts_data.Add(RandomNewId, ClonedSandbox);
                        profileCacheEntry.AccountData.athena.loadouts_data[RandomNewId].attributes.locker_name = Body.optNewNameForTarget;
                        profileCacheEntry.AccountData.athena.loadouts.Add(RandomNewId);
                        profileCacheEntry.AccountData.athena.last_applied_loadout = RandomNewId;

                        MultiUpdates.Add(new
                        {
                            changeType = "itemAdded",
                            itemId = RandomNewId,
                            item = ClonedSandbox // doesnr matter this owrks
                        });

                        MultiUpdates.Add(new
                        {
                            changeType = "statModified",
                            name = "loadouts",
                            value = profileCacheEntry.AccountData.athena.loadouts
                        });

                        MultiUpdates.Add(new
                        {
                            changeType = "statModified",
                            name = "last_applied_loadout",
                            value = RandomNewId
                        });

                    }
                    else
                    {
                        string Indx2 = profileCacheEntry.AccountData.athena.loadouts[Body.targetIndex];
                        if (!string.IsNullOrEmpty(Indx2))
                        {
                            if (Indx2 != "sandbox_loadout")
                            {
                                var ClonedSandbox = System.Text.Json.JsonSerializer.Deserialize<SandboxLoadout>(
                                    System.Text.Json.JsonSerializer.Serialize(profileCacheEntry.AccountData.athena.loadouts_data!["sandbox_loadout"]))!;

                                if(ClonedSandbox.attributes != null)
                                {
                                    var Item = profileCacheEntry.AccountData.athena.loadouts_data[Indx2].attributes;
                                    Item.locker_slots_data = ClonedSandbox.attributes.locker_slots_data;
                                    if (Item.banner_color_template != ClonedSandbox.attributes.banner_color_template)
                                    {
                                        Item.banner_color_template = ClonedSandbox.attributes.banner_color_template;
                                        MultiUpdates.Add(new
                                        {
                                            changeType = "itemAttrChanged", // attr is attribute
                                            itemId = Indx2,
                                            attributeName = "banner_color_template",
                                            attributeValue = Item.banner_icon_template
                                        });
                                    }

                                    if (Item.banner_icon_template != ClonedSandbox.attributes.banner_icon_template)
                                    {
                                        Item.banner_icon_template = ClonedSandbox.attributes.banner_icon_template;

                                        MultiUpdates.Add(new
                                        {
                                            changeType = "itemAttrChanged", // attr is attribute
                                            itemId = Indx2,
                                            attributeName = "banner_icon_template",
                                            attributeValue = Item.banner_icon_template
                                        });
                                   }
                              
                                 
                                    MultiUpdates.Add(new
                                    {
                                        changeType = "itemAttrChanged",
                                        itemId = Indx2,
                                        attributeName = "locker_slots_data",
                                        attributeValue = Item.locker_slots_data
                                    });
                                }
                            }
                            else
                            {
                                Logger.Error("This shouldnt happened </3 ", "CopyLoadout");
                            }
                        }
                    }

                }
                else
                {
                    // copy preset 1 -> preset 0
                    // 1 -> 0 
                   
                    if (Body.targetIndex == 0)
                    {
                        string Indx2 = profileCacheEntry.AccountData.athena.loadouts[Body.sourceIndex];
                        if (!string.IsNullOrEmpty(Indx2))
                        {
                            var ClonedSandbox = System.Text.Json.JsonSerializer.Deserialize<SandboxLoadout>(
                                System.Text.Json.JsonSerializer.Serialize(profileCacheEntry.AccountData.athena.loadouts_data[Indx2]))!;

                            if (ClonedSandbox.attributes != null)
                            {
                                var Item = profileCacheEntry.AccountData.athena.loadouts_data["sandbox_loadout"].attributes;
                                Item.locker_slots_data = ClonedSandbox.attributes.locker_slots_data;
                                if (Item.banner_color_template != ClonedSandbox.attributes.banner_color_template)
                                {
                                    Item.banner_color_template = ClonedSandbox.attributes.banner_color_template;
                                    MultiUpdates.Add(new
                                    {
                                        changeType = "itemAttrChanged", // attr is attribute
                                        itemId = "sandbox_loadout",
                                        attributeName = "banner_color_template",
                                        attributeValue = Item.banner_icon_template
                                    });
                                }

                                if (Item.banner_icon_template != ClonedSandbox.attributes.banner_icon_template)
                                {
                                    Item.banner_icon_template = ClonedSandbox.attributes.banner_icon_template;

                                    MultiUpdates.Add(new
                                    {
                                        changeType = "itemAttrChanged", // attr is attribute
                                        itemId = "sandbox_loadout",
                                        attributeName = "banner_icon_template",
                                        attributeValue = Item.banner_icon_template
                                    });
                                }

                                MultiUpdates.Add(new
                                {
                                    changeType = "itemAttrChanged",
                                    itemId = "sandbox_loadout",
                                    attributeName = "locker_slots_data",
                                    attributeValue = Item.locker_slots_data
                                });


                                // when you switch it should also change the preset on the side!!!
                                if (profileCacheEntry.AccountData.athena.last_applied_loadout != Indx2)
                                {
                                    profileCacheEntry.AccountData.athena.last_applied_loadout = Indx2;
                                    MultiUpdates.Add(new
                                    {
                                        changeType = "statModified",
                                        name = "last_applied_loadout",
                                        value = Indx2
                                    });
                                }
                            }
                        }
                    }


                    //string Indx = profileCacheEntry.AccountData.athena.loadouts[Body.targetIndex];
                    //string Indx2 = profileCacheEntry.AccountData.athena.loadouts[Body.sourceIndex];
                    //if (!string.IsNullOrEmpty(Indx))
                    //{
                    //    SandboxLoadout Sandblx = profileCacheEntry.AccountData.athena.loadouts_data[Indx];
                    //    profileCacheEntry.AccountData.athena.last_applied_loadout = Indx;
                    //    profileCacheEntry.AccountData.athena.loadouts_data[Indx2] = Sandblx;
                    //    if(!string.IsNullOrEmpty(Body.optNewNameForTarget))
                    //    {
                    //        profileCacheEntry.AccountData.athena.loadouts_data[Indx2].attributes.locker_name = Body.optNewNameForTarget;
                    //    }


                    //    MultiUpdates.Add(new
                    //    {
                    //        changeType = "statModified",
                    //        name = "last_applied_loadout",
                    //        value = Body.targetIndex
                    //    });

                    //    MultiUpdates.Add(new
                    //    {
                    //        changeType = "statModified",
                    //        name = "active_loadout_index",
                    //        value = Body.targetIndex
                    //    });
                    //}
                   
                }


                if (MultiUpdates.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.UtcNow;
                    profileCacheEntry.AccountData.athena.BumpRevisions();
                }

                if (BaseRev_G != RVN)
                {
                    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    MultiUpdates = test.profileChanges;
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev,
                    profileChanges = MultiUpdates,
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };
            }

            return new Mcp();
        }
    }
}
