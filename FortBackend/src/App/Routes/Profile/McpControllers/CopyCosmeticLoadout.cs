﻿using Amazon.Runtime.Internal.Transform;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class CopyCosmeticLoadout
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, Account AccountDataParsed, CopyCosmeticLoadoutResponse Body)
        {
            Console.WriteLine(ProfileId);
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                Dictionary<string, object> UpdatedData = new Dictionary<string, object>();
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
                int BaseRev = AccountDataParsed.athena.RVN;

                int GrabPlacement3 = AccountDataParsed.athena.Items.SelectMany((item, index) => new List<(Dictionary<string, object> Item, int Index)> { (Item: item, Index: index) })
                           .TakeWhile(pair => !pair.Item.ContainsKey("sandbox_loadout")).Count();
                int GrabPlacement2 = AccountDataParsed.athena.Items.SelectMany((item, index) => new List<(Dictionary<string, object> Item, int Index)> { (Item: item, Index: index) })
                    .TakeWhile(pair => !pair.Item.ContainsKey(AccountDataParsed.athena.last_applied_loadout)).Count();

                if (Body.targetIndex < AccountDataParsed.athena.loadouts.Length && !string.IsNullOrEmpty(AccountDataParsed.athena.loadouts[Body.targetIndex]))
                {
                    string[] loadouts = AccountDataParsed.athena.loadouts;
                    object objectToModify = AccountDataParsed.athena.Items.FirstOrDefault(item => item.ContainsKey(loadouts[Body.sourceIndex]));
                    Dictionary<string, object> GrabbedPlaceMent = AccountDataParsed.athena.Items[GrabPlacement2] as Dictionary<string, object>;
                    Console.WriteLine(AccountDataParsed.athena.last_applied_loadout);

                    if (objectToModify != null)
                    {
                        if (!string.IsNullOrEmpty(Body.optNewNameForTarget))
                        {


                            UpdatedData.Add($"athena.Items.{GrabPlacement2}.{AccountDataParsed.athena.last_applied_loadout}.attributes.locker_name", Body.optNewNameForTarget);
                        }

                        object objectToModify1 = GrabbedPlaceMent[AccountDataParsed.athena.last_applied_loadout];
                        if (objectToModify1 is JObject jsonLockerObject)
                        {
                            UpdatedData.Add($"athena.Items.{GrabPlacement3}.sandbox_loadout.attributes.locker_slots_data", new Dictionary<string, object>
                            {
                                {
                                    "slots", new Dictionary<string, object>
                                    {
                                        {
                                            "musicpack", new Dictionary<string, object>
                                            {
                                                {
                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["musicpack"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                }
                                            }
                                        },
                                        {
                                            "character", new Dictionary<string, object>
                                            {
                                                {
                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["character"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                }
                                            }
                                        },
                                        {
                                            "backpack", new Dictionary<string, object>
                                            {
                                                {
                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["backpack"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                }
                                            }
                                        },
                                        {
                                            "pickaxe", new Dictionary<string, object>
                                            {
                                                {
                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["pickaxe"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                }
                                            }
                                        },
                                        {
                                            "skydivecontrail", new Dictionary<string, object>
                                            {
                                                {
                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["skydivecontrail"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                }
                                            }
                                        },
                                        {
                                            "dance", new Dictionary<string, object>
                                            {
                                                {
                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["dance"]["items"].ToObject<string[]>() ?? new string[] { "", "", "", "", "" }
                                                }
                                            }
                                        },
                                        {
                                            "loadingscreen", new Dictionary<string, object>
                                            {
                                                {
                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["loadingscreen"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                }
                                            }
                                        },
                                        {
                                            "glider", new Dictionary<string, object>
                                            {
                                                {
                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["glider"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                }
                                            }
                                        },
                                        {
                                            "itemwrap", new Dictionary<string, object>
                                            {
                                                {
                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["itemwrap"]["items"].ToObject<string[]>() ?? new string[] { "", "", "", "", "", "" }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                        }
                        Console.WriteLine(AccountDataParsed.athena.RVN);
                        Console.WriteLine(AccountDataParsed.athena.CommandRevision);
                        AccountDataParsed.athena.RVN += 1;
                        AccountDataParsed.athena.CommandRevision += 1;
                        UpdatedData.Add("athena.last_applied_loadout", loadouts[Body.sourceIndex]);
                        UpdatedData.Add("athena.RVN", AccountDataParsed.athena.RVN);
                        UpdatedData.Add("athena.CommandRevision", AccountDataParsed.athena.CommandRevision);
                    }
                    else
                    {
                        throw new BaseError
                        {
                            errorCode = "errors.com.epicgames.modules.item_not_found",
                            errorMessage = "Couldnt find loadout | FR",
                            messageVars = new List<string> { "CopyCosmeticLoadout" },
                            numericErrorCode = 12801,
                            originatingService = "any",
                            intent = "prod",
                            error_description = "Couldnt find loadout | FR",
                        };
                    }

                    await Handlers.UpdateOne<Account>("accountId", AccountDataParsed.AccountId, UpdatedData);
                }
                else
                {
                    string RandomNewId = Guid.NewGuid().ToString();
                    Dictionary<string, object> GrabbedPlaceMent = AccountDataParsed.athena.Items[GrabPlacement3] as Dictionary<string, object>;
                    object objectToModify = GrabbedPlaceMent["sandbox_loadout"];

                    List<Dictionary<string, object>> itemList = new List<Dictionary<string, object>>();
                    if (objectToModify is JObject jsonLockerObject)
                    {

                        //GrabbedPlaceMent.Remove(AccountDataParsed.athena.last_applied_loadout);
                        //GrabbedPlaceMent[RandomNewId] = jsonLockerObject;

                        Dictionary<string, object> NewThingy = new Dictionary<string, object>
                        {
                            {
                            RandomNewId, new Dictionary<string, object>
                            {
                                { "templateId", $"CosmeticLocker:cosmeticlocker_athena" },
                                {
                                     "attributes", new Dictionary<string, object>
                                     {

                                        { "locker_slots_data", new Dictionary<string, object>
                                            {
                                                {
                                                    "slots", new Dictionary<string, object>
                                                    {
                                                        {
                                                            "musicpack", new Dictionary<string, object>
                                                            {
                                                                {
                                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["musicpack"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                                }
                                                            }
                                                        },
                                                        {
                                                            "character", new Dictionary<string, object>
                                                            {
                                                                {
                                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["character"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                                }
                                                            }
                                                        },
                                                        {
                                                            "backpack", new Dictionary<string, object>
                                                            {
                                                                {
                                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["backpack"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                                }
                                                            }
                                                        },
                                                        {
                                                            "pickaxe", new Dictionary<string, object>
                                                            {
                                                                {
                                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["pickaxe"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                                }
                                                            }
                                                        },
                                                        {
                                                            "skydivecontrail", new Dictionary<string, object>
                                                            {
                                                                {
                                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["skydivecontrail"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                                }
                                                            }
                                                        },
                                                        {
                                                            "dance", new Dictionary<string, object>
                                                            {
                                                                {
                                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["dance"]["items"].ToObject<string[]>() ?? new string[] { "", "", "", "", "" }
                                                                }
                                                            }
                                                        },
                                                        {
                                                            "loadingscreen", new Dictionary<string, object>
                                                            {
                                                                {
                                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["loadingscreen"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                                }
                                                            }
                                                        },
                                                        {
                                                            "glider", new Dictionary<string, object>
                                                            {
                                                                {
                                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["glider"]["items"].ToObject<string[]>() ?? new string[] { "" }
                                                                }
                                                            }
                                                        },
                                                        {
                                                            "itemwrap", new Dictionary<string, object>
                                                            {
                                                                {
                                                                    "items", jsonLockerObject["attributes"]["locker_slots_data"]["slots"]["itemwrap"]["items"].ToObject<string[]>() ?? new string[] { "", "", "", "", "", "" }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        { "use_count", 0 },
                                        { "banner_color_template", ""},
                                        { "banner_icon_template", ""},
                                        { "locker_name", Body.optNewNameForTarget ?? $"FortBackend~{RandomNewId}" },
                                        { "item_seen", false},
                                        { "favorite", false},
                                    }
                                },
                                {  "quantity", 1 }
                            }
                            }
                        };


                        //GrabbedPlaceMent[RandomNewId] = jsonLockerObject;

                        itemList.Add(NewThingy);


                        //Console.WriteLine($"Modified Locker: {JsonConvert.SerializeObject(jsonLockerObject)}");
                    }

                    await Handlers.PushOne<Account>("accountId", AccountDataParsed.AccountId, new Dictionary<string, object>
                    {
                        {
                            $"athena.items", itemList
                        }
                    });


                    await Handlers.PushOne<Account>("accountId", AccountDataParsed.AccountId, new Dictionary<string, object>
                    {
                        {
                            $"athena.loadouts", RandomNewId
                        }
                    }, false);
                }

                Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, AccountDataParsed);
                response.profileRevision = AccountDataParsed.athena.RVN;
                response.profileChangesBaseRevision = BaseRev;
                response.profileCommandRevision = AccountDataParsed.athena.CommandRevision;
                return response;
            }

            return new Mcp();
        }
    }
}