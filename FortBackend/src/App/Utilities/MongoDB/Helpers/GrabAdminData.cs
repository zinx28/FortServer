using FortLibrary.MongoDB.Modules;
using FortLibrary;
using MongoDB.Driver;
using FortLibrary.EpicResponses.Oauth;
using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using Newtonsoft.Json;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics.Dashboard;

namespace FortBackend.src.App.Utilities.MongoDB.Helpers
{
    public class GrabAdminData
    {
        public static List<AdminProfileCacheEntry> AdminUsers { get; set; } = new List<AdminProfileCacheEntry>();
        // theres better ways and this is the most retarded way i think

        public static async Task<bool> EditAdmin(AdminInfo adminInfo)
        {
            try
            {
                if (MongoDBStart.Database != null)
                {
                    if(AdminUsers == null)
                        AdminUsers = new List<AdminProfileCacheEntry>();

                    IMongoCollection<AdminInfo> AdminInfocollection = MongoDBStart.Database.GetCollection<AdminInfo>("AdminInfo");

                    var filter = Builders<AdminInfo>.Filter.Eq(x => x.DiscordId, adminInfo.DiscordId);

                 
                    AdminInfocollection.ReplaceOne(filter, adminInfo);

                    AdminProfileCacheEntry? CheckAdminUser = AdminUsers.FirstOrDefault(e => e.adminInfo.AccountId == adminInfo.AccountId);
                    if (CheckAdminUser != null)
                    {
                        AdminData? cachedAdminData = Saved.Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AdminUserEmail == CheckAdminUser.profileCacheEntry.UserData.Email);
                        if(cachedAdminData != null)
                        {
                            Logger.Log("CHANGING ROLES");
                            cachedAdminData.RoleId = adminInfo.Role;
                        }

                        CheckAdminUser.adminInfo = adminInfo;
                    }

                    //var existingAdmin = await AdminInfocollection.Find(x => x.DiscordId == adminInfo.DiscordId).FirstOrDefaultAsync();
                    Logger.Log($"Admin with Discord ID {adminInfo.DiscordId} updated successfully.", "ADMIN ADD PANEL");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "ADMIN ADD PANEL");
            }

            return false;
        }

        // Newer Version (this will take over in a future update)
        public static async Task<bool> EditAdminV2(string username, AdminDataInfo adminInfo, AdminData adminData)
        {
            try
            {
                if (MongoDBStart.Database != null)
                {
                    IMongoCollection<AdminInfo> AdminInfocollection = MongoDBStart.Database.GetCollection<AdminInfo>("AdminInfo");

                    var filter = Builders<AdminInfo>.Filter.Eq(x => x.DiscordId, adminInfo.DiscordId);
                    var existingAdmin = await AdminInfocollection.Find(filter).FirstOrDefaultAsync();

                    if (existingAdmin == null) return false;

                    var updateDefinition = Builders<AdminInfo>.Update.Combine();

                    if (existingAdmin.Role != adminInfo.Role)
                    {
                        updateDefinition = updateDefinition.Set(x => x.Role, adminInfo.Role);
                    }

                    if (updateDefinition == Builders<AdminInfo>.Update.Combine()) return false;


                    var updateResult = await AdminInfocollection.UpdateOneAsync(filter, updateDefinition);

                    if (updateResult.ModifiedCount > 0)
                    {
                        Logger.Warn(adminInfo.AccountId);
                        AdminProfileCacheEntry checkvalue = AdminUsers.FirstOrDefault(e => e.adminInfo.AccountId == adminInfo.AccountId)!;
                        if (checkvalue != null)
                        {
                            Logger.Warn(checkvalue.profileCacheEntry.UserData.Email);
                            Logger.Warn(JsonConvert.SerializeObject(Saved.Saved.CachedAdminData.Data));
                            AdminData cachedAdminData = Saved.Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AdminUserEmail == checkvalue.profileCacheEntry.UserData.Email)!;
                            if (cachedAdminData != null)
                            {
                                Logger.Log("CHANGING ROLES (THIS IS DATA IF THE USER LOGGED IN)");

                                
                                cachedAdminData.RoleId = adminInfo.Role;
                            }

                            if (checkvalue.adminInfo.Role != adminInfo.Role)
                            {
                                checkvalue.adminInfo.Role = adminInfo.Role;
                                EditAdminClass.Send(username, existingAdmin, adminInfo, adminData);
                            }
                        }

                        //var existingAdmin = await AdminInfocollection.Find(x => x.DiscordId == adminInfo.DiscordId).FirstOrDefaultAsync();
                        Logger.Log($"Admin with Discord ID {adminInfo.DiscordId} updated successfully.", "ADMIN EDIT PANEL V2");

                        return updateResult.ModifiedCount > 0;
                    }
                }   
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "ADMIN ADD PANEL");
            }

            return false;
        }

        public class NewAdminClass 
        {
            public string UserName { get; set; } = string.Empty;
            public string AccountID { get; set; } = string.Empty;
        }


        public static async Task<NewAdminClass> AddAdmin(string DiscordIds, AdminData adminData)
        {
            try
            {
                if (MongoDBStart.Database != null)
                {
                    IMongoCollection<AdminInfo> AdminInfocollection = MongoDBStart.Database.GetCollection<AdminInfo>("AdminInfo");

                    var existingAdmin = await AdminInfocollection.Find(x => x.DiscordId == DiscordIds).FirstOrDefaultAsync();

                    if (existingAdmin == null)
                    {

                        ProfileCacheEntry profileCacheEntry = await GrabData.ProfileDiscord(DiscordIds);
                        if (!string.IsNullOrEmpty(profileCacheEntry.AccountId))
                        {
                            AdminInfo newAdmin = new AdminInfo
                            {
                                AccountId = profileCacheEntry.UserData.AccountId,
                                DiscordId = profileCacheEntry.UserData.DiscordId
                            };
                            await AdminInfocollection.InsertOneAsync(newAdmin);

                            await GrabAdminData.GrabAllAdmin();

                            // Send a message in detection logs

                            NewAdmin.Send(profileCacheEntry.UserData.Username, adminData);

                            Logger.Log($"Admin with Discord ID {DiscordIds} added successfully.", "ADMIN ADD PANEL");

                            return new NewAdminClass
                            {
                                UserName = profileCacheEntry.UserData.Username,
                                AccountID = profileCacheEntry.AccountId
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "ADMIN ADD PANEL");
            }

            return new();
        }

        public static async Task GrabAllAdmin()
        {
            //AdminUsers
            try
            {
                Logger.Log("Grabbing all admins", "ADMIN PANEL");
                AdminUsers.Clear();
                if (MongoDBStart.Database != null)
                {
                    
                    IMongoCollection<AdminInfo> AdminInfocollection = MongoDBStart.Database.GetCollection<AdminInfo>("AdminInfo");
                 
                    List<AdminInfo> adminInfoList = await AdminInfocollection.Find(_ => true).ToListAsync();
                 
                    foreach (AdminInfo adminInfo in adminInfoList)
                    {
                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(adminInfo.AccountId);
             
                        if (!string.IsNullOrEmpty(profileCacheEntry.AccountId))
                        {
                            Logger.Log(adminInfo.AccountId);
                            AdminUsers.Add(new AdminProfileCacheEntry() {
                                profileCacheEntry = profileCacheEntry,
                                adminInfo = adminInfo
                            });
                        }
                    }
                    //var UserData = await Handlers.
                    //var UserData = await Handlers.FindOne<User>("email", UsersEmail);


                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "ADMIN PANEL");
            }
        }

        public static async Task<bool> ChangeForcedAdminPassword(string Email, string Password)
        {
            if(File.Exists(PathConstants.CachedPaths.FortConfig))
            {
                if (!string.IsNullOrEmpty(Saved.Saved.DeserializeConfig.AdminEmail)) // why would it if the default value isnt
                {
                    AdminData adminData = Saved.Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AdminUserEmail == Saved.Saved.DeserializeConfig.AdminEmail)!;
                    if (adminData != null)
                    {
                        FortConfig DeserializeConfig = new FortConfig();
                        string FortConfigStr = File.ReadAllText(PathConstants.CachedPaths.FortConfig);

                        if (!string.IsNullOrEmpty(FortConfigStr))
                            DeserializeConfig = JsonConvert.DeserializeObject<FortConfig>(FortConfigStr) ?? new();

                        adminData.AdminUserEmail = Email;
                        adminData.bIsSetup = false;

                        DeserializeConfig.AdminEmail = Email;
                        DeserializeConfig.AdminPassword = Password;

                        Saved.Saved.DeserializeConfig.AdminEmail = Email;
                        Saved.Saved.DeserializeConfig.AdminPassword = Password;

                        File.WriteAllText(PathConstants.CachedPaths.FortConfig, JsonConvert.SerializeObject(DeserializeConfig, Formatting.Indented));

                        return true;
                    }
                }
            }

            return false;
        }


    }
}
