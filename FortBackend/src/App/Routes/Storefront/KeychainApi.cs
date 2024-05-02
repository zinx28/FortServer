using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.Storefront
{
    [ApiController]
    [Route("fortnite/api/storefront/v2/keychain")]
    public class KeychainApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GrabKeychain([FromServices] IMemoryCache memoryCache)
        {
            Response.ContentType = "application/json";
            try
            {
                var cacheKey = $"KeychainEndpointKey";
                string filePath = PathConstants.Keychain;

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                string json = System.IO.File.ReadAllText(filePath);

               
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
                Logger.Error("[Keychain:Grabkeychain]: " + ex.Message, "KeyChain");
            }

            return NotFound();
        }
    }
}
