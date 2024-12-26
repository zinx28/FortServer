using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using FortLibrary.EpicResponses.Fortnite;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using FortLibrary;
using FortLibrary.Encoders;
using FortBackend.src.XMPP.Data;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.Encoders.JWTCLASS;

namespace FortBackend.src.App.Routes.CloudStorage
{
    [ApiController]
    [Route("fortnite/api/cloudstorage")]
    public class CloudStorageApiController : ControllerBase
    {
        // this api doesnt need auth well it sends a auth but not the one we need :)
        [HttpGet("system")]
        public async Task<IActionResult> SystemApi()
        {
            Response.ContentType = "application/json";
            List<object> files = new List<object>();
            try
            {
                return new JsonResult(IniManager.CloudStorageArrayData());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "Cloudstorage | SystemApi");
            }

            return new JsonResult(files);
        }

        [HttpGet("system/config")]
        public IActionResult SytemConfigApi()
        {
            return Ok(new { });
        }

        [HttpGet("system/{filename}")]
        public async Task<IActionResult> SystemFileApi(string filename)
        {
            Response.ContentType = "application/octet-stream";
            try
            {
                if (!Regex.IsMatch(filename, "^[a-zA-Z\\-\\._]+$")) // copied from image api but should prevent people doing stupid things
                {
                    return BadRequest("Invalid parameters");
                }

                string IniManagerFile = IniManager.GrabIniFile(filename);

                if (IniManagerFile == "NotFound")
                {
                    return NotFound();
                }

                return Content(IniManagerFile, "application/octet-stream"); // why was this text plain ;(
            }
            catch (Exception ex)
            {
                Logger.Error("CloudStorage FileName -> " + ex.Message);
            }
            return StatusCode(500);
        }

        [HttpGet("user/{id}/{file}")]
        public IActionResult UserApi(string id, string file)
        {
            Response.ContentType = "application/octet-stream";
            string filePath = PathConstants.CloudSettings($"ClientSettings-{id}.sav");

            if (System.IO.File.Exists(filePath))
            {
                byte[] fileContent = System.IO.File.ReadAllBytes(filePath);

                return File(fileContent, "application/octet-stream");
            }

            return NoContent();
        }

        [HttpPut("user/{accountId}/{file}")]
        [AuthorizeToken]
        public async Task<IActionResult> PutUserApi(string accountId, string file)
        {
            Response.ContentType = "application/octet-stream";
            if (Request.ContentLength.HasValue && Request.ContentLength.Value >= 400000)
            {
                Console.WriteLine("TOO BIG!");
                return StatusCode(403);
            }

            var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;
            if (profileCacheEntry != null)
            {
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    using (StreamReader reader = new StreamReader(Request.Body, Encoding.Latin1))
                    {
                        string requestBody = await reader.ReadToEndAsync();

                        System.IO.File.WriteAllText(PathConstants.CloudSettings($"ClientSettings-{accountId}.Sav"), requestBody, Encoding.Latin1);
                        StatusCode(204);

                        reader.Close();
                    }
                }
            }
            return NoContent();
        }

        [HttpGet("user/{accountId}")]
        [AuthorizeToken]
        public async Task<IActionResult> IdUserApi(string accountId)
        {
            Response.ContentType = "application/json";
            try
            {
                var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    string filePath = IniManager.GrabIniFile($"ClientSettings-{profileCacheEntry.AccountId}.sav");

                    if (System.IO.File.Exists(filePath))
                    {
                        string fileContents = System.IO.File.ReadAllText(filePath);
                        var fileInfo = new FileInfo(filePath);

                        return Ok(new[]
                        {
                            new CloudstorageFile
                            {
                                uniqueFilename = $"ClientSettings.Sav",
                                filename = $"ClientSettings.Sav",
                                hash = Hex.MakeHexWithString(fileInfo.Name),
                                hash256 = Hex.MakeHexWithString(fileInfo.Name),
                                length = fileContents.Length,
                                contentType = "application/octet-stream",
                                uploaded = fileInfo.LastWriteTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                storageType = "S3",
                                storageIds = new { },
                                accountId = profileCacheEntry.AccountId,
                                doNotCache = false
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("IdUserApi " + ex.Message);
            }
            return Ok(new object[] { });
        }
    }
}
