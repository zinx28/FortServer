using Microsoft.AspNetCore.Mvc;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Saved;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using static FortBackend.src.App.Utilities.Classes.DiscordAuth;
using System.Text;
using MongoDB.Driver;
using FortBackend.src.App.Utilities.Classes;
using FortBackend.src.App.Utilities.Helpers.Encoders;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.UserManagement;

namespace FortBackend.src.App.Routes.Development
{
    [ApiController]
    [Route("temp")]
    public class TempController : ControllerBase
    {
        private IMongoDatabase _database;
        public TempController(IMongoDatabase database)
        {
            _database = database;
        }
        [HttpGet("yeah69")]
        public IActionResult YeahImsoGayyy()
        {
            return Ok(new { });
        }

        [HttpGet("/image/{image}")]
        public IActionResult ImageEnd(string image)
        {
            try
            {
                // Prevent Weird Characters
                if (!Regex.IsMatch(image, "^[a-zA-Z\\-\\._]+$"))
                {
                    return BadRequest("Invalid image parameter");
                }
                var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/Image", image);
                if (System.IO.File.Exists(imagePath))
                {
                    return PhysicalFile(imagePath, "image/jpeg");
                }
                else
                {
                    return NotFound();
                }
            }
            catch /*(Exception ex)*/
            {

                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
