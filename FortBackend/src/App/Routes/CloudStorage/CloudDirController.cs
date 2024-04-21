using FortBackend.src.App.Utilities;
using Microsoft.AspNetCore.Mvc;
using SharpCompress.Common;
using System.Text.RegularExpressions;

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

        [HttpGet("ChunksV4/{number}/{chunk}")]
        public IActionResult CloudDirChunk(string number, string chunk)
        {
            Response.ContentType = "application/octet-stream";
            try
            {

                if (!Regex.IsMatch(chunk, @"^[a-zA-Z0-9\-._]+$"))
                {
                    Logger.Error("Invalid Args");
                    return BadRequest("Invalid image parameter");
                }
       
                string CloudDirFull = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src/Resources/Ini/CloudDir/Chunks/{chunk}");
                if (System.IO.File.Exists(CloudDirFull))
                {
                    byte[] FullIni = System.IO.File.ReadAllBytes(CloudDirFull);
                    //string FullIni = System.IO.File.ReadAllText(CloudDirFull);

                    return new FileContentResult(FullIni, "application/octet-stream");
                }else
                {
                    Logger.Error("Not found " + CloudDirFull);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CloudDIR_Chunks");
            }

            return NoContent();
        }
    }
}
