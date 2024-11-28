using Discord;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics.Dashboard;
using FortLibrary.Encoders;
using FortLibrary.MongoDB.Modules;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace FortBackend.src.App.Routes.ADMIN
{
    [Route("/admin")]
    public class HomeController : Controller
    {
        // this is get but as post
        [HttpPost("new/login/check")] // Used to check if the user has access to the page.. this data could change +
                                      // a websocket would prob be better
        public IActionResult LoginCheck()
        {
            Response.ContentType = "application/json";

            try
            {
                var authToken = Request.Headers["Authorization"].ToString().ToLower().Split("bearer ")[1];

                if (!string.IsNullOrEmpty(authToken))
                {
                    Console.WriteLine(authToken);
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken.ToLower() == authToken)!;
                    Console.WriteLine(JsonConvert.SerializeObject(adminData));
                    if (adminData != null)
                    {
                        if (adminData.bIsSetup)
                        {
                            return Json(new { displayName = adminData.AdminUserName, setup = true });
                        }

                        return Json(new { displayName = adminData.AdminUserName, setup = false });

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "LoginCheck");
            }

            return Json(new { message = "Not Logged In", error = true });
        }

        [HttpPost("new/dashboard")] // Used to check if the user has access to the page.. this data could change +
                                      // a websocket would prob be better
        public IActionResult DashboardCheck()
        {
            Response.ContentType = "application/json";

            try
            {
                var authToken = Request.Headers["Authorization"].ToString().ToLower().Split("bearer ")[1];

                if (!string.IsNullOrEmpty(authToken))
                {
                    Console.WriteLine(authToken);
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken.ToLower() == authToken)!;
                    Console.WriteLine(JsonConvert.SerializeObject(adminData));
                    if (adminData != null)
                    {
                        if (adminData.bIsSetup)
                        {
                            return Json(new { message = "Not Logged In", error = true });
                        }

                        return Json(new
                        {
                            displayName = adminData.AdminUserName,
                            PeopleConnected = GlobalData.Clients.Count,
                            ForceSeason = Saved.DeserializeGameConfig.ForceSeason,
                            SeasonForced = Saved.DeserializeGameConfig.Season,
                            setup = false
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "LoginCheck");
            }

            // Any times it fails logout!
            return Json(new { error = "Not Logged In" });
        }

        [HttpPost("new/dashboard/panel")] // Used to check if the user has access to the page.. this data could change +
                                    // a websocket would prob be better
        public IActionResult DashboardPanelCheck()
        {
            Response.ContentType = "application/json";

            try
            {
                var authToken = Request.Headers["Authorization"].ToString().ToLower().Split("bearer ")[1];

                if (!string.IsNullOrEmpty(authToken))
                {
                    Console.WriteLine(authToken);
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken.ToLower() == authToken)!;
                    Console.WriteLine(JsonConvert.SerializeObject(adminData));
                    if (adminData != null)
                    {
                        if (adminData.bIsSetup)
                        {
                            return Json(new { message = "Not Logged In", error = true });
                        }

                        return Json(new
                        {
                            displayName = adminData.AdminUserName,
                            roleId = adminData.RoleId,
                            moderator = adminData.RoleId > AdminDashboardRoles.Moderator,
                            AdminLists = GrabAdminData.AdminUsers.Select(admin => new
                            {
                                UserData = admin.profileCacheEntry.UserData,
                                AccountId = admin.profileCacheEntry.AccountId,
                                adminInfo = admin.adminInfo
                            }).ToList(),
                            SeasonForced = Saved.DeserializeGameConfig.Season,
                            setup = false
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "LoginCheck");
            }

            // Any times it fails logout!
            return Json(new { error = "Not Logged In" });
        }


        [HttpPost("new/login")]
        public IActionResult LoginDS()
        {
            Response.ContentType = "application/json";
            try
            {
                var email = "";
                var password = "";
                var FormRequest = HttpContext.Request.Form;
               
                if (FormRequest.TryGetValue("email", out var emailL))
                {
                    Console.WriteLine("GI " + emailL);
                    email = emailL;
                }
                if (FormRequest.TryGetValue("password", out var passwordL))
                {
                    password = passwordL;
                }

                if (email == DefaultValues.AdminEmail)
                {
                    if (password == DefaultValues.AdminPassword)
                    {
                        // setup won't show again
                        if (email != Saved.DeserializeConfig.AdminEmail) { return Ok(new { message = "Invalid email or password.", Token = "", setup = false, error = true }); }
                        if (password != Saved.DeserializeConfig.AdminPassword) { return Ok(new { message = "Invalid email or password.", Token = "", setup = false, error = true }); }

                        var Token = JWT.GenerateRandomJwtToken(24, Saved.DeserializeConfig.JWTKEY);

                        Saved.CachedAdminData.Data.Add(new AdminData
                        {
                            AccessToken = Token,
                            AdminUserEmail = email,
                            IsForcedAdmin = email == Saved.DeserializeConfig.AdminEmail, // "NOT" default values (in this context doesnt matter)
                            RoleId = AdminDashboardRoles.Admin,
                            bIsSetup = true
                        });

                        Response.Cookies.Delete("AuthToken");

                        Response.Cookies.Append("AuthToken", Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = false,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTime.UtcNow.AddDays(7)
                        });

                        Console.WriteLine("ADDED TOKEN!");


                        return Ok(new { message = "Login successful", Token, setup = true }); // setup? ig
                    }
                }


                if (email == Saved.DeserializeConfig.AdminEmail)
                {
                    if (password == Saved.DeserializeConfig.AdminPassword)
                    {
                        var Token = JWT.GenerateRandomJwtToken(24, Saved.DeserializeConfig.JWTKEY);

                        AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AdminUserEmail == email);
                        if (adminData != null)
                        {
                            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                            {
                                Console.WriteLine(authToken);
                                Console.WriteLine(adminData.AccessToken);
                                if (adminData.AccessToken == authToken)
                                {
                                    return Ok(new { message = "Login successful", Token, setup = false });
                                }

                            }
                        }
                        else
                        {
                            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                            {
                                Console.WriteLine(authToken);
                                if (adminData != null)
                                {
                                    Console.WriteLine(adminData.AccessToken);
                                    if (adminData.AccessToken == authToken)
                                    {
                                        return Ok(new { message = "Login successful", Token, setup = false });
                                    }
                                }
                            }

                            Saved.CachedAdminData.Data.Add(new AdminData
                            {
                                AccessToken = Token,
                                AdminUserEmail = email,
                                IsForcedAdmin = email == Saved.DeserializeConfig.AdminEmail,
                                RoleId = AdminDashboardRoles.Admin
                            });
                        }
                        Response.Cookies.Delete("AuthToken");

                        Response.Cookies.Append("AuthToken", Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = false,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTime.UtcNow.AddDays(7)
                        });

                        return Ok(new { message = "Login successful", Token, setup = false });
                    }
                }
                else
                {

                    // Mods, Admin accounts

                    AdminProfileCacheEntry FoundAcc = GrabAdminData.AdminUsers?.FirstOrDefault(e => e.profileCacheEntry.UserData.Email == email)!;
                    if (FoundAcc != null && !string.IsNullOrEmpty(FoundAcc.profileCacheEntry.AccountId))
                    {
                        if (string.IsNullOrEmpty(password))
                        {
                            return Ok(new { message = "Invalid email or password.", Token = "", setup = false, error = true }); 
                        }

                        ProfileCacheEntry profileCacheEntry = FoundAcc.profileCacheEntry;

                        if (profileCacheEntry != null)
                        {
                            Console.WriteLine("TEST");
                            if (CryptoGen.VerifyPassword(password, profileCacheEntry.UserData.Password))
                            {
                                var Token = JWT.GenerateRandomJwtToken(24, Saved.DeserializeConfig.JWTKEY);

                                AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AdminUserEmail == email)!;
                                if (adminData != null)
                                {
                                    if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                                    {
                                        if (adminData.AccessToken == authToken)
                                        {
                                            return Ok(new { message = "Login successful", Token, setup = false });
                                        }
                                    }
                                }
                                else
                                {
                                    if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                                    {
                                        if (adminData != null)
                                        {
                                            if (adminData.AccessToken == authToken)
                                            {
                                                return Ok(new { message = "Login successful", Token, setup = false });
                                            }
                                        }
                                    }

                                    Saved.CachedAdminData.Data.Add(new AdminData
                                    {
                                        AccessToken = Token,
                                        AdminUserEmail = FoundAcc.profileCacheEntry.UserData.Email,
                                        AdminUserName = FoundAcc.profileCacheEntry.UserData.Username,
                                        IsForcedAdmin = email == Saved.DeserializeConfig.AdminEmail,
                                        RoleId = FoundAcc.adminInfo.Role
                                    });
                                }

                                Response.Cookies.Delete("AuthToken");

                                Response.Cookies.Append("AuthToken", Token, new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = false,
                                    SameSite = SameSiteMode.Lax,
                                    Expires = DateTime.UtcNow.AddDays(7)
                                });


                                return Ok(new { message = "Login successful", Token, setup = false });
                            }
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                Logger.Error(ex.Message, "LoginDS");
            }

            return Ok(new { message = "Invalid email or password.", Token = "", setup = false, error = true }); // setup? ig
        }

        [HttpPost("new/login/setup")]
        public async Task<IActionResult> LoginSetupDS()
        {
            Response.ContentType = "application/json";
            try
            {
                var email = "";
                var password = "";
                var cumpassword = "";
                var FormRequest = HttpContext.Request.Form;


                if (FormRequest.TryGetValue("email", out var emailL))
                {
                    Console.WriteLine(email);
                    email = emailL;
                }
                if (FormRequest.TryGetValue("password", out var passwordL))
                {
                    password = passwordL;
                }
                if (FormRequest.TryGetValue("confirmPassword", out var CumpasswordL))
                {
                    cumpassword = CumpasswordL;
                }

  

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password)) {
                    string authToken = "";
                    if (Request.Cookies.TryGetValue("AuthToken", out authToken!))
                    {
                        AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken)!;

                        if (adminData != null)
                        {
                            // idk why i didnt do this check before... :skull:

                            if (password != cumpassword)
                            {
                                return Json(new { error = "Password's doesn't match" });
                            }

                            string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&'^\-_\#]{7,}$";


                            Regex regex = new Regex(pattern);


                            if (regex.IsMatch(password))
                            {
                                if (password != DefaultValues.AdminPassword)
                                {
                                    pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                                    regex = new Regex(pattern);
                                    if (regex.IsMatch(email))
                                    {
                                        if (email != DefaultValues.AdminEmail)
                                        {
                                            // Change email and passwrod
                                            if (await GrabAdminData.ChangeForcedAdminPassword(email, password))
                                            {
                                                return Json(new { login = true });
                                            }
                                            else
                                            {
                                                return Json(new { login = false });
                                            };
                                        }
                                        else
                                        {
                                            return Json(new { message = "DONT USE DEFAULT EMAIL", error = true });
                                        }
                                    }
                                    else
                                    {
                                        return Json(new { message = "DONT USE DEFAULT EMAIL", error = true });
                                    }
                                }
                                else
                                {
                                    return Json(new { message = "Email isnt in the correct format!", error = true });
                                }
                            }
                            else
                            {
                                return Json(new { message = "Password must be at least 7 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.", error = true });
                            }
                        }
                    }
                }
                else
                {
                    return Json(new { message = "Password's doesn't match!", error = true });
                }      
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "LoginSetupDS");
            }

            return Json(new { message = "Not Logged In", error = true });
        }

        [HttpGet("login")]
        public IActionResult Index()
        {
            Logger.Error("The dashboard is moved! These endpoints will be deleted in a future update!");
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {
                if (!string.IsNullOrEmpty(authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken)!;

                    if (adminData != null)
                    {
                        if (adminData.bIsSetup)
                        {
                            return Redirect("/admin/setup");
                        }
                        else
                        {
                            return Redirect("/admin/dashboard");
                        }
                    }
                }
            }
          

            return View("~/src/App/Utilities/ADMIN/Index.cshtml");
        }

        public IActionResult ReturnErrorMessage(string Message, string PageName = "Index.cshtml")
        {
            if (!string.IsNullOrEmpty(Message))
            {
                ViewBag.ErrorMessage = Message;
                return View("~/src/App/Utilities/ADMIN/" + PageName);
            }
            else
            {
                return Redirect("/admin/login");
            }
        }

        [HttpGet("setup")]
        public IActionResult SetupEndpoint()
        {
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {
                if (!string.IsNullOrEmpty(authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken)!;

                    if (adminData != null)
                    {
                        if (adminData.bIsSetup)
                        {
                            return View("~/src/App/Utilities/ADMIN/ChangePassword.cshtml");
                        }
                       
                        return Redirect("/admin/dashboard");
                    }

                }
            }
            return Redirect("/admin/login"); // wtfd? who trying to be :banned: #adddd
        }

        [HttpPost("setup")]
        public async Task<IActionResult> SetupEndpointPost(string email, string Password, string OtherPassword)
        {
       
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(Password))
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                {
                    if (!string.IsNullOrEmpty(authToken))
                    {
                        AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken)!;

                        if (adminData != null)
                        {
                            //if(Password == OtherPassword)
                            //{
                            string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&'^\-_\#]{7,}$";


                            Regex regex = new Regex(pattern);


                            if (regex.IsMatch(Password))
                            {
                                if (Password != DefaultValues.AdminPassword)
                                {
                                    pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                                    regex = new Regex(pattern);
                                    if (regex.IsMatch(email))
                                    {
                                        if (email != DefaultValues.AdminEmail)
                                        {
                                            if (await GrabAdminData.ChangeForcedAdminPassword(email, Password))
                                            {
                                                return Redirect("/admin/dashboard");
                                            }
                                            else
                                            {
                                                return Redirect("/admin/login");
                                            };                                         
                                        }
                                        else
                                        {
                                            return ReturnErrorMessage("DONT USE DEFAULT EMAIL", "ChangePassword.cshtml");
                                        }
                                    }
                                }
                                else
                                {
                                    return ReturnErrorMessage("DONT USE DEFAULT PASSWORD", "ChangePassword.cshtml");
                                }
                            }
                            else
                            {
                                return ReturnErrorMessage("Password must be at least 7 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.", "ChangePassword.cshtml");
                            }
                        }else
                        {
                            return Redirect("/admin/login");
                        }
                    }
                }
               // }
               // else
               // {
               //     return ReturnErrorMessage("Password Doesn't Match", "ChangePassword.cshtml");
               // } 
            }

            return ReturnErrorMessage("Server Error", "ChangePassword.cshtml");
        }



        [HttpPost("login")]
        public async Task<IActionResult> Index(string email, string password)
        {

            //Console.WriteLine(email);
            ////Console.WriteLine($"Password {password}");
            //return ReturnErrorMessage("Admin account *inactive* until you change values");
            if (email == DefaultValues.AdminEmail) {
                if (password == DefaultValues.AdminPassword) {
                    // setup won't show again
                    if (email != Saved.DeserializeConfig.AdminEmail) { return ReturnErrorMessage("Invalid email or password."); }
                    if (password != Saved.DeserializeConfig.AdminPassword) { return ReturnErrorMessage("Invalid email or password."); }
                    
                    var Token = JWT.GenerateRandomJwtToken(24, Saved.DeserializeConfig.JWTKEY);

                    Saved.CachedAdminData.Data.Add(new AdminData
                    {
                        AccessToken = Token,
                        AdminUserEmail = email,
                        IsForcedAdmin = email == Saved.DeserializeConfig.AdminEmail, // "NOT" default values (in this context doesnt matter)
                        RoleId = AdminDashboardRoles.Admin,
                        bIsSetup = true
                    });

                    Response.Cookies.Delete("AuthToken");

                    Response.Cookies.Append("AuthToken", Token, new CookieOptions
                    {
                        // HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Strict
                    });


                    return Redirect("/admin/setup"); // setup? ig
                }
            }
           
            
            // Admin account is forced so the account data should be added
            if (email == Saved.DeserializeConfig.AdminEmail)
            {
                if (password == Saved.DeserializeConfig.AdminPassword)
                {
                    Console.WriteLine($"w");
                    ViewBag.ErrorMessage = "Correct!";
                    var Token = JWT.GenerateRandomJwtToken(24, Saved.DeserializeConfig.JWTKEY);

                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AdminUserEmail == email);
                    if (adminData != null)
                    {
                        if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                        {
                            Console.WriteLine(authToken);
                            Console.WriteLine(adminData.AccessToken);
                            if (adminData.AccessToken == authToken)
                            {
                                Console.WriteLine("CORRET TOKEN!");

                                return Redirect("/admin/dashboard");
                            }

                        }
                    }
                    else
                    {
                        if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                        {
                            Console.WriteLine(authToken);
                            if (adminData != null)
                            {
                                Console.WriteLine(adminData.AccessToken);
                                if (adminData.AccessToken == authToken)
                                {
                                    Console.WriteLine("CORRET TOKEN!");
                                    return Redirect("/admin/dashboard");
                                }
                            }
                        }

                        Saved.CachedAdminData.Data.Add(new AdminData
                        {
                            AccessToken = Token,
                            AdminUserEmail = email,
                            IsForcedAdmin = email == Saved.DeserializeConfig.AdminEmail,
                            RoleId = AdminDashboardRoles.Admin
                        });
                    }
                    Response.Cookies.Delete("AuthToken");

                    Response.Cookies.Append("AuthToken", Token, new CookieOptions
                    {
                       // HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Strict
                    });

                    //HttpContext.Session.SetString("Key", "Value");

                    return Redirect("/admin/dashboard");
                }
            }
            else
            {
                //ProfileEmail(email);
                AdminProfileCacheEntry FoundAcc = GrabAdminData.AdminUsers?.FirstOrDefault(e => e.profileCacheEntry.UserData.Email == email)!;
                if(FoundAcc != null && !string.IsNullOrEmpty(FoundAcc.profileCacheEntry.AccountId))
                {
                    ProfileCacheEntry profileCacheEntry = FoundAcc.profileCacheEntry;
                    if (profileCacheEntry.UserData.Password == password)
                    {
                        ViewBag.ErrorMessage = "Correct!";
                        var Token = JWT.GenerateRandomJwtToken(24, Saved.DeserializeConfig.JWTKEY);

                        AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AdminUserEmail == email);
                        if (adminData != null)
                        {
                            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                            {
                                if (adminData.AccessToken == authToken)
                                {
                                    return Redirect("/admin/dashboard");
                                }

                            }
                        }
                        else
                        {
                            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                            {
                                if (adminData != null)
                                {
                                    if (adminData.AccessToken == authToken)
                                    {
                                        return Redirect("/admin/dashboard");
                                    }
                                }
                            }

                            Saved.CachedAdminData.Data.Add(new AdminData
                            {
                                AccessToken = Token,
                                AdminUserEmail = FoundAcc.profileCacheEntry.UserData.Email,
                                AdminUserName = FoundAcc.profileCacheEntry.UserData.Username,
                                IsForcedAdmin = email == Saved.DeserializeConfig.AdminEmail,
                                RoleId = FoundAcc.adminInfo.Role
                            });
                        }
                        Response.Cookies.Delete("AuthToken");

                        Response.Cookies.Append("AuthToken", Token, new CookieOptions
                        {
                            // HttpOnly = true,
                            Secure = false,
                            SameSite = SameSiteMode.Strict
                        });


                        return Redirect("/admin/dashboard");
                    }
                }
            }
            return ReturnErrorMessage("Invalid email or password.");
        }



        [HttpPost("new/logout")]
        public IActionResult NewLogout()
        {
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {
                if (!string.IsNullOrEmpty(authToken) && Saved.CachedAdminData.Data != null)
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        Saved.CachedAdminData.Data!.Remove(adminData);

                        return Json(new { message = "Done", error = false });
                    }
                }
            }

            return Json(new { message = "Not Logged In", error = true });
        }

        [HttpGet("logout")]
        public IActionResult Details()
        {
            if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
            {
                if(!string.IsNullOrEmpty(authToken) && Saved.CachedAdminData.Data != null)
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken == authToken);
                    if (adminData != null)
                    {
                        Saved.CachedAdminData.Data!.Remove(adminData);
                    }
                }
            }

            return Redirect("/admin/login");
        }


      
    }
}

/*
 * if (context.Request.Path.StartsWithSegments("/css") && context.Request.Path.Value.EndsWith(".css"))
//                {
//                    string cssFileName = Path.GetFileNameWithoutExtension(context.Request.Path.Value);

//                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "src/App/Utilities/ADMIN/CSS", cssFileName + ".css");

//                    Console.WriteLine(filePath);
//                    if (!System.IO.File.Exists(filePath))
//                    {
//                        context.Response.StatusCode = 404;
//                    }
//                    else
//                    {
//                        string cssContent = System.IO.File.ReadAllText(filePath);
                        
//                        Console.WriteLine(cssFileName);
//                        context.Response.ContentType = "text/css";
//                        await context.Response.WriteAsync(cssContent);
                        

                        
//                    }

                   
//                }
*/