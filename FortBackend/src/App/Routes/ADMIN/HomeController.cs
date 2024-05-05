using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics;
using FortLibrary.Encoders;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace FortBackend.src.App.Routes.ADMIN
{
    [Route("/admin")]
    public class HomeController : Controller
    {
        [HttpGet("login")]
        public IActionResult Index()
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
                            return Redirect("/admin/setup");
                        }
                        else
                        {
                            return Redirect("/admin/dashboard");
                        }
                    }
                }
            }
          

            return View("~/src/App/Utilities/ADMIN/Pages/Index.cshtml");
        }

        public IActionResult ReturnErrorMessage(string Message = "Server Issue", string PageName = "Index.cshtml")
        {
            ViewBag.ErrorMessage = Message;
            return View("~/src/App/Utilities/ADMIN/Pages/" + PageName);
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
                            return View("~/src/App/Utilities/ADMIN/Pages/ChangePassword.cshtml");
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
                            string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{7,}$";


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

            Console.WriteLine(email);
            Console.WriteLine($"Password {password}");
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