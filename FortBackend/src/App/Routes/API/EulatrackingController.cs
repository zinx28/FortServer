using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Constants;
using FortLibrary;
using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    [Route("eulatracking/api/shared/agreements/fn")]
    public class EulatrackingController : ControllerBase
    {
        [HttpGet]
        public IActionResult EulaTracking()
        {
            Response.ContentType = "application/json";
            try
            {
                var EulaPath = PathConstants.EulaTrackingFN;

                if (System.IO.File.Exists(EulaPath))
                {
                    var ReadFile = System.IO.File.ReadAllText(EulaPath);

                    if(!string.IsNullOrEmpty(ReadFile))
                    {
                        return Content(ReadFile, "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "EulaTracking");
            }

            return Ok(new {});
        }

    }
}
