using FortLibrary.ConfigHelpers;

namespace FortBackend.src.App.Utilities.ADMIN
{
    public class DashboardConfigData
    {
        public static List<ConfigTop> GetDashboardConfigData()
        {
            FortConfig FortConfigData = Saved.Saved.DeserializeConfig;
            return new List<ConfigTop>
            {
                 new ConfigTop
                {
                    Key = "Project",
                    Description = "TBD",
                    Data = new List<ConfigData>
                    {
                        new ConfigData
                        {
                            Title = "ProjectName",
                            METADATA = "ProjectName",
                            Type = "string",
                            Value = FortConfigData.ProjectName
                        },
                        new ConfigData
                        {
                            Title = "JWTKEY",
                            METADATA = "JWTKEY",
                            Type = "string",
                            Private = true,
                            Value = FortConfigData.JWTKEY
                        },
                        new ConfigData
                        {
                            Title = "BackendPort",
                            METADATA = "BackendPort",
                            Type = "int",
                            Value = FortConfigData.BackendPort
                        },
                        new ConfigData
                        {
                            Title = "MatchmakerPort",
                            METADATA = "MatchmakerPort",
                            Type = "int",
                            Value = FortConfigData.MatchmakerPort
                        },
                        new ConfigData
                        {
                            Title = "MatchmakerIP",
                            METADATA = "MatchmakerIP",
                            Type = "string",
                            Value = FortConfigData.MatchmakerIP
                        },
                    }
                 },
                new ConfigTop
                {
                    Key = "Discord",
                    Description = "Discord configuration settings",
                    Data = new List<ConfigData>
                    {
                        new ConfigData
                        {
                            Title = "DiscordToken",
                            METADATA = "DiscordToken",
                            Type = "string",
                            Private = true,
                            Value = FortConfigData.DiscordToken
                        },
                        new ConfigData
                        {
                            Title = "bShowBotMessage",
                            METADATA = "bShowBotMessage",
                            Type = "bool",
                            Value = FortConfigData.bShowBotMessage
                        },
                        new ConfigData
                        {
                            Title = "DiscordBotMessage",
                            METADATA = "DiscordBotMessage",
                            Type = "string",
                            Value = FortConfigData.DiscordBotMessage
                        },
                        new ConfigData
                        {
                            Title = "ActivityType",
                            METADATA = "ActivityType",
                            Type = "int",
                            Value = FortConfigData.ActivityType
                        },
                        new ConfigData
                        {
                            Title = "ServerID",
                            METADATA = "ServerID",
                            Type = "ulong",
                            Value = FortConfigData.ServerID
                        },
                        new ConfigData
                        {
                            Title = "RoleID",
                            METADATA = "RoleID",
                            Type = "ulong",
                            Value = FortConfigData.RoleID
                        },
                        new ConfigData
                        {
                            Title = "ApplicationClientID",
                            METADATA = "ApplicationClientID",
                            Type = "string",
                            Value = FortConfigData.ApplicationClientID
                        },
                        new ConfigData
                        {
                            Title = "ApplicationSecret",
                            METADATA = "ApplicationSecret",
                            Type = "string",
                            Private = true,
                            Value = FortConfigData.ApplicationSecret
                        },
                        new ConfigData
                        {
                            Title = "ApplicationURI",
                            METADATA = "ApplicationURI",
                            Type = "string",
                            Value = FortConfigData.ApplicationURI
                        },
                        new ConfigData
                        {
                            Title = "ShopWebhookUrl",
                            METADATA = "ShopWebhookUrl",
                            Type = "string",
                            Private = true,
                            Value = FortConfigData.ShopWebhookUrl
                        },
                        new ConfigData
                        {
                            Title = "DetectedWebhookUrl",
                            METADATA = "DetectedWebhookUrl",
                            Type = "string",
                            Private = true,
                            Value = FortConfigData.DetectedWebhookUrl
                        }
                    }
                },
                new ConfigTop
                {
                    Key = "MongoDB",
                    Description = "Database configuration settings",
                    Data = new List<ConfigData>
                    {
                        new ConfigData
                        {
                            Title = "Name",
                            METADATA = "MongoDBConnectionName",
                            Type = "string",
                            Value = FortConfigData.MongoDBConnectionName
                        },
                        new ConfigData
                        {
                            Title = "URI",
                            METADATA = "MongoDBConnectionString",
                            Type = "string",
                            Value = FortConfigData.MongoDBConnectionString
                        }
                    }
                },
                new ConfigTop
                {
                    
                    Key = "Lazy AHHH",
                    Description = "Extra",
                    Data = new List<ConfigData>
                    {
                        new ConfigData
                        {
                            Title = "Full Locker (everyone)",
                            METADATA = "FullLockerForEveryone",
                            Type = "bool",
                            Value = FortConfigData.FullLockerForEveryone
                        },
                    }
                }
            };
        }

        public class ConfigTop
        {
            public string Key { get; set; }
            public string Description { get; set; }
            public List<ConfigData> Data { get; set; }
        }

        public class ConfigData
        {
            public string Title { get; set; }
            public string METADATA { get; set; }
            public string Type { get; set; } = "string";
            public bool Private { get; set; }
            public dynamic Value { get; set; }
        }
    }
}
