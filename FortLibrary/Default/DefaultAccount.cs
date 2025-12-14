using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Default
{
    public class AccountJsonFor
    {
        public string AccountID { get; set; } = string.Empty;
        public string DiscordId { get; set; } = string.Empty;
    }
    public class DefaultAccount
    {
        public static Account Init(AccountJsonFor CreateAccArg)
        {
            Account AccountData = new Account
            {
                AccountId = CreateAccArg.AccountID,
                athena = new Athena()
                {
                    Items = new Dictionary<string, AthenaItem>()
                    {
                        ["AthenaPickaxe:DefaultPickaxe"] = new AthenaItem
                        {
                            templateId = "AthenaPickaxe:DefaultPickaxe",
                            attributes = new AthenaItemAttributes
                            {
                                item_seen = true
                            }
                        },
                        ["AthenaGlider:DefaultGlider"] = new AthenaItem
                        {
                            templateId = "AthenaGlider:DefaultGlider",
                            attributes = new AthenaItemAttributes
                            {
                                item_seen = true
                            }
                        },
                        ["AthenaDance:EID_DanceMoves"] = new AthenaItem
                        {
                            templateId = "AthenaDance:EID_DanceMoves",
                            attributes = new AthenaItemAttributes
                            {
                                item_seen = true
                            }
                        }
                    },
                    loadouts = new()
                    {
                        "sandbox_loadout"
                    },
                    loadouts_data = new Dictionary<string, SandboxLoadout>()
                    {
                        ["sandbox_loadout"] = new SandboxLoadout() // dont need much just the default obj
                        {
                            templateId = "CosmeticLocker:cosmeticlocker_athena",
                            attributes = new SandboxLoadoutAttributes
                            {
                                locker_slots_data = new SandboxLoadoutSlots
                                {
                                    slots = new LockerSlotsData
                                    {
                                        musicpack = new Slots
                                        {
                                            items = new List<string>
                                            {
                                                ""
                                            }
                                        },
                                        character = new Slots
                                        {
                                            items = new List<string>
                                            {
                                                ""
                                            },
                                            activevariants = new()
                                        },
                                        backpack = new Slots
                                        {
                                            items = new List<string>
                                            {
                                                ""
                                            },
                                            activevariants = new()
                                        },
                                        pickaxe = new Slots
                                        {
                                            items = new List<string>
                                            {
                                                "AthenaPickaxe:DefaultPickaxe"
                                            },
                                            activevariants = new()
                                        },
                                        skydivecontrail = new Slots
                                        {
                                            items = new List<string>
                                            {
                                                ""
                                            }
                                        },
                                        dance = new Slots
                                        {
                                            items = new List<string>
                                            {
                                                "",
                                                "",
                                                "",
                                                "",
                                                "",
                                                "",
                                                ""
                                            }
                                        },
                                        loadingscreen = new Slots
                                        {
                                            items = new List<string>
                                            {
                                                ""
                                            }
                                        },
                                        glider = new Slots
                                        {
                                            items = new List<string>
                                            {
                                                "AthenaGlider:DefaultGlider"
                                            }
                                        },
                                        itemwrap = new Slots
                                        {
                                            items = new List<string>
                                            {
                                                "",
                                                "",
                                                "",
                                                "",
                                                "",
                                                "",
                                                ""
                                            }
                                        }
                                    }
                                },
                                use_count = 0,
                                banner_color_template = "",
                                banner_icon_template = "",
                                locker_name = "",
                                item_seen = false,
                                favorite = false
                            },
                            quantity = 1
                        }
                    }
                },
                commoncore = new CommonCore()
                {
                    Items = new Dictionary<string, CommonCoreItem>()
                    {
                        ["Currency"] = new CommonCoreItem
                        {
                            templateId = "Currency:MtxPurchased",
                            //attributes = new CommonCoreItemAttributes
                            //{
                            //     platform = "EpicPC"
                            // },
                            quantity = 1000
                        }
                    }
                },
                DiscordId = CreateAccArg.DiscordId
            };

            return AccountData;
        }
    }
}
