using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.APIS.Development
{
    [ApiController]
    [Route("temp")]
    public class TempController : ControllerBase
    {
        [HttpGet("yeah69")]
        public async Task<IActionResult> YeahImsoGayyy()
        {
            return Ok(new { });
        }
    }
}
