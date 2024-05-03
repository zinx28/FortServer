using FortLibrary.Encoders;
using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Utilities.ADMIN.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/src/App/Utilities/ADMIN/Pages/Index.cshtml");
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
           
            Console.WriteLine(email);
            Console.WriteLine($"Password {password}");
            if (email == Saved.Saved.DeserializeConfig.AdminEmail)
            {
                if(password == Saved.Saved.DeserializeConfig.AdminPassword)
                {
                    Console.WriteLine($"w");
                    ViewBag.ErrorMessage = "Correct!";
                    var Token = JWT.GenerateRandomJwtToken(24, Saved.Saved.DeserializeConfig.JWTKEY);

                    AdminData adminData = AdminServer.CachedAdminData.Data?.FirstOrDefault(e => e.AdminUser == email);
                    if (adminData != null)
                    {
                        if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                        {
                            Console.WriteLine(authToken);
                            Console.WriteLine(adminData.AccessToken);
                            if (adminData.AccessToken == authToken)
                            {
                                Console.WriteLine("CORRET TOKEN!");

                                return Redirect("/dashboard");
                            }
                            
                        }
                    }
                    else
                    {
                        if (Request.Cookies.TryGetValue("AuthToken", out string authToken))
                        {
                            Console.WriteLine(authToken);
                            if(adminData != null)
                            {
                                Console.WriteLine(adminData.AccessToken);
                                if (adminData.AccessToken == authToken)
                                {
                                    Console.WriteLine("CORRET TOKEN!");
                                    return Redirect("/dashboard");
                                }
                            }
                        }

                        AdminServer.CachedAdminData.Data.Add(new AdminData
                        {
                            AccessToken = Token,
                            AdminUser = email,
                            IsForcedAdmin = email == Saved.Saved.DeserializeConfig.AdminEmail
                        });
                    }
                    Response.Cookies.Delete("AuthToken");

                    Response.Cookies.Append("AuthToken", Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Strict
                    });

                    //HttpContext.Session.SetString("Key", "Value");

                    return Redirect("/dashboard");
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password.";

            return View("~/src/App/Utilities/ADMIN/Pages/Index.cshtml");
        }
    }
}
