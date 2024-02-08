using FortBackend.src.App.Utilities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.APIS.Storefront
{
    [ApiController]
    [Route("fortnite/api/storefront/v2")]
    public class KeychainApiController : ControllerBase
    {
        [HttpGet("keychain")]
        public IActionResult GrabKeychain()
        {
            Response.ContentType = "application/json";
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/keychain.json");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                string json = System.IO.File.ReadAllText(filePath);

                return Content(json);
            }
            catch (Exception ex)
            {
                Logger.Error("[Keychain:Grabkeychain]: " + ex.Message);
            }
            return NotFound();
        }
    }
}
