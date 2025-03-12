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
        public IActionResult CloudStorageSystem()
        {
            Response.ContentType = "application/json";
            List<object> files = new List<object>();
            try
            {
                return new JsonResult(IniManager.CloudStorageArrayData());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CloudStorageSystem");
            }

            return new JsonResult(files);
        }

        [HttpGet("system/config")]
        public IActionResult CloudStorageConfig()
        {
            return Ok(new { });
        }

        [HttpGet("system/{filename}")]
        public IActionResult CloudStorageSystemFile(string filename)
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

                return Content(IniManagerFile, "application/octet-stream"); 
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CloudStorageSystemFile");
            }
            return StatusCode(500);
        }

        [HttpGet("user/{id}/{file}")]
        [AuthorizeToken]
        public async Task<IActionResult> UserApi(string id, string file)
        {
            Response.ContentType = "application/octet-stream";
            var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;
            if (profileCacheEntry != null)
            {
                string filePath = PathConstants.CloudSettings($"ClientSettings-{profileCacheEntry.AccountId}.sav");
                //Logger.Log(filePath, "GET");
                if (System.IO.File.Exists(filePath))
                {
                    string fileContent = await System.IO.File.ReadAllTextAsync(filePath, Encoding.GetEncoding("ISO-8859-1"));
                    byte[] fileBytes = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(fileContent);


                    return File(fileBytes, "application/octet-stream", file);
                }
            }

            return NoContent();
        }

        [HttpPut("user/{accountId}/{file}")]
        [AuthorizeToken]
        public async Task<IActionResult> PutClouudStorageUser(string accountId, string file)
        {
            Response.ContentType = "application/octet-stream";

            try {
                var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;
                if (profileCacheEntry != null)
                {
                    if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                    {
                        using (StreamReader reader = new StreamReader(Request.Body, Encoding.GetEncoding("ISO-8859-1"))) // hahah so this doesnt support 256+ things
                        {
                            string requestBody = await reader.ReadToEndAsync();
                            if (requestBody.Length >= 400000)
                                return StatusCode(403);

                            System.IO.File.WriteAllText(PathConstants.CloudSettings($"ClientSettings-{profileCacheEntry.AccountId}.Sav"), requestBody, Encoding.GetEncoding("ISO-8859-1"));
                            StatusCode(204);

                            reader.Close();
                        }
                    }
                }

            } 
            catch (Exception ex)  { 
                Logger.Error(ex.Message, "PutClouudStorageUser"); 
            }
            return NoContent();
        }

        [HttpGet("user/{accountId}")]
        [AuthorizeToken]
        public async Task<IActionResult> GetCloudStorageUserData(string accountId)
        {
            Response.ContentType = "application/json";
            try
            {
                var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    string filePath = PathConstants.CloudSettings($"ClientSettings-{profileCacheEntry.AccountId}.sav");

                    if (System.IO.File.Exists(filePath))
                    {
                        string fileContents = await System.IO.File.ReadAllTextAsync(filePath);
                        var fileInfo = new FileInfo(filePath);

                        return Ok(new[]
                        {
                            new CloudstorageFile
                            {
                                uniqueFilename = $"ClientSettings.Sav",
                                filename = $"ClientSettings.Sav",
                                hash =  IniManager.GetFileHashBasicversionfrfrfr(filePath),
                                hash256 = IniManager.GetFileHashfrfr(filePath),
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
                Logger.Error(ex.Message, "GetCloudStorageUserData");
            }

            return Ok(new object[] { });
        }
    }
}
