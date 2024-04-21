using FortBackend.src.App.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.CloudStorage
{
    [ApiController]
    [Route("Builds/Fortnite/Content/CloudDir")]
    public class CloudDirController : ControllerBase
    {
        [HttpGet("{id}/{id2}")]
        public IActionResult CloudDirTest(string id, string id2)
        {
            Response.ContentType = "application/octet-stream";
            try
            {
                string CloudDirFull = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/Ini/CloudDir/Full.ini");
                string FullIni = System.IO.File.ReadAllText(CloudDirFull);

                return Content(FullIni, "application/octet-stream");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CloudDIR");
            }

            return NoContent();
        }
    }
}
