using FortBackend.src.App.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.Storefront
{
    [ApiController]
    [Route("fortnite/api/storefront/v2")]
    public class KeychainApiController : ControllerBase
    {
        [HttpGet("keychain")]
        public IActionResult GrabKeychain([FromServices] IMemoryCache memoryCache)
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

                var cacheKey = $"KeychainEndpointKey";
                if (memoryCache.TryGetValue(cacheKey, out string? cachedResult))
                {
                    if (cachedResult != null) { return Content(cachedResult); }
                }

                memoryCache.Set(cacheKey, json, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

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
