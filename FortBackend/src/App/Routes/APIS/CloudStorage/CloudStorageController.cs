using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Fortnite;
using FortBackend.src.App.Utilities.Helpers.Encoders;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FortBackend.src.App.Routes.APIS.CloudStorage
{
    [ApiController]
    [Route("fortnite/api/cloudstorage")]
    public class CloudStorageApiController : ControllerBase
    {
        [HttpGet("system")]
        public IActionResult SystemApi()
        {
            Response.ContentType = "application/json";
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src\\Resources\\ini");
            List<object> files = new List<object>();

            foreach (string filePath in Directory.EnumerateFiles(directoryPath).Where(f => f.EndsWith(".ini")))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                fileInfo.GetHashCode();
                files.Add(new CloudstorageFile
                {
                    uniqueFilename = fileInfo.Name,
                    filename = fileInfo.Name,
                    hash = Hex.MakeHexWithString(fileInfo.Name),
                    hash256 = Hex.MakeHexWithString2(fileInfo.Name),
                    length = fileInfo.Length,
                    contentType = "text/plain",
                    uploaded = fileInfo.CreationTimeUtc,
                    storageType = "S3",
                    doNotCache = false
                });
            }


            return new JsonResult(files);
        }

        [HttpGet("system/config")]
        public IActionResult SytemConfigApi()
        {
            return Ok(new { });
        }

        [HttpGet("system/{filename}")]
        public IActionResult SystemFileApi(string filename)
        {
            Response.ContentType = "application/octet-stream";
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\ini\\{filename}");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                string fileContents = System.IO.File.ReadAllText(filePath);

                return Content(fileContents, "text/plain");
            }
            catch (Exception ex)
            {
                Logger.Error("CloudStorage FileName -> " + ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("user/{id}/{file}")]
        public async Task<IActionResult> UserApi(string id, string file)
        {
            Response.ContentType = "application/octet-stream";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FortBackend", "ClientSettings", $"ClientSettings-{id}.sav");

            if (System.IO.File.Exists(filePath))
            {
                byte[] fileContent = System.IO.File.ReadAllBytes(filePath);
                
                return File(fileContent, "application/octet-stream");
            }

            return NoContent();
        }

        [HttpPut("user/{accountId}/{file}")]
        public async Task<IActionResult> PutUserApi(string accountId, string file)
        {
            Response.ContentType = "application/octet-stream";
            if (Request.ContentLength.HasValue && Request.ContentLength.Value >= 400000)
            {
                Console.WriteLine("TOO BIG!");
                return StatusCode(403);
            }

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.Latin1))
            {
                string requestBody = await reader.ReadToEndAsync();

                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FortBackend", "ClientSettings");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var UserData = await Handlers.FindOne<User>("accountId", accountId);
                if (UserData != "Error")
                {
                    System.IO.File.WriteAllText(Path.Combine(folderPath, $"ClientSettings-{accountId}.Sav"), requestBody, Encoding.Latin1);
                    StatusCode(204);
                }

                reader.Close();
            }
            return NoContent();
        }

        [HttpGet("user/{accountId}")]
        public IActionResult IdUserApi(string accountId)
        {
            Response.ContentType = "application/json";
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FortBackend", "ClientSettings", $"ClientSettings-{accountId}.sav");

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
                            hash = "sd", // need to do this in today
                            hash256 = "973124FFC4A03E66D6A4458E587D5D6146F71FC57F359C8D516E0B12A50AB0D9",
                            length = fileContents.Length,
                            contentType = "application/octet-stream",
                            uploaded = fileInfo.LastWriteTime,
                            storageType = "S3",
                            storageIds = new { },
                            accountId = accountId,
                            doNotCache = false
                        }
                    });
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
